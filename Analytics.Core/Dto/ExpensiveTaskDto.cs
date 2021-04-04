using System;

namespace Analytics.Core.Dto
{
    public class ExpensiveTaskDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public PeriodDto Period { get; set; }

        public string PeriodString { get; set; }
    }
}
