using AgentsApi.Dto;
using AgentsApi.Models;

namespace AgentsApi.Service
{
	public interface ITargetService
	{
		Task<List<TargetModel>> GetTargetsAsync();
		Task<TargetModel?> GetTargetByIdAsync(long id);
		Task<TargetModel> CreateTargetAsync(TargetDto targetDto);
		Task<TargetModel?> UpdateTargetAsync(long id, TargetModel targetModel);
		Task<TargetModel?> DeleteTargetAsync(long id);
	}
}
