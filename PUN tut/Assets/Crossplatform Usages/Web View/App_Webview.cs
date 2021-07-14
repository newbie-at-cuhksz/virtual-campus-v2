using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
public class App_Webview : MonoBehaviour
{
    public static App_Webview instance;
    WebView CurrentWebView;
    private void OnEnable()
    {
        // register for events
        WebView.OnShow += OnWebViewShow;
        WebView.OnHide += OnWebViewHide;
        WebView.OnLoadStart += OnWebViewLoadStart;
        WebView.OnLoadFinish += OnWebViewLoadFinish;
    }

    private void OnDisable()
    {
        // register for events
        WebView.OnShow -= OnWebViewShow;
        WebView.OnHide -= OnWebViewHide;
        WebView.OnLoadStart -= OnWebViewLoadStart;
        WebView.OnLoadFinish -= OnWebViewLoadFinish;
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    //APP_Webview.instance.NewWebView(webpageURL);
    public void NewWebView(string URL)
    {
        Debug.Log("a new web view should be created");
        CurrentWebView = WebView.CreateInstance();
        CurrentWebView.SetFullScreen();
        CurrentWebView.LoadURL(URLString.URLWithPath(URL));
        CurrentWebView.Show();
    }
    //APP_Webview.instance.HideCurrentWebview();
    public void HideCurrentWebview()
    {
        CurrentWebView.Hide();
    }
    private void OnWebViewShow(WebView result)
    {
        Debug.Log("Webview is being displayed : " + result);
    }
    private void OnWebViewHide(WebView result)
    {
        Debug.Log("Webview is hidden : " + result);

    }
    private void OnWebViewLoadStart(WebView result)
    {
        Debug.Log("Webview is being displayed : " + result);
    }
    private void OnWebViewLoadFinish(WebView result, Error error)
    {
        Debug.Log("Webview is being displayed : " + result);
    }

}
