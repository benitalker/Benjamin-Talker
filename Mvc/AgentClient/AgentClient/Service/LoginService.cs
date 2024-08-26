using AgentClient.Dto;
using System.Text;
using System.Text.Json;

namespace AgentClient.Service
{
    public class LoginService(IHttpClientFactory clientFactory, IHttpContextAccessor contextAccessor) : ILoginService
    {
        private readonly string _loginApi = "https://localhost:7034/Login";

        public async Task<bool> PerformLoginAsync(string id)
        {
            var httpClient = clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _loginApi);

            request.Content = new StringContent(
                JsonSerializer.Serialize(new LoginDto { Id = id }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                contextAccessor.HttpContext?.Session.SetString("JwtToken", content);
                return true;
            }
            return false;
        }
    }
}
