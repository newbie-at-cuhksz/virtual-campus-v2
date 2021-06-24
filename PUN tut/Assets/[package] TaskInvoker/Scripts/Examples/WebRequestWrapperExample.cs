using UnityEngine;
using UnityEngine.Networking;

namespace Vuopaja
{
    //
    // Example of using WebRequestWrapper with TaskInvoker.
    // Purpose of this example is to show how you can start all of your WebRequests
    // with the TaskInvoker so that whenever the user/OS sends the app to the background
    // the WebRequests would run until they are complete or the app is killed.
    // 
    public class WebRequestWrapperExample : MonoBehaviour
    {
        public UnityEngine.UI.Text ResultTextContainer;
        public UnityEngine.UI.InputField URLField;

        WebRequestWrapper webRequestWrapper;
        int sent, completed, failed;

        void Start()
        {
            // Create a new wrapper object and register for callbacks
            webRequestWrapper = new WebRequestWrapper();
            webRequestWrapper.Completed += onCompleted;
            webRequestWrapper.Failed += onFailed;
        }

        void OnDestroy()
        {
            // Unregister callbacks on destroy
            if (webRequestWrapper != null){
                webRequestWrapper.Completed -= onCompleted;
                webRequestWrapper.Failed -= onFailed;
            }
        }

        public void SendNewRequest()
        {
            // This method is called from Unity UI Button.
            // Here we create a WebRequest like you normally would
            var request = UnityWebRequest.Get(URLField.text);
            
            // Then we send the created request using the wrapper.
            // The wrapper starts a TaskInvoker task so that if the user/OS sends the app
            // to the background during this WebRequest it will continue running in background.
            // Takes milliseconds as a parameter that defines the delay between each invoke.
            webRequestWrapper.Send(request, 100);

            // Update example UI
            sent++;
            updateUI();
        }

        void onCompleted(WrappedRequest wrappedRequest)
        {
            // Handle completed WebRequest
            var request = wrappedRequest.Request;
            Debug.Log("Download completed with size: " + request.downloadHandler.data.Length);

            // Update example UI
            completed++;
            updateUI();
        }

        void onFailed(WrappedRequest wrappedRequest, string reason)
        {
            // Handle failure
            Debug.LogError(reason);
            
            // Update example UI
            failed++;
            updateUI();
        }

        void updateUI()
        {
            // Display example info about sent WebRequests
            ResultTextContainer.text = string.Format("Requests sent: {0}\nRequests completed: {1}\nRequests failed: {2}", sent, completed, failed);
        }
    }
}
