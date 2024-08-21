using AgentsApi.Data;
using AgentsApi.Dto;
using AgentsApi.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AgentsApi.Service
{
	public class TargetService(ApplicationDbContext context) : ITargetService
	{
		public async Task<TargetModel> CreateTargetAsync(TargetDto targetDto)
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

		public async Task<List<TargetModel>> GetTargetsAsync()
			=> await context.Targets.ToListAsync();

		public async Task<TargetModel?> GetTargetByIdAsync(long id)
			=> await context.Targets.FindAsync(id);

		public async Task<TargetModel?> UpdateTargetAsync(long id,TargetModel targetModel)
		{
			TargetModel? target = await context.Targets.FirstOrDefaultAsync(t => t.Id == id);
			if (target == null)
			{
				return null;
			}
			target.Name = targetModel.Name;
			target.Image = targetModel.Image;
			target.TargetStatus = targetModel.TargetStatus;
			target.X = targetModel.X;
			target.Y = targetModel.Y;
			target.Role = targetModel.Role;

			await context.SaveChangesAsync();
			return target;
		}

		public async Task<TargetModel?> DeleteTargetAsync(long id)
		{
			TargetModel? target = await context.Targets.FirstOrDefaultAsync(t => t.Id == id);
			if (target == null)
			{
				return null;
			}
			context.Targets.Remove(target);
			await context.SaveChangesAsync();
			return target;
		}
	}
}

