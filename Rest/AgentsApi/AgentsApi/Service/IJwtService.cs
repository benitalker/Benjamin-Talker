namespace AgentsApi.Service
{
    public interface IJwtService
    {
        string CreateToken(string name);
        public bool ValidateToken(string authToken);
    }
}
