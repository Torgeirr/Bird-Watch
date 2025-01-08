using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLanding : MonoBehaviour
{

    public GameObject landing, landing1, landing2, landing3;
    public bool treeFull = false, landingTaken = false, landing1Taken = false, landing2Taken = false, landing3Taken = false;

    public void CheckTreeFull()
    {
        treeFull = landingTaken && landing1Taken && landing2Taken && landing3Taken;
    }
}
