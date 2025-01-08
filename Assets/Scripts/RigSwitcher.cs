using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RigSwitcher : MonoBehaviour
{
    public GameObject pcRig;
    public GameObject vrRig;

    void Start()
    {
        //If the VR XR is turned on, turn on VR controls
        if (XRSettings.isDeviceActive)
        {
            pcRig.SetActive(false);
            vrRig.SetActive(true);
        }
        //else, turn on PC controls
        else
        {
            pcRig.SetActive(true);
            vrRig.SetActive(false);
        }
    }
}
