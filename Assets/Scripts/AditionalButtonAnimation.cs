using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AditionalButtonAnimation : MonoBehaviour
{
    public void PopPulse()
    {
        Pulse pulse = GetComponent<Pulse>();
        pulse.OneOff(new Vector3(1, 1, 1), new Vector3(1.25f, 1.25f, 1), 0.3f, null);
    }
}
