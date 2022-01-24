using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates visual and audio feedback when the player clicks with a mouse
/// </summary>
[RequireComponent(typeof(AudioCuePlayer))]
public class ClickFeedback : MonoBehaviour
{
    [SerializeField] private GameObject visualFeedback;
    [SerializeField] private string audioCue;
    private AudioCuePlayer audioPlayer;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioCuePlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject feedback = Instantiate(visualFeedback);
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 3f;
            feedback.transform.position = pos;
            audioPlayer.PlayCue(audioCue);
        }
    }
}
