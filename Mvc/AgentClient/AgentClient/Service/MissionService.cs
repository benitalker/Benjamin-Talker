using AgentClient.Models;
using AgentClient.ViewModel;
using static AgentClient.Service.MissionService;

namespace AgentClient.Service
{
    public class MissionService(IServiceProvider serviceProvider) : IMissionService
    {
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
                        MissionDistance = distance,
                        MissionTimeLeft = mission.TimeLeft
                    });
                }
            }

            return missionVms;
        }

        // Utility method to calculate the distance between two points
        private double MeasureMissionDistance(TargetModel target, AgentModel agent)
            => Math.Sqrt(Math.Pow(target.X - agent.X, 2) + Math.Pow(target.Y - agent.Y, 2));

    }
}
