using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class NoctuaGame {
    
    #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void init(string productCode);

        [DllImport("__Internal")]
        private static extern void trackEvent(string eventName, string parameters);

        [DllImport("__Internal")]
        private static extern void trackPurchaseEvent(string orderId, double purchaseAmount, string currency, string parameters);
    #endif

    private static AndroidJavaClass _unityPlayer;
    private static AndroidJavaObject _unityActivity;
    
    public static void Init(string productCode)
    {
        #if UNITY_IOS
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    init(productCode);
                }
        #elif UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _unityActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
        #endif
    }

    public static void TrackEvent(string eventName, Dictionary<string, string> parameters = null)
    {
#if UNITY_IOS
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if (parameters == null)
                    {
                        trackEvent(eventName, null);
                    }
                    else
                    {
                        string parametersJsonString = JsonConvert.SerializeObject(parameters);
                        trackEvent(eventName, parametersJsonString);
                    }
                }
#elif UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (_unityActivity != null)
                    {
                        if (parameters == null)
                        {
                            _unityActivity.CallStatic("doTrackEvent", eventName, null);
                        }
                        else
                        {
                            AndroidJavaObject bundle = new AndroidJavaObject("android.os.Bundle");
                            foreach (KeyValuePair<string, string> entry in parameters) {
                                bundle.CallStatic("putString", entry.Key, entry.Value);
                            }
                            _unityActivity.CallStatic("doTrackEvent", eventName, bundle);
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to get the current Unity activity.");
                    }
                }
#endif
    }
    public static void TrackPurchaseEvent(string orderId, double purchaseAmount, string currency, Dictionary<string, string> parameters = null)
    {
#if UNITY_IOS
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if (parameters == null)
                    {
                        trackPurchaseEvent(orderId, purchaseAmount, currency, null);
                    }
                    else
                    {
                        string parametersJsonString = JsonConvert.SerializeObject(parameters);
                        trackPurchaseEvent(orderId, purchaseAmount, currency, parametersJsonString);
                    }
                }
#elif UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (_unityActivity != null)
                    {
                        if (parameters == null)
                        {
                            _unityActivity.CallStatic("doTrackPurchaseEvent", orderId, purchaseAmount, currency, null);
                        }
                        else
                        {
                            AndroidJavaObject bundle = new AndroidJavaObject("android.os.Bundle");
                            foreach (KeyValuePair<string, string> entry in parameters) {
                                bundle.CallStatic("putString", entry.Key, entry.Value);
                            }
                            _unityActivity.CallStatic("doTrackPurchaseEvent", orderId, purchaseAmount, currency, bundle);
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to get the current Unity activity.");
                    }
                }
#endif
    }
}
