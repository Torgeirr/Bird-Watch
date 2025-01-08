using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RMBInstruction : MonoBehaviour
{
    public GameObject RMB, MouseBlank, Instructions;
    public RawImage RMBImg;
    public TextMeshProUGUI instructionsTMP;
    public float cycleTime = 1f; // Total cycle time of a full fade-in and fade-out
    public bool alreadyClicked = false;
    private Coroutine faderCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        RMBImg = RMB.GetComponent<RawImage>();
        if (RMBImg == null)
        {
            Debug.LogError("RMB RawImage missing.");
            return;
        }
        // Wait to see if Player right-clicks before instructing
        StartCoroutine("Delay");
    }

    private IEnumerator Delay()
    {
        Debug.Log("Delay Started");
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Delay over");

        if (!alreadyClicked) // If Player has not right-clicked yet, turn on RMB instructions
        {
            Debug.Log("Not already clicked");
            Instructions.SetActive(true);
            MouseBlank.SetActive(true);
            RMB.SetActive(true);
            Debug.Log("All true");
            faderCoroutine = StartCoroutine("CycleRMBAlpha"); // Cycle RMB alpha to indicate it to Player
            Debug.Log("Second CRT ran");
        }
        else // If Player has right-clicked, turn off coroutine and UI objects
        {
            RunCleanup(); 
        }
    }

    private IEnumerator CycleRMBAlpha()
    {
        Color fullAlpha = RMBImg.color; // The RMB's color with full alpha
        float halfCycle = cycleTime / 2f; // Half the total cycle time, i.e. One fade-in/fade-out 
        while (true)
        {
            //Fade in
            for (float t = 0; t < halfCycle; t += Time.deltaTime)
            {
                if (!alreadyClicked) // If Player has not right-clicked, fade in indicator
                {
                    float normalizedTime = t / halfCycle; // Goes from 0 to 1
                    SetAlpha(Mathf.Lerp(0f, 1f, normalizedTime));
                    
                    //Use this if you want the instruction text to also fade in/out
                    //instructionsTMP.color = new Color(fullAlpha.r, fullAlpha.g, fullAlpha.b, Mathf.Lerp(0, 1, normalizedTime));
                    
                    yield return null;
                }
                else // If Player has right-clicked, turn the instructions off
                {
                    gameObject.SetActive(false);
                }
                
            }
            //Fade out
            for(float t = 0; t < halfCycle; t += Time.deltaTime)
            {
                if (!alreadyClicked) // If Player has not right-clicked, fade out indicator
                {
                    float normalizedTime = t / halfCycle;
                    SetAlpha(Mathf.Lerp(1f, 0f, normalizedTime));

                    //Use this if you want the instruction text to also fade in/out
                    //instructionsTMP.color = new Color(fullAlpha.r, fullAlpha.g, fullAlpha.b, Mathf.Lerp(1, 0, normalizedTime));
                    
                    yield return null;
                }
                else // If Player has right-clicked, turn the instructions off
                {
                    gameObject.SetActive(false);
                }

            }
        }
    }
    private void SetAlpha(float alpha)
    {
        Color currentColor = RMBImg.color;
        currentColor.a = alpha; // Modify only the alpha value
        RMBImg.color = currentColor; // Apply the updated color
    }

    private void RunCleanup()
    {
        if (faderCoroutine != null)
        {
            StopCoroutine(faderCoroutine);
        }

        Instructions.SetActive(false);
        MouseBlank.SetActive(false);
        RMB.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //Make sure all corouties are stopped when gameobject is disabled to avoid crashes
        StopAllCoroutines();
    }
}
