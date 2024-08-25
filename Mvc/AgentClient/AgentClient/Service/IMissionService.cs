using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public interface IMissionService
    {
        Task<List<MissionVm>> ShowAllMissions();
    }
}
