using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TaskTracker.Core.Services;
using Task = TaskTracker.Web.Models.Task;

namespace TaskTracker.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly ILogger<TaskController> logger;

        // test popug id
        public Guid adminId = new Guid("4D4F5682-9AC0-E811-A2D6-00155DB24300");

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            this.taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await taskService.GetList(adminId);
            return View(tasks.Select(x => new Task
            {
                Id = x.Id,
                Description = x.Description,
                PopugId = x.PopugId,
                Status = (int)x.Status,
                Price = x.Price
            }));
        }

        public async Task<IActionResult> Finish(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            await taskService.Finish(id.Value, adminId);

            return RedirectToAction(nameof(Index));
        }

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
