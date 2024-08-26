using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public interface ITargetService
    {
        Task<List<TargetVm>> GetAllTargetsDetails();
    }
}
