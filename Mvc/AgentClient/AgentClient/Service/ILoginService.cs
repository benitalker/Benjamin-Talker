namespace AgentClient.Service
{
    public interface ILoginService
    {
        Task<bool> PerformLoginAsync(string id);
    }
}
