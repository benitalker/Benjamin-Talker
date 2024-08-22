using AgentsApi.Data;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AgentsApi.Service
{
	public class MissionService(ApplicationDbContext context) : IMissionService
	{
		public async Task<List<MissionModel>> GetMissionsAsync()
		{
			try
			{
				var missions = await context.Missions.ToListAsync();
				return missions;
			}
			catch
			{
				return new List<MissionModel>();
			}
		}

		public async Task<MissionModel?> CreateMission(long agentId,long targetId)
		{
			try
			{
				MissionModel mission = new MissionModel()
				{
					TargetId = targetId,
					TargetModel = await context.Targets.Where(t => t.Id == targetId).FirstOrDefaultAsync(),
					AgentId = agentId,
					AgentModel = await context.Agents.Where(t => t.Id == agentId).FirstOrDefaultAsync(),
					TimeLeft = 0
				};
				await context.Missions.AddAsync(mission);
				await context.SaveChangesAsync();
				return mission;
			}
			catch
			{
				return new MissionModel();
			}
		}

		public async Task<bool> IsMissionCreateValid(long agentId, long targetId)
		{
			bool agentHasMission = await context.Missions.AnyAsync(m => m.AgentId == agentId);
			return !agentHasMission;
		}
	}
}
