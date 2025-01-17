using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LMBInstruction : MonoBehaviour
{
    public GameObject LMB, MouseBlank, Instructions;
    public RawImage LMBImg;
    public TextMeshProUGUI instructionsTMP;
    public float cycleTime = 1f; // Total cycle time of a full fade-in and fade-out
    public bool alreadyClicked = false;
    private Coroutine faderCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        LMBImg = LMB.GetComponent<RawImage>();
        if (LMBImg == null)
        {
            Debug.LogError("RMB RawImage missing.");
            return;
        }
        // Wait to see if Player right-clicks before instructing
        StartCoroutine("Delay");
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.0f);

        if (!alreadyClicked) // If Player has not left-clicked yet, turn on RMB instructions
        {
            Instructions.SetActive(true);
            MouseBlank.SetActive(true);
            LMB.SetActive(true);
            faderCoroutine = StartCoroutine("CycleRMBAlpha"); // Cycle RMB alpha to indicate it to Player
        }
        else // If Player has right-clicked, turn off coroutine and UI objects
        {
            RunCleanup();
        }
    }

    private IEnumerator CycleRMBAlpha()
    {
        // Split the whole alpha cycle in half, since there's just two things to alternate
        float halfTheCycle = cycleTime / 2f;

        // While the player hasn't clicked RMB for the first time yet
        while (!alreadyClicked)
        {
            // Fade in
            yield return StartCoroutine(FadeAlpha(0f, 1f, halfTheCycle));
            // Fade out
            yield return StartCoroutine(FadeAlpha(1f, 0f, halfTheCycle));
        }

        // Cleanup when alreadyClicked is true
        RunCleanup();

        /*Color fullAlpha = RMBImg.color; // The RMB's color with full alpha
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
        }*/
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            //Determine what the alpha is based on where the cycle is between beginning and end
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to); // Ensure exact end value
    }

    private void SetAlpha(float alpha)
    {
        Color currentColor = LMBImg.color;
        currentColor.a = alpha; // Specify that the alpha value should be modified, only
        LMBImg.color = currentColor; // Apply the updated alpha
    }


    private void RunCleanup()
    {
        // The proper way to stop a Coroutine with "while" logic, since it is now an instance
        if (faderCoroutine != null)
        {
            StopCoroutine(faderCoroutine);
        }

        // Make sure all parts of the RMB Instructions are "off"
        Instructions.SetActive(false);
        MouseBlank.SetActive(false);
        LMB.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //Make sure all corouties are stopped when gameobject is disabled to avoid crashes
        StopAllCoroutines();
    }
}
