namespace AgentClient.Models
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
        public long TargetId { get; set; }
        public TargetModel? TargetModel { get; set; }
        public long AgentId { get; set; }
        public AgentModel? AgentModel { get; set; }
        public double TimeLeft { get; set; } = 0;
        public double ExecutionTime { get; set; } = 0;
        public MissionStatus MissionStatus { get; set; } = MissionStatus.KillPropose;
    }
}
