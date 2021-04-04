using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TaskTracker.Core.Services;
using Task = TaskTracker.Client.Models.Task;

namespace TaskTracker.Client.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly ILogger<TaskController> logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            this.taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.FindFirst("sub").Value;
            var tasks = await taskService.GetList(userId);
            return View(tasks.Select(x => new Task
            {
                Id = x.Id,
                Description = x.Description,
                UserId = x.UserId,
                Status = (int)x.Status
            }));
        }

        public async Task<IActionResult> Finish(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirst("sub").Value;
            await taskService.Finish(id.Value, userId);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reassign()
        {
            await taskService.Reassign();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] Task task)
        {
            if (ModelState.IsValid)
            {
                await taskService.Create(new Core.Dto.TaskCreateDto { Description = task.Description });
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
