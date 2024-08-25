using AgentsApi.Dto;
using AgentsApi.Models;

namespace AgentsApi.Service
{
	public interface IAgentService
	{
		Task<List<TargetModel>> GetTargetsForMissions(long agentId);
		Task<IEnumerable<AgentModel>> GetAgentsAsync();
		Task<AgentModel?> GetAgentByIdAsync(long id);
		Task<AgentModel> CreateAgentAsync(AgentDto agentDto);
		Task UpdateAgentLocation(long id, PositionDto position);
		Task MoveAgent(long id, DirectionsDto directionDto);
	}
}
