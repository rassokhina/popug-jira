using Analytics.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

using Analytics.Core.Queries;

using MediatR;

using Microsoft.AspNetCore.Authorization;

namespace Analytics.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : Controller
    {
        private readonly IMediator mediator;

        public AnalyticsController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> Index()
        {
            var expensiveTasks = await mediator.Send(new GetMostExpensiveTaskQuery());
            var earnedByManagement = await mediator.Send(new GetEarnedByManagementQuery());
            return View(new AnalyticsModel()
            {
                EarnedByManagement = earnedByManagement,
                ExpensiveTasks = expensiveTasks
            });
        }
    }
}
