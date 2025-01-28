using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Logbook : MonoBehaviour
{
    [SerializeField] private GameObject entryTemplate; // Prototype Entry GameObject
    [SerializeField] private Transform LogbookParent; // Parent object where entries will be housed
    [SerializeField] private List<Transform> entryPositions; // List of positions for entries (max 4)
    [SerializeField] private List<GameObject> activeEntries = new List<GameObject>(); // Active log entries
    public List<GameObject> BirdIndex = new List<GameObject>();
    public int curIndex;
    public GameObject openingIndexPage, hSparrowIndex, CardinalIndex;
    public GameObject binoculars;
    public Binoculars binoScript;
    public GameObject LogbookUpPOS, LogbookDownPOS;
    private float moveSpeed = 5f;
    public List<int> birdIDs = new List<int>();
    private int thisID;
    public GameObject LMBInstructions;
    public bool isLMBClicked = false, newLMBClick = false, firstClicked = false, isRaised = false;

    private const int maxVisibleEntries = 4;
    private const int maxBirdIDs = 4;

    public void Start()
    {
        binoScript = binoculars.GetComponent<Binoculars>();
        InitBirdIndex();
    }
    public void FixedUpdate()
    {
        
        // Only let player operate Logbook if Binoculars are not being used
        if (!binoScript.isZoomed)
        {
            // Operates both Logbook raise/lower action and the Logbook UI instructions
            // Uses same general logic as FixedUpdate() of Binoculars.cs, just in a function 
            LogbookOperation();
        }
        
    }
    // Method to add a new bird entry to the logbook
    public void AddBirdEntry(BirdInfo birdInfo)
    {
        //Only add entry if scanned bird's ID is not already in the Logbook's list of IDs
        if (!BirdManager.Instance.birdIDs.Contains(thisID))
        {
            // Create a new log entry based on the template
            GameObject newEntry = Instantiate(entryTemplate, LogbookParent);
            newEntry.SetActive(true);

            // Populate the log entry with the bird's data
            PopulateEntry(newEntry, birdInfo);

            // Add the new entry at the top of the list
            activeEntries.Insert(0, newEntry);

            // Update entry positions
            UpdateEntryPositions();

            // Remove the oldest entry and ID if the list exceeds max limit (4 entries)
            if (activeEntries.Count > maxVisibleEntries)
            {
                RemoveEntryAndID();
            }
        }
        else
        {
            Debug.Log("Current bird is already logged in Logbook");
        }
    }

    // Populate a log entry with bird data
    private void PopulateEntry(GameObject entry, BirdInfo birdInfo)
    {
        GameObject entryImgBox = entry.transform.Find("Image Box").gameObject; //ref to Image Box parent that houses all the bird images for the Entry prefab
        GameObject refImage = null; //ref to what the appropriate image will be assigned to

        // Check bird species
        if(birdInfo.curSpecies == BirdInfo.SpeciesList.hSparrow)
        {
            // Check bird gender, assign the appropriate reference image
            if (birdInfo.curSex == BirdInfo.SexCategory.male)
            {
                refImage = entryImgBox.transform.Find("House Sparrow Image Male").gameObject;
            }
            else if (birdInfo.curSex == BirdInfo.SexCategory.female)
            {
                refImage = entryImgBox.transform.Find("House Sparrow Image Female").gameObject;
            }

            //Turn the referenced image's gameobject on
            if (refImage != null)
            {
                refImage.SetActive(true);
            }

            //Turn off all reference images that aren't the appropriate one
            foreach (Transform child in entryImgBox.transform)
            {
                if(child.gameObject != refImage)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        else if (birdInfo.curSpecies == BirdInfo.SpeciesList.cardinal)
        {
            // Check bird gender, assign the appropriate reference image
            if (birdInfo.curSex == BirdInfo.SexCategory.male)
            {
                refImage = entryImgBox.transform.Find("Cardinal Image Male").gameObject;
            }
            else if (birdInfo.curSex == BirdInfo.SexCategory.female)
            {
                refImage = entryImgBox.transform.Find("Cardinal Image Female").gameObject;
            }

            //Turn the referenced image's gameobject on
            if (refImage != null)
            {
                refImage.SetActive(true);
            }

            //Turn off all reference images that aren't the appropriate one
            foreach (Transform child in entryImgBox.transform)
            {
                if (child.gameObject != refImage)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Assign the bird's ID to local holding variable and add it to birdIDs List
        thisID = birdInfo.ID;
        birdIDs.Add(thisID);

        // Find the species TMP and update with the bird's species info
        TMP_Text speciesText = entry.transform.Find("Species Text").GetComponent<TMP_Text>();
        speciesText.fontSize = 34.8f;
        speciesText.text = birdInfo.species;

        // Find the gender TMP and update with the bird's gender info
        TMP_Text genderText = entry.transform.Find("Gender Text").GetComponent<TMP_Text>();
        genderText.text = birdInfo.curSex.ToString();

        // Find the length and weight TMPs and update with the bird's length and weight info
        TMP_Text lengthText = entry.transform.Find("Length Text").GetComponent<TMP_Text>();
        lengthText.fontSize = 34.8f;
        lengthText.text = $"Length: {birdInfo.length:F1}in";

        TMP_Text weightText = entry.transform.Find("Weight Text").GetComponent<TMP_Text>();
        weightText.fontSize = 34.8f;
        weightText.text = $"Weight: {birdInfo.weight:F1}oz";

        // Find the info blurb TMP and update with the bird's blurb info
        TMP_Text infoText = entry.transform.Find("Info Text").GetComponent<TMP_Text>();
        infoText.fontSize = 30.7f;
        infoText.text = birdInfo.info;

    }

    private void UpdateEntryPositions()
    {
        // Update position of each active entry
        for (int i = 0; i < activeEntries.Count && i < entryPositions.Count; i++)
        {
            activeEntries[i].transform.localPosition = entryPositions[i].localPosition;
        }
    }

    public void RemoveEntryAndID()
    {
        //Remove the appropriate bird ID from birdIDs List
        birdIDs.RemoveAt(birdIDs.Count - 1);

        //Assign the last entry in activeEntries and remove it, then destroy the gameobject
        GameObject removedEntry = activeEntries[activeEntries.Count - 1];
        activeEntries.RemoveAt(activeEntries.Count - 1);
        Destroy(removedEntry);
    }

    public void LogbookOperation()
    {
        newLMBClick = false;
        //If the player is clicking LMB while LMB isn't considered clicked
        if (!isLMBClicked && Input.GetMouseButton(0))
        {
            isLMBClicked = true;
            newLMBClick = true;
            //Debug.Log("newLMBClick turned to true");

            // If this is the first left-click, tell LMBInstruction to turn off and not instruct the player
            if (!firstClicked)
            {
                LMBInstructions.GetComponent<LMBInstruction>().alreadyClicked = true;
            }

        }
        //If LMB is already considered clicked, but the player is not clicking it
        else if (isLMBClicked && !Input.GetMouseButton(0))
        {
            isLMBClicked = false;
        }

        if (newLMBClick)
        {
            isRaised = !isRaised;
        }

        if (isRaised)
        {
            if (Input.mouseScrollDelta.y < 0) // If scrolled up
            {

                Debug.Log("Page scrolled UP");
                SetPage(1);
            }
            else if (Input.mouseScrollDelta.y > 0) // If scrolled down
            {
                Debug.Log("Page scrolled DOWN");
                SetPage(-1);
            }
        }


        //Replace with page scrolling logic
        //markerFrom = curMarkerPOS;
        // curMarkerPOS = ZoomMarkers[curZoom];
        // markerTo = curMarkerPOS;

        //float newFOV = isZoomed ? ZoomLevels[curZoom] : normalFOV;
        //BinocularCamera.fieldOfView = Mathf.Lerp(BinocularCamera.fieldOfView, newFOV, Time.deltaTime * zoomSpeed);

        // if (markerFrom != null && markerTo != null)
        // {
        //     zoomMarker.transform.position = Vector3.Lerp(markerFrom.transform.position, markerTo.transform.position, Time.deltaTime * zoomSpeed);
        // }
        // else
        // {
        //     Debug.LogWarning("MarkerFrom or MarkerTo not assigned");
        // }

        // Update the transform of the Logbook object itself so it stays where it should be (whether player is using it or has it down)
        transform.position = Vector3.Lerp(transform.position, isRaised ? LogbookUpPOS.transform.position : LogbookDownPOS.transform.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, isRaised ? LogbookUpPOS.transform.rotation : LogbookDownPOS.transform.rotation, Time.deltaTime * moveSpeed);
    }

        //Sets the Logbook Encyclopedia page as the player scrolls (when book is raised)
        public void SetPage(int scrollDirection)
        {

            curIndex = Mathf.Clamp(curIndex + scrollDirection, 0, BirdIndex.Count - 1);
            for (int i = 0; i < BirdIndex.Count; i++)
            {
                if(i == curIndex)
                {
                    BirdIndex[i].SetActive(true); //Enable the current page
                }
            else
                {
                    BirdIndex[i].SetActive(false); //Disable all other pages
                }
            }
         }
    

    public void InitBirdIndex()
    {
        BirdIndex.Add(openingIndexPage);
        curIndex = BirdIndex.Count - 1;
    }
}


