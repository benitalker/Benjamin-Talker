using System.Net.Http;
using System.Text.Json;
using AgentClient.Models;
using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public class GeneralService(IHttpClientFactory _clientFactory) : IGeneralService
    {
        private readonly string _baseUrlAgents = "https://localhost:7034/Agents";
        private readonly string _baseUrlTargets = "https://localhost:7034/Targets";
        private readonly string _baseUrlMissions = "https://localhost:7034/Missions";

        

        public async Task<List<AgentModel>> GetAllAgentsAsync()
        {
            var httpClient = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrlAgents);
            var result = await httpClient.SendAsync(request);

            return result.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<List<AgentModel>>(
                        await result.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                    ) ?? []
                : [];
        }

        public async Task<List<TargetModel?>> GetAllTargetsAsync()
        {
            var httpClient = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrlTargets);
            var result = await httpClient.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TargetModel>>(
                    content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TargetModel?>();
            }

            return new List<TargetModel?>();
        }

        public async Task<List<MissionModel>> GetAllMissionsAsync()
        {
            var httpClient = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrlMissions);
            var result = await httpClient.SendAsync(request);

            return result.IsSuccessStatusCode ? 
             JsonSerializer.Deserialize<List<MissionModel>>(
                    await result.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [] 
                    : [];
        }

        public async Task<GeneralVM> GetGeneralStatisticsAsync()
        {
            var generalVM = new GeneralVM();

            // Get Agents data
            var totalAgents = await GetAllAgentsAsync();
            generalVM.NumOfAgents = totalAgents.Count;
            generalVM.NumOfActiveAgents = totalAgents.Count(a => a?.AgentStatus == AgentStatus.Active);

            var numOfDormantAgents = totalAgents.Count(a => a?.AgentStatus == AgentStatus.Dormant);

            // Get Targets data
            var totalTargets = await GetAllTargetsAsync();
            generalVM.NumOfTargets = totalTargets.Count;
            generalVM.NumOfDeadTargets = totalTargets.Count(t => t?.TargetStatus == TargetStatus.Dead);

            var numOfAliveTargets = totalTargets.Count(t => t?.TargetStatus == TargetStatus.Alive);

            // Get Missions data
            var totalMissions = await GetAllMissionsAsync();
            generalVM.NumOfMissions = totalMissions.Count;
            generalVM.NumOfActiveMissions = totalMissions.Count(m => m?.MissionStatus == MissionStatus.OnTask);

            // Calculate Agent to Target ratio
            generalVM.AgentToTargetRatio = generalVM.NumOfTargets > 0 ?
                (double)generalVM.NumOfAgents / generalVM.NumOfTargets : 0;

            // Calculate Dormant Agents to Target ratio
            generalVM.DormentAgentsToTargetRatio = generalVM.NumOfTargets > 0 ?
                (double)numOfDormantAgents / (double)numOfAliveTargets : 0;

            return generalVM;
        }
    }
}
