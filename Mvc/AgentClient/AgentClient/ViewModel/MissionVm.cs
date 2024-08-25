namespace AgentClient.ViewModel
{
    public class MissionVm
    {
        public long Id {  get; set; }
        public string AgentNickName { get; set; } = string.Empty;
        public int AgentX {  get; set; }
        public int AgentY { get; set; }
        public string TargetName { get; set; } = string.Empty;
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public double MissionDistance { get; set; }
        public double MissionTimeLeft { get; set; }
    }
}
