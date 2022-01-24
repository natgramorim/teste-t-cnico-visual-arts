using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Performs async scene loading and operates the fader screen
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image fadeScreen;
    [SerializeField] bool fadeOutOnStart;
    private CanvasFader fader;
    private string loadingScene;
    public static LoadingManager I;
    private AudioCuePlayer cuePlayer;

    private IEnumerator LoadRoutine(string scene)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!loading.isDone)
        {
            yield return null;
        }

        // Underused
    }

    private void Awake()
    {
        I = this;
        fader = GetComponent<CanvasFader>();
        cuePlayer = GetComponent<AudioCuePlayer>();
    }

    private void Start()
    {
        if (fadeOutOnStart)
        {
            fader.FadeOut();
            cuePlayer.PlayCue("Fade Out");
        }
        else
        {
            fader.SetT(0);
            fadeScreen.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Loads a scene from its name
    /// </summary>
    /// <param name="scene"></param>
    public void StartSceneLoading(string scene)
    {
        loadingScene = scene;
        fadeScreen.gameObject.SetActive(true);
        fader.FadeIn();
        cuePlayer.PlayCue("Fade In");
    }

    public void LoadScene()
    {
        StartCoroutine(LoadRoutine(loadingScene));
    }
}
