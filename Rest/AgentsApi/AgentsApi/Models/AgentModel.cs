using System.ComponentModel.DataAnnotations;

namespace AgentsApi.Models
{
	public enum AgentStatus
	{
		Dormant,
		Active
	}
	public class AgentModel
	{
		public long Id { get; set; }
		[Required, StringLength(100, MinimumLength = 3)]
		public required string Nickname { get; set; }
		[Required, StringLength(225, MinimumLength = 3)]
		public required string Image { get; set; }
		public int X  { get; set; } = -1;
		public int Y  { get; set; } = -1;
		public AgentStatus AgentStatus { get; set; } = AgentStatus.Dormant;
	}
}
