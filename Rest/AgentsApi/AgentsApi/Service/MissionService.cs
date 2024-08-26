using AgentsApi.Data;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentsApi.Service
{
    public class MissionService(IServiceProvider serviceProvider) : IMissionService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new Exception(nameof(serviceProvider));
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<List<MissionModel>> GetMissionsAsync()
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            return await dbContext.Missions
                .Include(m => m.AgentModel)
                .Include(m => m.TargetModel)
                .ToListAsync();
        }

        public async Task<MissionModel?> CreateMission(long agentId, long targetId)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await CreateMissionInternal(agentId, targetId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<MissionModel?> CreateMissionInternal(long agentId, long targetId)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);

            var existingMission = await dbContext.Missions
                .FirstOrDefaultAsync(m => m.AgentId == agentId && m.TargetId == targetId);

            var agent = await dbContext.Agents.FindAsync(agentId);
            var target = await dbContext.Targets.FindAsync(targetId);

            if (agent == null || target == null)
            {
                return null;
            }

            if (existingMission == null)
            {
                if (IsMissionCreationValid(agent, target))
                {
                    var mission = new MissionModel
                    {
                        TargetId = targetId,
                        AgentId = agentId,
                        TimeLeft = CalculateMissionTime(target, agent)
                    };

                    await dbContext.Missions.AddAsync(mission);
                    await dbContext.SaveChangesAsync();
                    return mission;
                }
            }
            else
            {
                existingMission.TimeLeft = CalculateMissionTime(target, agent);
                await dbContext.SaveChangesAsync();
            }
            return null;
        }

        public double MeasureMissionDistance(TargetModel target, AgentModel agent)
            => Math.Sqrt(Math.Pow(target.X - agent.X, 2) + Math.Pow(target.Y - agent.Y, 2));

        private double CalculateMissionTime(TargetModel target, AgentModel agent)
            => MeasureMissionDistance(target, agent) / 5;

        public async Task<string> MissionStatusUpdate(long missionId)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var mission = await dbContext.Missions.FindAsync(missionId);
            if (mission == null)
            {
                return string.Empty;
            }

            var agent = await dbContext.Agents.FindAsync(mission.AgentId);
            var target = await dbContext.Targets.FindAsync(mission.TargetId);

            if (target == null || agent == null)
            {
                return string.Empty;
            }

            mission.MissionStatus = MissionStatus.OnTask;
            mission.ExecutionTime = CalculateMissionTime(target, agent);
            agent.AgentStatus = AgentStatus.Active;
            target.TargetStatus = TargetStatus.Hunted;

            await dbContext.SaveChangesAsync();

            await RemoveRelatedMissions(mission);

            return "assigned";
        }

        private async Task RemoveRelatedMissions(MissionModel mission)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var missionsToDelete = await dbContext.Missions
                .Where(m => (m.AgentId == mission.AgentId || m.TargetId == mission.TargetId) 
                            && m.MissionStatus == MissionStatus.KillPropose)
                .ToListAsync();
            dbContext.RemoveRange(missionsToDelete);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateMissions()
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var missionsToUpdate = await dbContext.Missions
                .Where(m => m.MissionStatus == MissionStatus.OnTask)
                .ToListAsync();

            await Task.WhenAll(missionsToUpdate.Select(m => MissionUpdate(m.Id)));
        }

        private async Task MissionUpdate(long missionId)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var mission = await dbContext.Missions.FindAsync(missionId);
            if (mission == null)
            {
                return;
            }

            var agent = await dbContext.Agents.FindAsync(mission.AgentId);
            var target = await dbContext.Targets.FindAsync(mission.TargetId);

            if (target == null || agent == null || !IsMissionValidToUpdate(mission, agent, target))
            {
                return;
            }

            if (agent.X == target.X && agent.Y == target.Y)
            {
                CompleteMission(target, agent, mission);
            }
            else
            {
                UpdateAgentPosition(agent, target);
                mission.TimeLeft = CalculateMissionTime(target, agent);
            }

            await dbContext.SaveChangesAsync();
        }

        private static void CompleteMission(TargetModel target, AgentModel agent, MissionModel mission)
        {
            target.TargetStatus = TargetStatus.Dead;
            target.X = -1;
            target.Y = -1;
            agent.AgentStatus = AgentStatus.Dormant;
            mission.MissionStatus = MissionStatus.MissionEnded;
            mission.TimeLeft = 0;
        }

        private static void UpdateAgentPosition(AgentModel agent, TargetModel target)
        {
            var (x, y) = MoveAgentTowardsTarget(agent, target);
            agent.X = x;
            agent.Y = y;
        }

        private static bool IsMissionValidToUpdate(MissionModel mission, AgentModel agent, TargetModel target)
            => agent.AgentStatus == AgentStatus.Active
               && target.TargetStatus == TargetStatus.Hunted
               && mission.MissionStatus == MissionStatus.OnTask;

        private static bool IsAgentValidForMission(AgentModel agent)
            => agent.AgentStatus == AgentStatus.Dormant;

        private static bool IsTargetValidForMission(TargetModel target)
            => target.TargetStatus == TargetStatus.Alive;

        private static bool IsMissionCreationValid(AgentModel agent, TargetModel target)
            => IsAgentValidForMission(agent) && IsTargetValidForMission(target);

        private static (int x, int y) MoveAgentTowardsTarget(AgentModel agent, TargetModel target)
        {
            int newX = agent.X + Math.Sign(target.X - agent.X);
            int newY = agent.Y + Math.Sign(target.Y - agent.Y);
            return (newX == agent.X && newY == agent.Y) ? (-10, -10) : (newX, newY);
        }
    }
}
