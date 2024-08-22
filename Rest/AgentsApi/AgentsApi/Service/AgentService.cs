using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Service
{
	public class AgentService(ApplicationDbContext context) : IAgentService
	{
		public async Task<AgentModel> CreateAgentAsync(AgentDto agentDto)
		{
			try
			{
				AgentModel agent = new()
				{
					Nickname = agentDto.Nickname,
					Image = agentDto.Photo_url
				};
				await context.Agents.AddAsync(agent);
				await context.SaveChangesAsync();
				return agent;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<AgentModel?> GetAgentByIdAsync(long id)
			=> await context.Agents.FindAsync(id);

		public async Task<IEnumerable<AgentModel>> GetAgentsAsync()
		{
			try
			{
				var agents = await context.Agents.ToListAsync();
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
				AgentModel? agent = await context.Agents.FirstOrDefaultAsync(t => t.Id == id);
				if (agent == null)
				{
					throw new Exception("Target not found");
				}
				agent.X = position.X;
				agent.Y = position.Y;
				await context.SaveChangesAsync();
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
				AgentModel? agent = await context.Agents.FirstOrDefaultAsync(t => t.Id == id);
				if (agent == null)
				{
					throw new Exception("Agent not found");
				}
				if(agent.AgentStatus == AgentStatus.Active)
				{
					throw new Exception("Agent is active");
				}
				directionDto.direction.ToLower().Aggregate(agent, (currentAgent, direction) =>
				{
					(int newX, int newY) = direction switch
					{
						'n' => (currentAgent.X - 1, currentAgent.Y),
						's' => (currentAgent.X + 1, currentAgent.Y),
						'e' => (currentAgent.X, currentAgent.Y - 1),
						'w' => (currentAgent.X, currentAgent.Y + 1),
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
				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

	}
}
