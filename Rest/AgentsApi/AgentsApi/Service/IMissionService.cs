using AgentsApi.Models;

namespace AgentsApi.Service
{
	public interface IMissionService
	{
		Task<List<MissionModel>> GetMissionsAsync();
		double MeasureMissionDistance(TargetModel target, AgentModel agent);
		Task<MissionModel?> CreateMission(long agentId, long targetId);
		Task<string> MissionStatusUpdate(long missionId);
		Task UpdateMissions();
	}
}
