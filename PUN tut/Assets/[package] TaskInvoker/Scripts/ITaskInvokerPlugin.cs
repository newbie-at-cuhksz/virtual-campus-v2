
//
// TaskInvoker interface that is used to provide unified functionality between different platforms
//

namespace Vuopaja
{
    public delegate void TaskEvent(int taskId);
    public interface ITaskInvokerPlugin
    {
        event TaskEvent OnInvoke;
        event TaskEvent OnExpire;
        int StartTask(int delay);
        void StopTask(int taskId);
        void StopAllTasks();
    }
}
