#if UNITY_ANDROID
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins.Android;

namespace VoxelBusters.EssentialKit.MediaServicesCore.Android
{
    public class NativeMediaAssetSelectionListener : AndroidJavaProxy
    {
        #region Delegates

        public delegate void OnSuccessDelegate(NativeAsset asset);
        public delegate void OnFailureDelegate(string error);

        #endregion

        #region Public callbacks

        public OnSuccessDelegate  onSuccessCallback;
        public OnFailureDelegate  onFailureCallback;

        #endregion


        #region Constructors

        public NativeMediaAssetSelectionListener() : base("com.voxelbusters.android.essentialkit.features.mediaservices.IMediaServices$IMediaAssetSelectionListener")
        {
        }

        #endregion


        #region Public methods
#if NATIVE_PLUGINS_DEBUG_ENABLED
        public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            DebugLogger.Log("**************************************************");
            DebugLogger.Log("[Generic Invoke : " +  methodName + "]" + " Args Length : " + (javaArgs != null ? javaArgs.Length : 0));
            if(javaArgs != null)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                foreach(AndroidJavaObject each in javaArgs)
                {
                    if(each != null)
                    {
                        builder.Append(string.Format("[Type : {0} Value : {1}]", each.Call<AndroidJavaObject>("getClass").Call<string>("getName"), each.Call<string>("toString")));
                        builder.Append("\n");
                    }
                    else
                    {
                        builder.Append("[Value : null]");
                        builder.Append("\n");
                    }
                }

                DebugLogger.Log(builder.ToString());
            }
            DebugLogger.Log("-----------------------------------------------------");
            return base.Invoke(methodName, javaArgs);
        }
#endif

        public void onSuccess(AndroidJavaObject asset)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Proxy : Callback] : " + "onSuccess"  + " " + "[" + "asset" + " : " + asset +"]");
#endif
            if(onSuccessCallback != null)
            {
                onSuccessCallback(new NativeAsset(asset));
            }
        }
        public void onFailure(string error)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Proxy : Callback] : " + "onFailure"  + " " + "[" + "error" + " : " + error +"]");
#endif
            if(onFailureCallback != null)
            {
                onFailureCallback(error);
            }
        }

        #endregion
    }
}
#endif