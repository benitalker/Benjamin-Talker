using AgentClient.Models;
using AgentClient.ViewModel;
using AgentClient.Dto;
using Newtonsoft.Json;
using System.Text.Json;
using static AgentClient.Service.MissionService;

namespace AgentClient.Service
{
    public class MissionService(
        IServiceProvider serviceProvider, 
        IHttpClientFactory clientFactory,
        IHttpContextAccessor contextAccessor 
        ) : IMissionService
    {
        private readonly string _baseUrlMissions = "https://localhost:7034/Missions";
        private IGeneralService generalService => serviceProvider.GetRequiredService<IGeneralService>();
        
        public async Task<List<MissionVm>> ShowAllMissions()
        {
            // Retrieve all necessary data
            var missions = await generalService.GetAllMissionsAsync();
            var agents = await generalService.GetAllAgentsAsync();
            var targets = await generalService.GetAllTargetsAsync();

            // Initialize the list of MissionVm
            List<MissionVm> missionVms = new List<MissionVm>();

            // Iterate through each mission
            foreach (var mission in missions)
            {
                // Find the associated agent and target
                var agent = agents.FirstOrDefault(a => a.Id == mission.AgentId);
                var target = targets.FirstOrDefault(t => t.Id == mission.TargetId);

                if (agent != null && target != null)
                {
                    // Calculate the distance between agent and target
                    double distance = MeasureMissionDistance(target,agent);

                    // Create a new MissionVm instance and add it to the list
                    missionVms.Add(new MissionVm
                    {
                        Id = mission.Id,
                        AgentNickName = agent.Nickname,
                        AgentX = agent.X,
                        AgentY = agent.Y,
                        TargetName = target.Name,
                        TargetX = target.X,
                        TargetY = target.Y,
                        MissionStatus = mission.MissionStatus,
                        MissionDistance = distance,
                        MissionTimeLeft = mission.TimeLeft
                    });
                }
            }

            return missionVms;
        }

        public async Task<MissionVm?> GetMissionDetails(long id)
        {
            // Retrieve the mission by id
            var missions = await generalService.GetAllMissionsAsync();
            var mission =  missions.FirstOrDefault(m => m.Id == id);
            if (mission == null)
            {
                return null; // Or throw an exception if you prefer
            }

            // Find the associated agent and target
            var agents = await generalService.GetAllAgentsAsync();
            var agent = agents.FirstOrDefault(a => a.Id == mission.AgentId);

            var targets = await generalService.GetAllTargetsAsync();
            var target = targets.FirstOrDefault(t => t.Id == mission.TargetId);

            if (agent != null && target != null)
            {
                // Calculate the distance between agent and target
                double distance = MeasureMissionDistance(target, agent);

                // Return a new MissionVm instance
                return new MissionVm
                {
                    Id = mission.Id,
                    AgentNickName = agent.Nickname,
                    AgentX = agent.X,
                    AgentY = agent.Y,
                    TargetName = target.Name,
                    TargetX = target.X,
                    TargetY = target.Y,
                    MissionStatus = mission.MissionStatus,
                    MissionDistance = distance,
                    MissionTimeLeft = mission.TimeLeft
                };
            }
            return null;
        }

        public async Task<bool> AssignMissionToAgent(long id)
        {
            // Create an HttpClient instance using the clientFactory
            var httpClient = clientFactory.CreateClient();

            // Retrieve the JWT token from the session
            var jwtToken = contextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(jwtToken))
            {
                return false;
            }

            // Construct the request message
            using var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrlMissions}/{id}");
            // Add the Authorization header with the Bearer token
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            try
            {
                // Send the request
                var response = await httpClient.SendAsync(request);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response content
                var content = await response.Content.ReadAsStringAsync();
                var missionUpdate = JsonConvert.DeserializeObject<MissionUpdateDto>(content);

                if (missionUpdate == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        // Utility method to calculate the distance between two points
        private double MeasureMissionDistance(TargetModel target, AgentModel agent)
            => Math.Sqrt(Math.Pow(target.X - agent.X, 2) + Math.Pow(target.Y - agent.Y, 2));

    }
}
