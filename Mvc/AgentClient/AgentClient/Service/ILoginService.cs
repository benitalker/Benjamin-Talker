namespace AgentClient.Service
{
    public interface ILoginService
    {
        Task<Result<bool>> PerformLoginAsync(string id);
    }
}
