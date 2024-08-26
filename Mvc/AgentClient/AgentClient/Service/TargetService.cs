using AgentClient.Models;
using AgentClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AgentClient.Service
{
    public class TargetService(IServiceProvider serviceProvider) : ITargetService
    {
        private IGeneralService generalService => serviceProvider.GetRequiredService<IGeneralService>();

        public async Task<List<TargetVm>> GetAllTargetsDetails()
        {
            var targets = await generalService.GetAllTargetsAsync();

            var result = new List<TargetVm>();

            foreach (var target in targets)
            {
                result.Add(new TargetVm
                {
                    Name = target.Name,
                    Image = target.Image,
                    Role = target.Role,
                    X = target.X,
                    Y = target.Y,
                    TargetStatus = target.TargetStatus
                });
            }

            return result;
        }
    }
}
