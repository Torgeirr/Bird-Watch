using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

using UnityEngine.UIElements;

public class Binoculars : MonoBehaviour
{
    public Camera mainCamera, BinocularCamera;
    public float zoomDistance = 10f;
    public float restingDistance = 50f;
    public float zoomedFOV = 15f;
    public float normalFOV = 60f;
    public float binoMoveSpeed = 5f, zoomSpeed = 3f;
    public float[] ZoomLevels = { 8f, 15f, 22.5f }; //FOV values to serve as scrollable zoom levels
    public int curZoom = 1; // Default to middle zoom level
    public GameObject[] ZoomMarkers = new GameObject[3];
    public float curMarkerSetting = 0;
    public GameObject curMarkerPOS;
    public GameObject zoomMarker, MarkerPOS1, MarkerPOS2, MarkerPOS3, markerFrom, markerTo;

    public GameObject BinocularDisplay, BinosUpPOS, BinosDownPOS;
    private Coroutine infoBoxCoroutine, ScannedIndicator;

    public float detectionRange = 50f; //Range of bird scanner raycast
    public LayerMask birdLayer;
    public GameObject specimenInfoBox, speciesInfoBox, blurbInfoBox; // Bino UI panel that displays bird info
    public GameObject scannedIndicator; // Bino UI indicator that shows when bird scanned
    public GameObject Logbook; // Logbook gameobject reference
    public Logbook logbookScript;
    public GameObject RMBInstructions;
    public TMPro.TextMeshProUGUI birdStatText, birdSpeciesText, birdInfoText;

    public bool isZoomed = false, isRMBClicked = false, newRMBClick = false, tester = false, firstClicked = false, scrolled = false;

    private void Start()
    {
        //Initialize the zoom marker UI
        curMarkerPOS = ZoomMarkers[1];
        markerFrom = curMarkerPOS;
        markerTo = curMarkerPOS;

        //Initialize referenced Logbook.cs script
        logbookScript = Logbook.GetComponent<Logbook>();
    }
    void FixedUpdate()
    {
        // Only let player operate Binoculars if Logbook is not being used
        if (!logbookScript.isRaised)
        {
            newRMBClick = false;
            scrolled = false;

            //If the player is clicking RMB while RMB isn't considered clicked
            if (!isRMBClicked && Input.GetMouseButton(1))
            {
                isRMBClicked = true;
                newRMBClick = true;
                //Debug.Log("newRMBClick turned to true");

                // If this is the first right-click, tell RMBInstruction to turn off and not instruct the player
                if (!firstClicked)
                {
                    RMBInstructions.GetComponent<RMBInstruction>().alreadyClicked = true;
                }

            }
            //If RMB is already considered clicked, but the player is not clicking it
            else if (isRMBClicked && !Input.GetMouseButton(1))
            {
                isRMBClicked = false;
            }

            if (newRMBClick)
            {
                isZoomed = !isZoomed;
            }

            if (isZoomed)
            {
                if (Input.mouseScrollDelta.y < 0) // If scrolled up
                {
                    SetZoom(1);
                }
                else if (Input.mouseScrollDelta.y > 0) // If scrolled down
                {
                    SetZoom(-1);
                }
            }
            markerFrom = curMarkerPOS;
            curMarkerPOS = ZoomMarkers[curZoom];
            markerTo = curMarkerPOS;

            float newFOV = isZoomed ? ZoomLevels[curZoom] : normalFOV;
            BinocularCamera.fieldOfView = Mathf.Lerp(BinocularCamera.fieldOfView, newFOV, Time.deltaTime * zoomSpeed);

            if (markerFrom != null && markerTo != null)
            {
                zoomMarker.transform.position = Vector3.Lerp(markerFrom.transform.position, markerTo.transform.position, Time.deltaTime * zoomSpeed);
            }
            else
            {
                Debug.LogWarning("MarkerFrom or MarkerTo not assigned");
            }

            // Update the transform of the binocular object itself so it stays where it should be (whether player is using it or has it down)
            BinocularDisplay.transform.position = Vector3.Lerp(BinocularDisplay.transform.position, isZoomed ? BinosUpPOS.transform.position : BinosDownPOS.transform.position, Time.deltaTime * binoMoveSpeed);

            if (Input.GetMouseButton(0)) //Changed from GetMouseButtonDown() to make scanning more reliable
            {
                Ray ray = new(BinocularCamera.transform.position, BinocularCamera.transform.forward);
                //Debug.Log("Drawing Ray");
                Debug.DrawRay(BinocularCamera.transform.position, BinocularCamera.transform.forward, Color.green);
                // If raycast hits something in the "Bird" Layer
                if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
                {
                    if (IsInBirdLayer(hit.collider.gameObject, birdLayer))
                    {
                        BirdInfo bird = hit.collider.GetComponent<BirdInfo>();
                        if (bird != null)
                        {
                            Debug.Log("Scanned a bird");
                            ShowBirdInfo(bird);
                            LogBirdInfo(bird);
                        }
                        else
                        {
                            Debug.Log("Bird = null");
                        }
                        if (!speciesInfoBox.activeSelf)
                        {
                            StartInfoBoxCoroutine();
                        }
                        else if (speciesInfoBox.activeSelf)
                        {
                            RestartInfoBoxCoroutine();
                        }
                        if (!scannedIndicator.activeSelf)
                        {
                            StartScannedIndicator();
                        }
                        else if (scannedIndicator.activeSelf)
                        {
                            RestartScannedIndicator();
                        }
                    }

                }
            }
        }

        
    }

