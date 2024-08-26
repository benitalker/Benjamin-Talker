using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentsApi.Service
{
    public class AgentService(IServiceProvider serviceProvider) : IAgentService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new Exception(nameof(serviceProvider));
        private readonly IMissionService _missionService = serviceProvider.GetRequiredService<IMissionService>();

        public async Task<List<TargetModel>> GetTargetsForMissions(long agentId)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var agent = await dbContext.Agents.FindAsync(agentId) ?? throw new Exception($"Agent with ID {agentId} not found");
            var targets = await dbContext.Targets
                .Where(t => t.TargetStatus == TargetStatus.Alive)
                .ToListAsync();

            return targets
                .Where(t => _missionService.MeasureMissionDistance(t, agent) <= 200)
                .ToList();
        }

        public async Task<AgentModel> CreateAgentAsync(AgentDto agentDto)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var agent = new AgentModel
            {
                Nickname = agentDto.NickName,
                Image = agentDto.PhotoUrl
            };

            await dbContext.Agents.AddAsync(agent);
            await dbContext.SaveChangesAsync();
            return agent;
        }

        public async Task<AgentModel?> GetAgentByIdAsync(long id)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            return await dbContext.Agents.FindAsync(id);
        }

        public async Task<IEnumerable<AgentModel>> GetAgentsAsync()
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            return await dbContext.Agents.ToListAsync();
        }

        public async Task UpdateAgentLocation(long id, PositionDto position)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var agent = await dbContext.Agents.FindAsync(id) ?? throw new Exception($"Agent with ID {id} not found");
            agent.X = position.X;
            agent.Y = position.Y;
            await dbContext.SaveChangesAsync();
        }

        public async Task MoveAgent(long id, DirectionsDto directionDto)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var agent = await dbContext.Agents.FindAsync(id) ?? throw new Exception($"Agent with ID {id} not found");
            if (agent.AgentStatus == AgentStatus.Active)
            {
                throw new Exception("Agent is active and cannot be moved");
            }

            agent = directionDto.Direction.ToLower().Aggregate(agent, (currentAgent, direction) =>
            {
                var (newX, newY) = GetNewPosition(currentAgent, direction);
                ValidatePosition(newX, newY);
                currentAgent.X = newX;
                currentAgent.Y = newY;
                return currentAgent;
            });

            await dbContext.SaveChangesAsync();
        }

        private static (int, int) GetNewPosition(AgentModel agent, char direction) => direction switch
        {
            'w' => (agent.X - 1, agent.Y),
            'e' => (agent.X + 1, agent.Y),
            's' => (agent.X, agent.Y - 1),
            'n' => (agent.X, agent.Y + 1),
            _ => throw new Exception($"Invalid direction character: {direction}")
        };

        private static void ValidatePosition(int x, int y)
        {
            if (x < 0 || x > 1000 || y < 0 || y > 1000)
            {
                throw new Exception($"Movement out of bounds: ({x}, {y})");
            }
        }
    }
}