using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Viddy.Services
{
    public interface ITaskService
    {
        Task CreateService();
        void RemoveService();
    }

    public class TaskService : ITaskService
    {
        public async Task CreateService()
        {
            var backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();

            var task = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == Constants.ViddyNotificationAgent);
            if (task.Value == null)
            {
                var taskBuilder = new BackgroundTaskBuilder
                {
                    Name = Constants.ViddyNotificationAgent,
                    TaskEntryPoint = Constants.ViddyNotificationAgentStartPoint
                };

                var trigger = new TimeTrigger(30, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.Register();
            }
        }

        public void RemoveService()
        {
            var task = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == Constants.ViddyNotificationAgent);
            if (task.Value == null) return;
            
            var reg = task.Value;
            reg.Unregister(true);
        }
    }
}
