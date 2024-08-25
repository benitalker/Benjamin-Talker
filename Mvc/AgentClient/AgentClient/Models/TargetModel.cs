using System.ComponentModel.DataAnnotations;

namespace AgentClient.Models
{
	public enum TargetStatus
	{
		Alive,
		Hunted,
		Dead
	}
	public class TargetModel
	{
		public long Id { get; set; }
		public required string Name { get; set; }
		public required string Image { get; set; }
		public required string Role { get; set; }
		public int X { get; set; } = -1;
		public int Y { get; set; } = -1;
		public TargetStatus TargetStatus { get; set; } = TargetStatus.Alive;
	}
}