using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Vuopaja
{
    //
    // Utility class to send UnityWebRequests with the TaskInvoker.
    // Any request sent with the wrapper will keep running in the background
    // if the user/OS sends the app to the background.
    //
    // NOTE: This is only an utility class and the TaskInvoker plugin works without it,
    // look at the TaskStarterExample.cs for functionality without this class.
    // 
    public class WebRequestWrapper
    {
        public Action<WrappedRequest> Completed;
        public Action<WrappedRequest, string> Failed;

        List<WrappedRequest> runningRequests = new List<WrappedRequest>();


        // Sends a WebRequest given as a parameter and checks its status every millisecondsPerUpdate
        public void Send(UnityWebRequest request, int millisecondsPerUpdate)
        {
            // Start a TaskInvoker task and send the UnityWebRequest.
            int taskID = TaskInvoker.StartTask(millisecondsPerUpdate, onInvoke, onExpire);
            runningRequests.Add(new WrappedRequest(request, taskID));
            request.SendWebRequest();
        }

        void onInvoke(int id)
        {
            // Check the state of the request running with this id.
            var wrappedRequest = runningRequests.FirstOrDefault(x => x.ID == id);
            if (wrappedRequest != null && wrappedRequest.Request.isDone)
            {
                if (wrappedRequest.Request.isNetworkError || !string.IsNullOrEmpty(wrappedRequest.Request.error))
                {
                    // Invoke Failed event in case of any error with the request.
                    if(Failed != null) Failed(wrappedRequest, wrappedRequest.Request.error);
                } else {
                    // Invoke Completed event once the request is done.
                    if(Completed != null) Completed(wrappedRequest);
                }

                // Stop the TaskInvoker task once the request is done.
                TaskInvoker.StopTask(id);
                runningRequests.Remove(wrappedRequest);
            }
        }

        void onExpire(int id)
        {
            var wrappedRequest = runningRequests.FirstOrDefault(x => x.ID == id);
            if (wrappedRequest != null)
            {
                // Invoke Failed event if the task expires
                if (Failed != null) Failed(wrappedRequest, "Task running this WebRequest expired.");
            }
        }
    }

    //
    // Class to represent a UnityWebRequest sent with the WebRequestWrapper
    //
    public class WrappedRequest
    {
        public UnityWebRequest Request;
        public int ID;

        public WrappedRequest(UnityWebRequest request, int id)
        {
            Request = request;
            ID = id;
        }
    }
}
