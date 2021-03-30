using System;

using Accounting.Core.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Accounting.Client.Controllers
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
    }
}
