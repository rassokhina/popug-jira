using System;
using System.Threading.Tasks;

using Accounting.Client.Models;
using Accounting.Core.Queries;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Client.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class WalletController : Controller
    {

        private readonly IMediator mediator;

        public WalletController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.FindFirst("sub").Value;
            var audits = await mediator.Send(new GetWalletAuditQuery { UserId = userId });
            var earnedByManagement = await mediator.Send(new GetEarnedByManagementQuery());
            return View(new WalletModel()
            {
                EarnedByManagement = earnedByManagement, 
                WalletAudits = audits
            });
        }
    }
}