    public void SetZoom(int scrollDirection)
    {
        
        curZoom = Mathf.Clamp(curZoom + scrollDirection, 0, ZoomLevels.Length - 1);
        BinocularCamera.fieldOfView = ZoomLevels[curZoom];
        curMarkerSetting = Mathf.Clamp(curMarkerSetting + scrollDirection, 0, ZoomMarkers.Length - 1);
        Debug.Log($"Zoom set to {ZoomLevels[curZoom]}");
    }

    public bool IsInBirdLayer(GameObject hitObj, LayerMask mask)
    {
        return ((1 << hitObj.layer) & mask.value) != 0;
    }
       

    private void StartInfoBoxCoroutine()
    {
        if (infoBoxCoroutine != null)
        {
            StopCoroutine(infoBoxCoroutine);
        }

        infoBoxCoroutine = StartCoroutine(ActivateInfoBox());
    }

    private void RestartInfoBoxCoroutine()
    {
        if (infoBoxCoroutine != null)
        {
            StopCoroutine(infoBoxCoroutine);
        }

        infoBoxCoroutine = StartCoroutine(ActivateInfoBox());
    }

    public IEnumerator ActivateInfoBox()
    {
        specimenInfoBox.SetActive(true);
        speciesInfoBox.SetActive(true);
        blurbInfoBox.SetActive(true);

        yield return new WaitForSeconds(4);

        specimenInfoBox.SetActive(false);
        speciesInfoBox.SetActive(false);
        blurbInfoBox.SetActive(false);
        infoBoxCoroutine = null; //Resets the checker when timer is done
    }

    private void StartScannedIndicator()
    {
        if (ScannedIndicator != null)
        {
            StopCoroutine(infoBoxCoroutine);
        }

        ScannedIndicator = StartCoroutine(ActivateIndicator());
    }

    private void RestartScannedIndicator()
    {
        if (infoBoxCoroutine != null)
        {
            StopCoroutine(infoBoxCoroutine);
        }

        infoBoxCoroutine = StartCoroutine(ActivateIndicator());
    }

    public IEnumerator ActivateIndicator()
    {
        scannedIndicator.SetActive(true);

        yield return new WaitForSeconds(1);

        scannedIndicator.SetActive(false);
        ScannedIndicator = null; //Resets the checker when timer is done
    }

    void ShowBirdInfo(BirdInfo bird)
    {
        // Set info in Info Box
        birdStatText.fontSize = 55.22f;
        birdStatText.text = $"Species: {bird.species}\n" +
                            $"Sex: {bird.curSex}\n" +
                            $"Length: {bird.length}in\n" +
                            $"Weight: {bird.weight}oz";

        birdSpeciesText.text = $"Species: {bird.species}\n" +
                               $"Scientific Name: {bird.latin}\n" +
                               $"Length Range: {bird.lengthMin}in - {bird.lengthMax}in\n" +
                               $"Weight Range: {bird.weightMin}oz - {bird.weightMax}oz\n";

        birdInfoText.text = bird.info;

        /*// Display Info Box
        specimenInfoBox.SetActive(true);
        speciesInfoBox.SetActive(true);
        blurbInfoBox.SetActive(true);*/
    }
    void LogBirdInfo(BirdInfo bird)
    {
        // Add a log entry to the logbook script with the bird's BirdInfo
        Logbook.GetComponent<Logbook>().AddBirdEntry(bird);
    }
}
