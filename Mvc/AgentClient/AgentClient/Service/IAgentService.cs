using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public interface IAgentService
    {
        Task<List<AgentVm>> GetAllAgentsDetails();
    }
}
