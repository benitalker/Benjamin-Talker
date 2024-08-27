using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AgentClient.Models;
using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public class GeneralService(IHttpClientFactory _clientFactory) : IGeneralService
    {
        private const string BaseUrlAgents = "https://localhost:7034/Agents";
        private const string BaseUrlTargets = "https://localhost:7034/Targets";
        private const string BaseUrlMissions = "https://localhost:7034/Missions";

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<List<AgentModel>> GetAllAgentsAsync() =>
            await FetchDataAsync<List<AgentModel>>(BaseUrlAgents);

        public async Task<List<TargetModel>> GetAllTargetsAsync() =>
            await FetchDataAsync<List<TargetModel>>(BaseUrlTargets);

        public async Task<List<MissionModel>> GetAllMissionsAsync() =>
            await FetchDataAsync<List<MissionModel>>(BaseUrlMissions);

        private async Task<T> FetchDataAsync<T>(string url) where T : new()
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, JsonOptions) ?? new T();
            }

            return new T();
        }

        public async Task<GeneralVM> GetGeneralStatisticsAsync()
        {
            var agentsTask = GetAllAgentsAsync();
            var targetsTask = GetAllTargetsAsync();
            var missionsTask = GetAllMissionsAsync();

            await Task.WhenAll(agentsTask, targetsTask, missionsTask);

            var agents = await agentsTask;
            var targets = await targetsTask;
            var missions = await missionsTask;

            int numOfDormantAgents = agents.Count(a => a.AgentStatus == AgentStatus.Dormant);
            int numOfAliveTargets = targets.Count(t => t.TargetStatus == TargetStatus.Alive);

            return new GeneralVM
            {
                NumOfAgents = agents.Count,
                NumOfActiveAgents = agents.Count(a => a.AgentStatus == AgentStatus.Active),
                NumOfTargets = targets.Count,
                NumOfDeadTargets = targets.Count(t => t.TargetStatus == TargetStatus.Dead),
                NumOfMissions = missions.Count,
                NumOfActiveMissions = missions.Count(m => m.MissionStatus == MissionStatus.OnTask),
                AgentToTargetRatio = CalculateRatio(agents.Count, targets.Count),
                DormentAgentsToTargetRatio = CalculateRatio(numOfDormantAgents, numOfAliveTargets)
            };
        }

        private static double CalculateRatio(int numerator, int denominator) =>
            denominator > 0 ? (double)numerator / denominator : 0;
    }
}