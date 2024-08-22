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
			AgentModel agent = new()
			{
				Nickname = agentDto.Nickname,
				Image = agentDto.Photo_url
			};
			await context.Agents.AddAsync(agent);
			await context.SaveChangesAsync();
			return agent;
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
			catch
			{
				return Enumerable.Empty<AgentModel>();
			}	
		}

		public async Task MoveAgent(long id, DirectionsDto directionDto)
		{
			AgentModel? agent = await context.Agents.FirstOrDefaultAsync(t => t.Id == id);
			if (agent == null)
			{
				throw new Exception("Target not found");
			}
			foreach (char direction in directionDto.direction.ToLower())
			{
				switch (direction)
				{
					case 'n':
						agent.X -= 1;
						break;
					case 's':
						agent.X += 1;
						break;
					case 'e':
						agent.Y -= 1;
						break;
					case 'w':
						agent.Y += 1;
						break;
					default:
						throw new Exception($"Invalid direction character: {direction}");
				}
			}
			await context.SaveChangesAsync();
		}

		public async Task UpdateAgentLocation(long id, PositionDto position)
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
	}
}
