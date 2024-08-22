using System;
using System.ComponentModel.DataAnnotations;

namespace AgentsApi.Models
{
	public enum MissionStatus
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
		public double ExecutionTime { get; set; } = 0;
		public MissionStatus MissionStatus { get; set; } = MissionStatus.KillPropose;
	}
}
