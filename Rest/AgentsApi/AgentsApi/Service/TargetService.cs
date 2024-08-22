using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Service
{
	public class TargetService(ApplicationDbContext context) : ITargetService
	{
		public async Task<TargetModel> CreateTargetAsync(TargetDto targetDto)
		{
			try
			{
				TargetModel target = new()
				{
					Name = targetDto.Name,
					Image = targetDto.Photo_url,
					Role = targetDto.Position
				};
				await context.Targets.AddAsync(target);
				await context.SaveChangesAsync();
				return target;
			}
			catch (Exception ex) 
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<TargetModel?> GetTargetByIdAsync(long id)
			=> await context.Targets.FindAsync(id);

		public async Task<IEnumerable<TargetModel>> GetTargetsAsync()
		{
			try
			{
				var targets = await context.Targets.ToListAsync();
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
				TargetModel? target = await context.Targets.FirstOrDefaultAsync(t => t.Id == id);
				if (target == null)
				{
					throw new Exception("Target not found");
				}
				target.X = position.X;
				target.Y = position.Y;
				await context.SaveChangesAsync();
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
				TargetModel? target = await context.Targets.FirstOrDefaultAsync(t => t.Id == id);
				if (target == null)
				{
					throw new Exception("Target not found");
				}

				directionDto.direction.ToLower().Aggregate(target, (currentTarget, direction) =>
				{
					(int newX, int newY) = direction switch
					{
						'n' => (currentTarget.X - 1, currentTarget.Y),
						's' => (currentTarget.X + 1, currentTarget.Y),
						'e' => (currentTarget.X, currentTarget.Y - 1),
						'w' => (currentTarget.X, currentTarget.Y + 1),
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
				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}

