using System;
using System.Collections.Generic;

using Analytics.Core.Dto;

namespace Analytics.Client.Models
{
    public class AnalyticsModel
    {
        public int EarnedByManagement { get; set; }

        public IEnumerable<ExpensiveTaskDto> ExpensiveTasks { get; set; }
    }
}
