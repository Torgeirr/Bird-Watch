using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Binoculars : MonoBehaviour
{
    public Camera mainCamera, BinocularCamera;
    public float zoomDistance = 10f;
    public float restingDistance = 50f;
    public float zoomedFOV = 5f;
    public float normalFOV = 60f;
    public float binoMoveSpeed = 5f, zoomSpeed = 3f;

    public GameObject BinocularDisplay, BinosUpPOS, BinosDownPOS;
    private Coroutine infoBoxCoroutine, ScannedIndicator;

    public float detectionRange = 50f; //Range of raycast
    public LayerMask birdLayer;
    public GameObject specimenInfoBox, speciesInfoBox, blurbInfoBox; // Bino UI panel that displays bird info
    public GameObject scannedIndicator; // Bino UI indicator that shows when bird scanned
    public GameObject RMBInstructions;
    public TMPro.TextMeshProUGUI birdStatText, birdSpeciesText, birdInfoText;

    public bool isZoomed = false, isRMBClicked = false, newRMBClick = false, tester = false, firstClicked = false;
    private Vector3 zoomTarget; // Spot the camera moves towards
    private Vector3 restingPosition; // Camera's starting/returning position

    void Start()
    {
        // Store the initial position of the camera
        restingPosition = mainCamera.transform.position;
        zoomTarget = mainCamera.transform.position + mainCamera.transform.forward * restingDistance;
    }

    void FixedUpdate()
    {
        newRMBClick = false;

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
        else if (isRMBClicked && !Input.GetMouseButton(1))
        {
            isRMBClicked = false;
        }

        if (newRMBClick)
        {
            isZoomed = !isZoomed;
        }

        // If right-click is down, is zoomed
        /*if (!isZoomed && Input.GetMouseButton(1))
        {
            isZoomed = true;
        }
        else if(isZoomed && !Input.GetMouseButton(1)) 
        {
            isZoomed = false;
        }*/


        // Move the camera toward the zoom target - Lerp makes it more immediate than Slerp
        //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, zoomTarget, Time.deltaTime * zoomSpeed);
        // Lerp between zoomed POV and non-zoomed POV depending on if isZoomed - Lerp makes it more immediate than Slerp
        //mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, isZoomed ? zoomedFOV : normalFOV, Time.deltaTime * zoomSpeed);

        // Left click to send raycast while zooming
        //mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, isZoomed ? zoomedFOV : normalFOV, Time.deltaTime * zoomSpeed);
        //Lerp between "Binos up" position and "Binos down" position, depending on if isZoomed  
        BinocularCamera.fieldOfView = Mathf.Lerp(BinocularCamera.fieldOfView, isZoomed ? zoomedFOV : normalFOV, Time.deltaTime + zoomSpeed);
        BinocularDisplay.transform.position = Vector3.Lerp(BinocularDisplay.transform.position, isZoomed ? BinosUpPOS.transform.position : BinosDownPOS.transform.position, Time.deltaTime * binoMoveSpeed);

        if (Input.GetMouseButtonDown(0))
            {
                Ray ray = new(BinocularCamera.transform.position, BinocularCamera.transform.forward);
                Debug.Log("Drawing Ray");
                Debug.DrawRay(BinocularCamera.transform.position, BinocularCamera.transform.forward, Color.green);
                // If raycast hits something in the "Bird" Layer
                if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, birdLayer))
                {
                
                    BirdInfo bird = hit.collider.GetComponent<BirdInfo>();
                    if (bird != null)
                    {
                        Debug.Log("Scanned a bird");
                        ShowBirdInfo(bird);
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

        /*//Display Info Box
        specimenInfoBox.SetActive(true);
        speciesInfoBox.SetActive(true);
        blurbInfoBox.SetActive(true);*/
    }
}
