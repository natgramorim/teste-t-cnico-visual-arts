using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Offers functions for the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject instructions;
    private CanvasFader fader;

    private void Awake()
    {
        fader = instructions.GetComponent<CanvasFader>();
    }

    /// <summary>
    /// Loads the main game scene
    /// </summary>
    public void StartGame()
    {
        LoadingManager.I.StartSceneLoading("GameMatch");
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Opens the instructions pannels
    /// </summary>
    public void OpenInstructions()
    {
        instructions.SetActive(true);
        fader.FadeIn();
    }

    /// <summary>
    /// Closes the instructions pannel
    /// </summary>
    public void CloseInstructions()
    {
        fader.FadeOut();
    }
}
