using UnityEngine;

namespace Vuopaja
{
    //
    // Example of TaskInvoker usage.
    // Runs a counter that invokes once per second while the app is in background and displays the elapsed background time in the UI.
    // 
    public class TaskStarterExample : MonoBehaviour
    {
        public UnityEngine.UI.Text ResultText;

        int elapsedTime;
        int currentTaskID = -1;

        void OnApplicationPause(bool paused)
        {
            // Start the counter on pause
            if (paused)
            {
                // Start a task that invokes once every second (1000 milliseconds)
                currentTaskID = TaskInvoker.StartTask(1000, onInvoke, onExpire);
            }
            else if (currentTaskID != -1)
            {
                // Stop the running task when entering foreground
                TaskInvoker.StopTask(currentTaskID);
            }
        }

        // Called once per second to count background time
        void onInvoke(int taskId)
        {
            elapsedTime++;
            Debug.Log("Task " + taskId + " invoked");
            updateExampleUI(false);
        }

        // Task was expired by OS and will stop running
        void onExpire(int taskId)
        {
            Debug.Log("Task " + taskId + " expired");
            updateExampleUI(true);
        }

        // Updates the example UI to display counter value and to show if the current task was expired
        void updateExampleUI(bool expired)
        {
            if (expired) {
                ResultText.text = string.Format("App has run {0} seconds in the background. The last task expired while in background", elapsedTime);
            } else {
                ResultText.text = string.Format("App has run {0} seconds in the background", elapsedTime);
            }
        }
    }
}
