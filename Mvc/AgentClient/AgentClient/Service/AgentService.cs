using AgentClient.Models;
using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public class AgentService(IServiceProvider serviceProvider) : IAgentService
    {
        private IGeneralService generalService => serviceProvider.GetRequiredService<IGeneralService>();

        public async Task<List<AgentVm>> GetAllAgentsDetails()
        {
            var agents = await generalService.GetAllAgentsAsync();
            var missions = await generalService.GetAllMissionsAsync();

            var result = new List<AgentVm>();

            foreach (var agent in agents)
            {
                var currentMission = missions.FirstOrDefault(m => m.AgentId == agent.Id && m.MissionStatus == MissionStatus.OnTask);

                result.Add(new AgentVm
                {
                    Id = agent.Id,
                    Nickname = agent.Nickname,
                    Image = agent.Image,
                    X = agent.X,
                    Y = agent.Y,
                    AgentStatus = agent.AgentStatus,
                    MissionId = currentMission?.Id ?? 0, 
                    TimeToElimanate = currentMission?.TimeLeft ?? 0,
                    NumOfKills = missions.Where(m => m.AgentId == agent?.Id && m.MissionStatus == MissionStatus.MissionEnded)
                        .Count(),
                });
            }

            return result;
        }
    }
}
