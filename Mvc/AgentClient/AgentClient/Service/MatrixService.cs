using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AgentClient.Models;
using AgentClient.ViewModel;

namespace AgentClient.Service
{
    public class MatrixService : IMatrixService
    {
        private IGeneralService generalService => serviceProvider.GetRequiredService<IGeneralService>();
        private readonly IServiceProvider serviceProvider;

        public MatrixService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<MatrixVm> InitMatrix()
        {
            try
            {
                var agents = await generalService.GetAllAgentsAsync();
                var targets = await generalService.GetAllTargetsAsync();

                if (!agents.Any() && !targets.Any())
                {
                    return new MatrixVm(1, 1);
                }
                // Determine the matrix size
                int maxX = Math.Max(agents.Max(a => a.X), targets.Max(t => t.X)) + 1;
                int maxY = Math.Max(agents.Max(a => a.Y), targets.Max(t => t.Y)) + 1;

                var model = new MatrixVm(maxX, maxY);

                // Place agents in the matrix
                foreach (var agent in agents)
                {
                    if (agent.X >= 0 && agent.X < maxX && agent.Y >= 0 && agent.Y < maxY)
                    {
                        model.Matrix[agent.X, agent.Y] += $"{agent.Nickname}";
                    }
                }

                // Place targets in the matrix
                foreach (var target in targets)
                {
                    if (target.X >= 0 && target.X < maxX && target.Y >= 0 && target.Y < maxY)
                    {
                        model.Matrix[target.X, target.Y] += $"{target.Name}";
                    }
                }

                return model;
            }
            catch(Exception ex)  
            { 
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
