using System;
using System.Runtime.InteropServices;
using AOT;

namespace Vuopaja
{
    //
    // Class to provide TaskInvoker functionality on iOS
    //
    public class IOSTaskInvoker : ITaskInvokerPlugin
    {

        //
        // Create static reference to the class instance
        // as a managed instance method cannot be called from native code.
        //

        static IOSTaskInvoker instance;

        public IOSTaskInvoker()
        {
            if (instance != null) throw new InvalidOperationException("Creating a second instance of IOSTaskInvoker. This is not supported.");
            instance = this;
        }

        //
		// ITaskInvoker events
		//
        
        public event TaskEvent OnInvoke;
        public event TaskEvent OnExpire;

        //
        // Methods to start and stop tasks.
        //

        public int StartTask(int delay)
        {
            #if UNITY_IOS
                return startTask(delay, invokeHandler, expireHandler);
            #else
                return 0;
            #endif
        }

        public void StopTask(int taskId)
        {
            #if UNITY_IOS
                stopTask(taskId);
            #endif
        }

        public void StopAllTasks()
        {
            #if UNITY_IOS
                stopAllTasks();
            #endif
        }
        
        //
        // Methods and callback implemented in the iOS plugin.
        // 

        delegate void UnityIntEvent(int taskId);

        [MonoPInvokeCallback(typeof(UnityIntEvent))]
        static void invokeHandler(int taskId)
        {
            if (instance != null && instance.OnInvoke != null) instance.OnInvoke(taskId);
        }

        [MonoPInvokeCallback(typeof(UnityIntEvent))]
        static void expireHandler(int taskId)
        {
            if (instance != null && instance.OnExpire != null) instance.OnExpire(taskId);
        }

        #if UNITY_IOS
        [DllImport ("__Internal")]
        static extern int startTask (int delay, UnityIntEvent invokedHandler, UnityIntEvent expiredHandler);
        [DllImport ("__Internal")]
        static extern void stopTask (int taskId);
        [DllImport ("__Internal")]
        static extern void stopAllTasks ();
        #endif
    }
}
