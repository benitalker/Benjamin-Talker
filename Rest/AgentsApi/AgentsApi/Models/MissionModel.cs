using System;
using System.ComponentModel.DataAnnotations;

namespace AgentsApi.Models
{
	public enum Status
	{
		KillPropose,
		OnTask,
		MissionEnded
	}
	public class MissionModel
	{
		public long Id { get; set; }
		[Required]
		public long TargetId { get; set; }
		public TargetModel? TargetModel { get; set; }
		[Required]
		public long AgentId { get; set; }
		public AgentModel? AgentModel { get; set; }
		public double TimeLeft { get; set; }
		public double ExecutionTime { get; set; }
		public Status MissionStatus { get; set; }
	}
}
