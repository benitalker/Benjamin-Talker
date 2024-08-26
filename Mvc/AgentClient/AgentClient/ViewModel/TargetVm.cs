using AgentClient.Models;

namespace AgentClient.ViewModel
{
    public class TargetVm
    {
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public TargetStatus TargetStatus { get; set; }
    }
}
