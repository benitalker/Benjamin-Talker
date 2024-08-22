using AgentsApi.Data;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AgentsApi.Service
{
	public class MissionService(ApplicationDbContext context) : IMissionService
	{
		public async Task<List<MissionModel>> GetMissionsAsync()
		{
			try
			{
				var missions = await context.Missions.ToListAsync();
				return missions;
			}
			catch
			{
				throw new Exception("GetMissionsAsync Error");
			}
		}

		public async Task<MissionModel?> CreateMission(long agentId, long targetId)
		{
			try
			{
				MissionModel mission = new MissionModel()
				{
					TargetId = targetId,
					TargetModel = await context.Targets.Where(t => t.Id == targetId).FirstOrDefaultAsync(),
					AgentId = agentId,
					AgentModel = await context.Agents.Where(t => t.Id == agentId).FirstOrDefaultAsync(),
					TimeLeft = 0
				};
				await context.Missions.AddAsync(mission);
				await context.SaveChangesAsync();
				return mission;
			}
			catch
			{
				throw new Exception("Error in creating Mission");
			}
		}

		public double MeasureDistance(TargetModel target, AgentModel agent)
			=> Math.Sqrt(Math.Pow(target.X - agent.X, 2) + Math.Pow(target.Y - agent.Y, 2));

		public bool IsAgentValidToMission(AgentModel agent)
			=> agent.AgentStatus == AgentStatus.Dormant;

		public bool IsTargetValidToMission(TargetModel target)
			=> target.TargetStatus == TargetStatus.Alive;


		//1. agent pin/move and target pin/move
		//2. distance between them: done
		//3. isAgentValidToMission() => check agent status: done
		//4. isTargetValidToMission() => missions.(targetid == id).(MissionStatus == MissionStatus.OnTask).any():
	}
}
