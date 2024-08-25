using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Service
{
	public class AgentService(IServiceProvider serviceProvider) : IAgentService
	{
		private IMissionService missionService => serviceProvider.GetRequiredService<IMissionService>();

		public async Task<List<TargetModel>> GetTargetsForMissions(long agentId)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

			var agent = await dbContext.Agents.FindAsync(agentId);
			if (agent == null)
			{
				throw new Exception("Agent not found");
			}

			var targets = await dbContext.Targets
				.Where(t => t.TargetStatus == TargetStatus.Alive)
				.ToListAsync();

			return targets
				.Where(t => missionService.MeasureMissionDistance(t, agent) <= 200)
				.ToList();
		}

		public async Task<AgentModel> CreateAgentAsync(AgentDto agentDto)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				AgentModel agent = new()
				{
					Nickname = agentDto.NickName,
					Image = agentDto.PhotoUrl
				};
				await dbContext.Agents.AddAsync(agent);
				await dbContext.SaveChangesAsync();
				return agent;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<AgentModel?> GetAgentByIdAsync(long id)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);
			return await dbContext.Agents.FindAsync(id);
		}

		public async Task<IEnumerable<AgentModel>> GetAgentsAsync()
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				var agents = await dbContext.Agents.ToListAsync();
				return agents;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task UpdateAgentLocation(long id, PositionDto position)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				AgentModel? agent = await dbContext.Agents.FirstOrDefaultAsync(t => t.Id == id);
				if (agent == null)
				{
					throw new Exception("Target not found");
				}
				agent.X = position.X;
				agent.Y = position.Y;
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task MoveAgent(long id, DirectionsDto directionDto)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				AgentModel? agent = await dbContext.Agents.FirstOrDefaultAsync(t => t.Id == id);
				if (agent == null)
				{
					throw new Exception("Agent not found");
				}
				if(agent.AgentStatus == AgentStatus.Active)
				{
					throw new Exception("Agent is active");
				}
				var newAgent = directionDto.Direction.ToLower().Aggregate(agent, (currentAgent, direction) =>
				{
					(int newX, int newY) = direction switch
					{
						'w' => (currentAgent.X - 1, currentAgent.Y),
						'e' => (currentAgent.X + 1, currentAgent.Y),
						's' => (currentAgent.X, currentAgent.Y - 1),
						'n' => (currentAgent.X, currentAgent.Y + 1),
						_ => throw new Exception($"Invalid direction character: {currentAgent.X} {currentAgent.Y}")
					};

					if (newX < 0 || newX > 1000 || newY < 0 || newY > 1000)
					{
						throw new Exception($"Movement out of bounds: ({newX}, {newY})");
					}

					currentAgent.X = newX;
					currentAgent.Y = newY;

					return currentAgent;
				});
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
