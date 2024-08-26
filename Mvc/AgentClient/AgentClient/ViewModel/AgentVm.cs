using AgentClient.Models;

namespace AgentClient.ViewModel
{
    public class AgentVm
    {
        public long Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public AgentStatus AgentStatus { get; set; }
        public long MissionId { get; set; }
        public double TimeToElimanate { get; set; }
        public long NumOfKills { get; set; }
    }
}
