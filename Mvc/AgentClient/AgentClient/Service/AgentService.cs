using AgentClient.Models;
using AgentClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentClient.Service
{
    public class AgentService(IServiceProvider _serviceProvider) : IAgentService
    {
        private IGeneralService GeneralService => _serviceProvider.GetRequiredService<IGeneralService>();

        public async Task<List<AgentVm>> GetAllAgentsDetails()
        {
            var agentsTask = GeneralService.GetAllAgentsAsync();
            var missionsTask = GeneralService.GetAllMissionsAsync();

            await Task.WhenAll(agentsTask, missionsTask);

            var agents = await agentsTask;
            var missions = await missionsTask;

            return agents.Select(agent => CreateAgentVm(agent, missions)).ToList();
        }

        private static AgentVm CreateAgentVm(AgentModel agent, IEnumerable<MissionModel> missions)
        {
            var currentMission = missions.FirstOrDefault(m => m.AgentId == agent.Id && m.MissionStatus == MissionStatus.OnTask);
            var completedMissionsCount = missions.Count(m => m.AgentId == agent.Id && m.MissionStatus == MissionStatus.MissionEnded);

            return new AgentVm
            {
                Id = agent.Id,
                Nickname = agent.Nickname,
                Image = agent.Image,
                X = agent.X,
                Y = agent.Y,
                AgentStatus = agent.AgentStatus,
                MissionId = currentMission?.Id ?? 0,
                TimeToElimanate = currentMission?.TimeLeft ?? 0,
                NumOfKills = completedMissionsCount
            };
        }
    }
}