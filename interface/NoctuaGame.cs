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

    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject currentActivity;
    private static AndroidJavaClass noctua;
    
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
                    unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    noctua = new AndroidJavaObject("com.noctua.QuickSDKMethod");
                    noctua.CallStatic("Init", activity, productCode);
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
                    if (noctua != null)
                    {
                        if (parameters == null)
                        {
                            noctua.CallStatic("trackEvent", eventName, null);
                        }
                        else
                        {
                            AndroidJavaObject bundle = new AndroidJavaObject("android.os.Bundle");
                            foreach (KeyValuePair<string, string> entry in parameters) {
                                bundle.CallStatic("putString", entry.Key, entry.Value);
                            }
                            noctua.CallStatic("trackEvent", eventName, bundle);
                        }
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
                    if (noctua != null)
                    {
                        if (parameters == null)
                        {
                            noctua.CallStatic("trackPurchaseEvent", orderId, purchaseAmount, currency, null);
                        }
                        else
                        {
                            AndroidJavaObject bundle = new AndroidJavaObject("android.os.Bundle");
                            foreach (KeyValuePair<string, string> entry in parameters) {
                                bundle.CallStatic("putString", entry.Key, entry.Value);
                            }
                            noctua.CallStatic("trackPurchaseEvent", orderId, purchaseAmount, currency, bundle);
                        }
                    }
                }
#endif
    }
}
