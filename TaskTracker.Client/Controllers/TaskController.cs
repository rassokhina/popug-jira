using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskTracker.Core.Commands;
using TaskTracker.Core.Queries;

using Task = TaskTracker.Client.Models.Task;

namespace TaskTracker.Client.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly IMediator mediator;

        public TaskController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.FindFirst("sub").Value;
            var tasks = await mediator.Send(new GetTasksQuery { UserId = userId } );
            return View(tasks.Select(x => new Task
            {
                Id = x.Id,
                Description = x.Description,
                UserId = x.UserId,
                Status = (int)x.Status
            }));
        }

        public async Task<IActionResult> Finish(Guid? id, CancellationToken token)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirst("sub").Value;
            await mediator.Send(new FinishTaskCommand() {UserId = userId, TaskId = id.Value}, token);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reassign(CancellationToken token)
        {
            await mediator.Send(new ReassignTasksCommand(), token);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] CreateTaskCommand command, CancellationToken token)
        {
            if (ModelState.IsValid)
            {
                await mediator.Send(command, token);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
