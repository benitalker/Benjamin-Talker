namespace AgentClient.ViewModel
{
	public class GeneralVM
	{
		public int NumOfAgents { get; set; }
		public int NumOfActiveAgents { get; set; }
		public int NumOfTargets { get; set; }
		public int NumOfDeadTargets { get; set; }
		public int NumOfMissions { get; set; }
		public int NumOfActiveMissions { get; set; }
		public double AgentToTargetRatio { get;set;}
		public double DormentAgentsToTargetRatio { get; set; }
    }
}
