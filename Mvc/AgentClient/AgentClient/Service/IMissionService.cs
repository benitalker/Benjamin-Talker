using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public interface IMissionService
    {
        Task<List<MissionVm>> ShowAllMissions();
        Task<MissionVm?> GetMissionDetails(long id);
        Task<bool> AssignMissionToAgent(long id);
    }
}
