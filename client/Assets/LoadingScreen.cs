using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
public class LoadingScreen : MonoBehaviour
{

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI progressText;
    
    public void Show()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
    }

    public void Hide()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    public void SetLoadingPercent(float percent)
    {
        progressText.text = "" + percent;
    }
}


public class ShowLoadingScreenDisposable : IDisposable
{
    private readonly LoadingScreen _loadingScreen;

    public ShowLoadingScreenDisposable(LoadingScreen loadingScreen)
    {
        _loadingScreen = loadingScreen;
        _loadingScreen.Show();
    }

    public void SetLoadingPercent(float percent)
    {   
        _loadingScreen.SetLoadingPercent(percent);
    }

    public void Dispose()
    {
        _loadingScreen.Hide();
    }
}