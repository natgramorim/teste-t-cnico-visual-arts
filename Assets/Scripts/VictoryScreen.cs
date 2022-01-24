using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Operates the victory screen
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;
    private CanvasFader fader;

    [SerializeField] private PlayableAsset victorySequence;
    private PlayableDirector director;

    public static VictoryScreen I;

    private void Awake()
    {
        fader = victoryScreen.GetComponent<CanvasFader>();
        director = GetComponent<PlayableDirector>();
        I = this;
    }

    /// <summary>
    /// Fades in the victory screen
    /// </summary>
    public void OpenScreen()
    {
        victoryScreen.SetActive(true);
        fader.FadeIn();
    }

    /// <summary>
    /// Fades out the victory screen
    /// </summary>
    public void CloseScreen()
    {
        fader.FadeOut();
    }

    /// <summary>
    /// Plays the timeline with the victory sequence
    /// </summary>
    public void PlayVictorySequence()
    {
        director.Play(victorySequence);
    }
}
