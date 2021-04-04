using System;

namespace Shared
{
    public static class Constants
    {
        // queues for task 
        public static string TaskQueueTaskTracker = "TaskQueueTaskTracker";
        public static string TaskQueueAccounting = "TaskQueueAccounting";
        public static string TaskQueueAnalytics = "TaskQueueAnalytics";

        // queues for user 
        public static string UserQueueTaskTracker = "UserQueueTaskTracker";
        public static string UserQueueAccounting = "UserQueueAccounting";
        public static string UserQueueAnalytics = "UserQueueAnalytics";

        //queues for balance
        public static string BalanceQueueAccounting = "BalanceQueueAccounting";
        public static string BalanceQueueAnalytics = "BalanceQueueAnalytics";
    }
}
