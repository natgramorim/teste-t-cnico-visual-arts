using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Offers functions for the ingame menu
/// </summary>
public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    private CanvasFader fader;

    private void Awake()
    {
        fader = menuScreen.GetComponent<CanvasFader>();
    }

    /// <summary>
    /// Opens de menu through a fade
    /// </summary>
    public void OpenMenu()
    {
        menuScreen.SetActive(true);
        fader.FadeIn();
    }

    /// <summary>
    /// Closes the menu through a fade
    /// </summary>
    public void CloseMenu()
    {
        fader.FadeOut();
    }

    /// <summary>
    /// Returns to the main menu screen
    /// </summary>
    public void ReturnToMainMenu()
    {
        GameManager.I.SaveGameData();
        LoadingManager.I.StartSceneLoading("MainMenu");
    }

    public void RestartMatch()
    {
        VictoryScreen.I.CloseScreen();
    }
}
