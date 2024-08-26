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
    public class TargetService(IServiceProvider serviceProvider) : ITargetService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new Exception(nameof(serviceProvider));
        private IMissionService MissionService => _serviceProvider.GetRequiredService<IMissionService>();

        public async Task<List<AgentModel>> GetAgentsForMissions(long targetId)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var target = await dbContext.Targets.FindAsync(targetId) ?? throw new Exception($"Target with ID {targetId} not found");
            var agents = await dbContext.Agents
                .Where(a => a.AgentStatus == AgentStatus.Dormant)
                .ToListAsync();

            return agents
                .Where(a => MissionService.MeasureMissionDistance(target, a) <= 200)
                .ToList();
        }

        public async Task<TargetModel> CreateTargetAsync(TargetDto targetDto)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var target = new TargetModel
            {
                Name = targetDto.Name,
                Image = targetDto.PhotoUrl,
                Role = targetDto.Position
            };

            await dbContext.Targets.AddAsync(target);
            await dbContext.SaveChangesAsync();
            return target;
        }

        public async Task<TargetModel?> GetTargetByIdAsync(long id)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            return await dbContext.Targets.FindAsync(id);
        }

        public async Task<IEnumerable<TargetModel>> GetTargetsAsync()
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            return await dbContext.Targets.ToListAsync();
        }

        public async Task UpdateTargetLocation(long id, PositionDto position)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var target = await dbContext.Targets.FindAsync(id) ?? throw new Exception($"Target with ID {id} not found");
            target.X = position.X;
            target.Y = position.Y;
            await dbContext.SaveChangesAsync();
        }

        public async Task MoveTarget(long id, DirectionsDto directionDto)
        {
            using var dbContext = DbContextFactory.CreateDbContext(_serviceProvider);
            var target = await dbContext.Targets.FindAsync(id) ?? throw new Exception($"Target with ID {id} not found");
            target = directionDto.Direction.ToLower().Aggregate(target, (currentTarget, direction) =>
            {
                var (newX, newY) = GetNewPosition(currentTarget, direction);
                ValidatePosition(newX, newY);
                currentTarget.X = newX;
                currentTarget.Y = newY;
                return currentTarget;
            });

            await dbContext.SaveChangesAsync();
        }

        private static (int, int) GetNewPosition(TargetModel target, char direction) => direction switch
        {
            'w' => (target.X - 1, target.Y),
            'e' => (target.X + 1, target.Y),
            's' => (target.X, target.Y - 1),
            'n' => (target.X, target.Y + 1),
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