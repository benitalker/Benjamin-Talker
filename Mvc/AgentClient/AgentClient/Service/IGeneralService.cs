using AgentClient.Models;
using AgentClient.ViewModel;

namespace AgentClient.Service
{
	public interface IGeneralService
	{
        Task<List<AgentModel>> GetAllAgentsAsync();
        Task<List<TargetModel>> GetAllTargetsAsync();
        Task<List<MissionModel>> GetAllMissionsAsync();
        Task<GeneralVM> GetGeneralStatisticsAsync();
    }
}
