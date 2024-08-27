using AgentClient.Dto;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AgentClient.Service
{
    // Service responsible for handling user login
    public class LoginService : ILoginService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string LoginApi = "https://localhost:7034/Login";

        public LoginService(IHttpClientFactory clientFactory, IHttpContextAccessor contextAccessor)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        // Attempts to log in a user with the given ID
        public async Task<Result<bool>> PerformLoginAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<bool>.Failure("Invalid user ID");
            }

            try
            {
                using var httpClient = _clientFactory.CreateClient();
                var loginDto = new LoginDto { Id = id };

                var response = await httpClient.PostAsJsonAsync(LoginApi, loginDto);

                response.EnsureSuccessStatusCode();

                var jwtToken = await response.Content.ReadAsStringAsync();
                _contextAccessor.HttpContext?.Session.SetString("JwtToken", jwtToken);

                return Result<bool>.Success(true);
            }
            catch (HttpRequestException ex)
            {
                return Result<bool>.Failure($"Login request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }

    // Represents the result of an operation
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);
    }
}