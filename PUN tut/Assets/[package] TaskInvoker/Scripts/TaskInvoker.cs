using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Vuopaja
{
    //
    // The main TaskIvoker class.
    // Contains all methods for starting and stopping background tasks.
    //
    public static class TaskInvoker
    {
        static ITaskInvokerPlugin taskInvokerPlugin;
        static List<Task> runningTasks = new List<Task>();

       
        // Starts a new background task and returns its task ID
        public static int StartTask(int delay, TaskEvent onInvoke, TaskEvent onExpire = null)
        {
            if (taskInvokerPlugin == null) createTaskInvoker();
            int id = taskInvokerPlugin.StartTask(delay);
            runningTasks.Add(new Task(id, onInvoke, onExpire));
            return id;
        }

        // Stops a task with the given ID
        public static void StopTask(int taskID)
        {
            var task = runningTasks.FirstOrDefault(x => x.TaskID == taskID);
            if (task != null) 
            {
                taskInvokerPlugin.StopTask(taskID);
                runningTasks.Remove(task);
            }
        }

        // Stops all running tasks
        public static void StopAllTasks()
        {
            if (taskInvokerPlugin != null) taskInvokerPlugin.StopAllTasks();
            runningTasks.Clear();
        }

        // Creates an instance of TaskInvoker depending on the platform
        static void createTaskInvoker()
        {
            #if UNITY_EDITOR
                taskInvokerPlugin = new GameObject("EditorTaskInvoker").AddComponent<EditorTaskInvoker>();
            #elif UNITY_ANDROID
                taskInvokerPlugin = new AndroidTaskInvoker();
            #elif UNITY_IOS
                taskInvokerPlugin = new IOSTaskInvoker();
            #else
                Debug.Log("TaskInvoker is not supported on this platform. Use Application.runInBackground to run in background.");
                return;
            #endif

            // Register to plugin events
            taskInvokerPlugin.OnInvoke += onInvoked;
            taskInvokerPlugin.OnExpire += onExpired;
        }

        // Propagates TaskInvoker invoke event to Task object's callback
        static void onInvoked(int taskID)
        {
            var task = runningTasks.FirstOrDefault(x => x.TaskID == taskID);
            if (task != null) task.OnInvoke(taskID);
        }

        // Propagates TaskInvoker expired event to Task object's callback
        static void onExpired(int taskID)
        {
            var task = runningTasks.FirstOrDefault(x => x.TaskID == taskID);
            if (task != null && task.OnExpire != null) task.OnExpire(taskID);
        }

        //
        // Class to represent a Task that is run by the TaskInvoker
        //
        class Task {
            public int TaskID;
            public TaskEvent OnInvoke;
            public TaskEvent OnExpire;

            public Task(int taskID, TaskEvent onInvoke, TaskEvent onExpire)
            {
                TaskID = taskID;
                OnInvoke = onInvoke;
                OnExpire = onExpire;
            }
        }
    }
}
