using AgentsApi.Models;

namespace AgentsApi.Service
{
	public interface IMissionService
	{
		Task<List<MissionModel>> GetMissionsAsync();
	}
}
