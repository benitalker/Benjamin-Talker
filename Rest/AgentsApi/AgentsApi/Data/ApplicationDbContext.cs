using AgentsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentsApi.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
	{
		public DbSet<MissionModel> Missions { get; set; }
		public DbSet<AgentModel> Agents { get; set; }
		public DbSet<TargetModel> Targets { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AgentModel>()
				.Property(a => a.AgentStatus)
				.HasConversion<string>()
				.IsRequired();

			modelBuilder.Entity<TargetModel>()
				.Property(a => a.TargetStatus)
				.HasConversion<string>()
				.IsRequired();

			modelBuilder.Entity<MissionModel>()
				.Property(a => a.MissionStatus)
				.HasConversion<string>()
				.IsRequired();

			modelBuilder.Entity<MissionModel>()
				.HasOne(m => m.AgentModel)
				.WithMany()
				.HasForeignKey(m => m.AgentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<MissionModel>()
				.HasOne(m => m.TargetModel)
				.WithMany()
				.HasForeignKey(m => m.TargetId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
