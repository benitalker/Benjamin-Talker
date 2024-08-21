namespace AgentsApi.Models
{
	public class AgentModel
	{
		public enum Status 
		{
			Dormant,
			Active
		}
		public long Id { get; set; }
		public string Nickname { get; set; } = string.Empty;
		public string Image { get; set; } = string.Empty;
		public (int x, int y) Location { get; set; } = (-1, -1);
		public Status AgentStatus { get; set; } = Status.Dormant;
	}
}
