using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgentsApi.Service
{
	public class TargetService(IServiceProvider serviceProvider) : ITargetService
	{
		private IMissionService missionService => serviceProvider.GetRequiredService<IMissionService>();

		public async Task<List<AgentModel>> GetAgentsForMissions(long targetId)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

			var target = await dbContext.Targets.FindAsync(targetId);
			if (target == null)
			{
				throw new Exception("target not found");
			}

			var agents = await dbContext.Agents
				.Where(a => a.AgentStatus == AgentStatus.Dormant)
				.ToListAsync();

			return agents
				.Where(a => missionService.MeasureMissionDistance(target, a) <= 200)
				.ToList();
		}
		public async Task<TargetModel> CreateTargetAsync(TargetDto targetDto)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				TargetModel target = new()
				{
					Name = targetDto.Name,
					Image = targetDto.PhotoUrl,
					Role = targetDto.Position
				};
				await dbContext.Targets.AddAsync(target);
				await dbContext.SaveChangesAsync();
				return target;
			}
			catch (Exception ex) 
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<TargetModel?> GetTargetByIdAsync(long id)
		{
			using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

			return await dbContext.Targets.FindAsync(id);
		}

		public async Task<IEnumerable<TargetModel>> GetTargetsAsync()
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				var targets = await dbContext.Targets.ToListAsync();
				return targets;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task UpdateTargetLocation(long id,PositionDto position)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				TargetModel? target = await dbContext.Targets.FirstOrDefaultAsync(t => t.Id == id);
				if (target == null)
				{
					throw new Exception("Target not found");
				}
				target.X = position.X;
				target.Y = position.Y;
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task MoveTarget(long id, DirectionsDto directionDto)
		{
			try
			{
				using var dbContext = DbContextFactory.CreateDbContext(serviceProvider);

				TargetModel? target = await dbContext.Targets.FirstOrDefaultAsync(t => t.Id == id);
				if (target == null)
				{
					throw new Exception("Target not found");
				}

				var newTarget = directionDto.Direction.ToLower().Aggregate(target, (currentTarget, direction) =>
				{
					(int newX, int newY) = direction switch
					{
						'w' => (currentTarget.X - 1, currentTarget.Y),
						'e' => (currentTarget.X + 1, currentTarget.Y),
						's' => (currentTarget.X, currentTarget.Y - 1),
						'n' => (currentTarget.X, currentTarget.Y + 1),
						_ => throw new Exception($"Invalid direction character: {direction}")
					};

					if (newX < 0 || newX > 1000 || newY < 0 || newY > 1000)
					{
						throw new Exception($"Movement out of bounds: ({newX}, {newY})");
					}

					currentTarget.X = newX;
					currentTarget.Y = newY;

					return currentTarget;
				});
				await dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}

