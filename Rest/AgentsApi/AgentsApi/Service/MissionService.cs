using AgentsApi.Data;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AgentsApi.Service
{
	public class MissionService(IServiceProvider serviceProvider) : IMissionService
	{

		public async Task<List<MissionModel>> GetMissionsAsync()
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				var missions = await dbContext.Missions
					.Include(m => m.AgentModel)
					.Include(m => m.TargetModel)
					.ToListAsync();
				return missions;
			}
			catch
			{
				throw new Exception("GetMissionsAsync Error");
			}
		}

		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		public async Task<MissionModel?> CreateMission(long agentId, long targetId)
		{
			await _semaphore.WaitAsync();
			try
			{
				return await CreateMissionInternal(agentId, targetId);
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task<MissionModel?> CreateMissionInternal(long agentId, long targetId)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				var checkMission = await dbContext.Missions
					.Where(m => m.AgentId == agentId && m.TargetId == targetId)
					.FirstOrDefaultAsync();

				var agentModel = await dbContext.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
				var targetModel = await dbContext.Targets.FirstOrDefaultAsync(t => t.Id == targetId);

				if (checkMission == null)
				{
					if (agentModel == null || targetModel == null)
					{
						return null;
					}

					if (IsMissionCreationValid(agentModel, targetModel))
					{
						var mission = new MissionModel
						{
							TargetId = targetId,
							AgentId = agentId,
							TimeLeft = MeasureMissionDistance(targetModel, agentModel) / 5
						};

						await dbContext.Missions.AddAsync(mission);
						await dbContext.SaveChangesAsync();
						return mission;
					}
				}
				else
				{
					checkMission.TimeLeft = MeasureMissionDistance(targetModel, agentModel) / 5;
					await dbContext.SaveChangesAsync();
				}
				return null;
			}
			catch (Exception ex)
			{
				throw new Exception("Error in creating Mission", ex);
			}
		}
		public double MeasureMissionDistance(TargetModel target, AgentModel agent)
			=> Math.Sqrt(Math.Pow(target.X - agent.X, 2) + Math.Pow(target.Y - agent.Y, 2));

		public async Task<string> MissionStatusUpdate(long missionId)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);
			MissionModel? mission = await dbContext.Missions.FirstOrDefaultAsync(m => m.Id == missionId);
			if (mission == null)
			{ 
				return string.Empty; 
			}
			mission.MissionStatus = MissionStatus.OnTask;

			AgentModel? agent = await dbContext.Agents.
				FirstOrDefaultAsync(a => a.Id == mission.AgentId);
			TargetModel? target = await dbContext.Targets.
				FirstOrDefaultAsync(t => t.Id == mission.TargetId);

			if(target == null || agent == null)
			{
				return string.Empty;
			}

			mission.ExecutionTime = MeasureMissionDistance(target, agent) / 5;
			agent.AgentStatus = AgentStatus.Active;
			target.TargetStatus = TargetStatus.Hunted;

			await dbContext.SaveChangesAsync();

			var missionsToDelete = await dbContext.Missions
				.Where(m => m.AgentId == mission.AgentId && m.MissionStatus == MissionStatus.KillPropose
				|| m.TargetId == mission.TargetId && m.MissionStatus == MissionStatus.KillPropose).ToListAsync();
			dbContext.RemoveRange(missionsToDelete);
			await dbContext.SaveChangesAsync();
			return "assigned";
		}

		public async Task UpdateMissions()
		{

			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);
			var missionsToUpdate = await dbContext.Missions.Where(m => m.MissionStatus == MissionStatus.OnTask).ToListAsync();
			missionsToUpdate.ForEach(async m => await MissionUpdate(m.Id));
			await dbContext.SaveChangesAsync();
		}

		private async Task MissionUpdate(long missionId)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);
			MissionModel? mission = await dbContext.Missions.
				FirstOrDefaultAsync(m => m.Id == missionId);
			if (mission == null)
			{
				return;
			}
			AgentModel? agent = await dbContext.Agents.
				FirstOrDefaultAsync(a => a.Id == mission.AgentId);
			TargetModel? target = await dbContext.Targets.
				FirstOrDefaultAsync(t => t.Id == mission.TargetId);

			if (target == null || agent == null || !isMissionValidToUpdate(mission,agent,target))
			{
				return;
			}
			if (agent.X == target.X && agent.Y == target.Y) 
			{ 
				target.TargetStatus = TargetStatus.Dead;
				agent.AgentStatus = AgentStatus.Dormant;
				mission.MissionStatus = MissionStatus.MissionEnded;
				mission.TimeLeft = 0;
				await dbContext.SaveChangesAsync();
				return;
			}
			var agentMovment = MoveAgentAfterTarget(agent,target);

			agent.X = agentMovment.x;
			agent.Y = agentMovment.y;

			mission.TimeLeft = MeasureMissionDistance(target, agent) / 5;

			await dbContext.SaveChangesAsync();
		}

		private bool isMissionValidToUpdate(MissionModel mission,AgentModel agent,TargetModel target)
		{
			return agent.AgentStatus == AgentStatus.Active
				&& target.TargetStatus == TargetStatus.Hunted
				&& mission.MissionStatus == MissionStatus.OnTask;
		}

		private bool IsAgentValidToMission(AgentModel agent)
			=> agent.AgentStatus == AgentStatus.Dormant;

		private bool IsTargetValidToMission(TargetModel target)
			=> target.TargetStatus == TargetStatus.Alive;

		private bool IsMissionCreationValid(AgentModel agent, TargetModel target)
			=> IsAgentValidToMission(agent)
			&& IsTargetValidToMission(target);

		private (int x, int y) MoveAgentAfterTarget(AgentModel agent, TargetModel target)
		{
			(int x, int y) agentLocation = (agent.X, agent.Y);
			if (agentLocation.x < target.X) agentLocation.x++;
			else if (agentLocation.x > target.X) agentLocation.x--;
			if (agentLocation.y < target.Y) agentLocation.y++;
			else if (agentLocation.y > target.Y) agentLocation.y--;
			return (agentLocation.x == agent.X && agentLocation.y == agent.Y) ? (-10, -10) : agentLocation;
		}
	}
}
