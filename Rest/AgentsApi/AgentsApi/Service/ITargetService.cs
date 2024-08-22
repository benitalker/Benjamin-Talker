using AgentsApi.Dto;
using AgentsApi.Models;

namespace AgentsApi.Service
{
	public interface ITargetService
	{
		Task<IEnumerable<TargetModel>> GetTargetsAsync();
		Task<TargetModel?> GetTargetByIdAsync(long id);
		Task<TargetModel> CreateTargetAsync(TargetDto targetDto);
		Task UpdateTargetLocation(long id,PositionDto position);
		Task MoveTarget(long id,DirectionsDto direction);
	}
}
