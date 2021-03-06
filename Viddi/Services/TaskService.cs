﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Viddi.Core;

namespace Viddi.Services
{
    public interface ITaskService
    {
        Task CreateService(int frequencyInMinutes = 30);
        void RemoveService();
    }

    public class TaskService : ITaskService
    {
        public async Task CreateService(int frequencyInMinutes = 30)
        {
            var backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();

            var task = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == Constants.ViddiNotificationAgent);
            if (task.Value == null)
            {
                var taskBuilder = new BackgroundTaskBuilder
                {
                    Name = Constants.ViddiNotificationAgent,
                    TaskEntryPoint = Constants.ViddiNotificationAgentStartPoint
                };

                var trigger = new TimeTrigger((uint)frequencyInMinutes, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.Register();
            }
        }

        public void RemoveService()
        {
            var task = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == Constants.ViddiNotificationAgent);
            if (task.Value == null) return;
            
            var reg = task.Value;
            reg.Unregister(true);
        }
    }
}
