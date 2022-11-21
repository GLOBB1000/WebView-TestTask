using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//using Firebase.RemoteConfig;
using Sirenix.OdinInspector;
using Firebase.Extensions;
using Unity.VisualScripting;

public class MyFireBaseRemoteConfig : MonoBehaviour 
{
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.Available;

	public bool CanLoadUrl { get; private set; }
	

	[SerializeField]
	private ApplicationController applicationController;
	// Use this for initialization
	void Awake() 
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(async task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available) {
				CanLoadUrl = true;
                //InitializeFirebase();
            } else {
				Debug.LogError(
					"Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});
	}

	private async Task LoadUrls()
	{
        applicationController.savedUrl = PlayerPrefs.GetString("URL");

		if (string.IsNullOrEmpty(applicationController.savedUrl))
			await applicationController.LoadUrl();
		else
			applicationController.StartWebView(applicationController.savedUrl);
    }

	void InitializeFirebase() 
	{
		System.Collections.Generic.Dictionary<string, object> defaults =
			new System.Collections.Generic.Dictionary<string, object>();

		//remoteConfig = FirebaseRemoteConfig.DefaultInstance;

        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:
        defaults.Add("config_test_string", "default local string");
		defaults.Add("config_test_int", 1);
		defaults.Add("config_test_float", 1.0);
		defaults.Add("config_test_bool", false);

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
		Debug.Log("Remote config ready!");
	}


    [Button]
    public void FetchFireBase() 
	{
		FetchDataAsync();
	}

	[Button]
	public void ShowData() 
	{
		Debug.Log("URL: " +
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url").StringValue);
    }

	[Button]
	public void DisplayAllKeys()
	{
        Debug.Log("Current Keys:");
        System.Collections.Generic.IEnumerable<string> keys =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys;
        foreach (string key in keys)
        {
            Debug.Log("    " + key);
        }
	}

        // Start a fetch request.
        public Task FetchDataAsync() {
		Debug.Log("Fetching data...");
		// FetchAsync only fetches new data if the current data is older than the provided
		// timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
		// By default the timespan is 12 hours, and for production apps, this is a good
		// number.  For this example though, it's set to a timespan of zero, so that
		// changes in the console will always show up immediately.
		Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
			TimeSpan.Zero);
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}

	void FetchComplete(Task fetchTask) 
	{

		if (fetchTask.IsCanceled) {
			Debug.Log("Fetch canceled.");
		} else if (fetchTask.IsFaulted) {
			Debug.Log("Fetch encountered an error.");
		} else if (fetchTask.IsCompleted) {
			Debug.Log("Fetch completed successfully!");
		}

		var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
		switch (info.LastFetchStatus) {
			case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => 
				{
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                         info.FetchTime));
                });
                break;
			case Firebase.RemoteConfig.LastFetchStatus.Failure:
				switch (info.LastFetchFailureReason) {
					case Firebase.RemoteConfig.FetchFailureReason.Error:
						Debug.Log("Fetch failed for unknown reason");
						break;
					case Firebase.RemoteConfig.FetchFailureReason.Throttled:
						Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
						break;
				}
				break;
			case Firebase.RemoteConfig.LastFetchStatus.Pending:
				Debug.Log("Latest Fetch call still pending.");
				break;
		}
	}
}