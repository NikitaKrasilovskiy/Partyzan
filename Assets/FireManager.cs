using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Analytics;
//using Firebase.Extensions;

public class FireManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        //{
        //    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //});
        
        DontDestroyOnLoad(this);
    }
}
