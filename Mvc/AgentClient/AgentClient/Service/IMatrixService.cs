using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public interface IMatrixService
    {
        Task<MatrixVm> InitMatrix();
    }
}
