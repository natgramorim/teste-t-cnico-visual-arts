using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays audio clips as audio feedback
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioCuePlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioCue
    {
        public string tag;
        public AudioClip clip;
    }

    // Parâmetros
    [SerializeField] private AudioCue[] audioLibrary;

    // Componentes e Referências
    private AudioSource source;

    // Declarações
    private Dictionary<string, AudioCue> audioRef = new Dictionary<string, AudioCue>();

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        foreach (AudioCue cue in audioLibrary)
            audioRef.Add(cue.tag, cue);
    }

    /// <summary>
    /// Plays an audio cue by its tag
    /// </summary>
    public void PlayCue(string tag)
    {
        if (audioRef.ContainsKey(tag))
        {
            AudioCue cue = audioRef[tag];
            source.PlayOneShot(cue.clip);
        }
    }
}
