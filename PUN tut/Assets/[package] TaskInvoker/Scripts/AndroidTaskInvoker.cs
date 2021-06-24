using UnityEngine;

namespace Vuopaja
{
	//
	// Class to provide TaskInvoker functionality on Android
	//
	public class AndroidTaskInvoker : AndroidJavaProxy, ITaskInvokerPlugin
	{

		//
		// Create an instance of com.vuopaja.background.TaskInvoker
		// when creating AndroidTaskInvoker and store the reference in javaObject.
		//

		AndroidJavaObject javaObject;

		public AndroidTaskInvoker() : base("com.vuopaja.background.TaskCallback")
		{
			javaObject = new AndroidJavaObject("com.vuopaja.background.TaskInvoker", this);
		}

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
			if (delay <= 0) delay = 1;
			return javaObject.Call<int>("StartTask", delay);
		}

		public void StopTask(int taskId)
		{
			javaObject.Call("StopTask", taskId);
		}

		public void StopAllTasks()
		{
			javaObject.Call("StopAllTasks");
		}

		// 
		// com.vuopaja.background.TaskInvoker interface implementation.
		// These methods are invoked from the Android plugin.
		// 

		public void InvokeHandler(int taskId)
		{
			if (OnInvoke != null) OnInvoke(taskId);
			else Debug.Log("Task was invoked from TaskInvoker but there is no callback registered to OnInvoke event");
		}
	}
}
