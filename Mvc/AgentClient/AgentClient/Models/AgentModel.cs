using System.ComponentModel.DataAnnotations;

namespace AgentClient.Models
{
	public enum AgentStatus
	{
		Dormant,
		Active
	}
	public class AgentModel
	{
		public long Id { get; set; }
		public required string Nickname { get; set; }
		public required string Image { get; set; }
		public int X { get; set; } = -1;
		public int Y { get; set; } = -1;
		public AgentStatus AgentStatus { get; set; } = AgentStatus.Dormant;
	}
}