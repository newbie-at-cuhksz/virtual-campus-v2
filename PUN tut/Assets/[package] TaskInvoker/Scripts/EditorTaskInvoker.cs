using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vuopaja
{
    //
    // Class to provide TaskInvoker functionality for Unity Editor play mode testing
    //
    public class EditorTaskInvoker : MonoBehaviour, ITaskInvokerPlugin
    {
        List<EditorTask> runningTasks = new List<EditorTask>();

        //
        // ITaskInvoker events
        //

        public event TaskEvent OnInvoke;
        public event TaskEvent OnExpire {add{}remove{}}

        //
        // Methods to start and stop tasks.
        //

        public int StartTask(int delay)
        {
            int index = getFreeIndex();
            var routine = StartCoroutine(invoke(index, delay));
            runningTasks.Add(new EditorTask(routine, index));
            return index;
        }

        public void StopTask(int taskId)
        {
            var task = runningTasks.FirstOrDefault(x => x.TaskID == taskId);
            if (task != null){
                StopCoroutine(task.Routine);
                runningTasks.Remove(task);
            } else {
                Debug.LogError("No task to stop with id " + taskId);
            }
        }

        public void StopAllTasks()
        {
            foreach(var task in runningTasks) StopCoroutine(task.Routine);
            runningTasks.Clear();
        }

        //
        // Editor specific implementation to invoke TaskInvoker events similar to native plugins
        //

        IEnumerator invoke(int index, int delay)
        {
            if (delay <= 0) delay = 1;
            yield return new WaitForSeconds(delay * 0.001f);
            var task = runningTasks.FirstOrDefault(x => x.TaskID == index);
            if (task != null)
            {
                if (OnInvoke != null) OnInvoke(index);
                task.Routine = StartCoroutine(invoke(index, delay));
            }
        }

        int getFreeIndex()
        {
            if (runningTasks.Count == 0) return 0;
            int indexOfHighest = 0;
            for(int i = 0; i < runningTasks.Count; i++){
                if (runningTasks[i].TaskID > runningTasks[indexOfHighest].TaskID) indexOfHighest = i;
            }
            return runningTasks[indexOfHighest].TaskID + 1;
        }
    }

    class EditorTask
    {
        public int TaskID;
        public Coroutine Routine;

        public EditorTask(Coroutine routine, int id)
        {
            Routine = routine;
            TaskID = id;
        }
    }
}
