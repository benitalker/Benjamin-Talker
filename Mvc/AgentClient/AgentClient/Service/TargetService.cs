using AgentClient.Models;
using AgentClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentClient.Service
{
    public class TargetService(IServiceProvider _serviceProvider) : ITargetService
    {
        private IGeneralService GeneralService => _serviceProvider.GetRequiredService<IGeneralService>();

        public async Task<List<TargetVm>> GetAllTargetsDetails()
        {
            var targets = await GeneralService.GetAllTargetsAsync();
            return targets.Select(CreateTargetVm).ToList();
        }

        private static TargetVm CreateTargetVm(TargetModel target)
        {
            return new TargetVm
            {
                Name = target.Name,
                Image = target.Image,
                Role = target.Role,
                X = target.X,
                Y = target.Y,
                TargetStatus = target.TargetStatus
            };
        }
    }
}
