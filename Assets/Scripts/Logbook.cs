using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class Logbook : MonoBehaviour
{
    [SerializeField] private GameObject entryTemplate; // Prototype Entry GameObject
    [SerializeField] private Transform LogbookParent; // Parent object where entries will be housed
    [SerializeField] private List<Transform> entryPositions; // List of positions for entries (max 4)
    [SerializeField] private List<GameObject> activeEntries = new List<GameObject>(); // Active log entries

    private const int maxVisibleEntries = 4;

    // Method to add a new bird entry to the logbook
    public void AddBirdEntry(BirdInfo birdInfo)
    {
        // Create a new log entry based on the template
        GameObject newEntry = Instantiate(entryTemplate, LogbookParent);
        newEntry.SetActive(true);

        // Populate the log entry with bird data
        PopulateEntry(newEntry, birdInfo);

        // Insert the new entry at the top of the list
        activeEntries.Insert(0, newEntry);

        // Update entry positions
        UpdateEntryPositions();

        // Remove the oldest entry if the list exceeds the visible limit
        if (activeEntries.Count > maxVisibleEntries)
        {
            GameObject removedEntry = activeEntries[activeEntries.Count - 1];
            activeEntries.RemoveAt(activeEntries.Count - 1);
            Destroy(removedEntry);
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
            // Check bird gender
            if (birdInfo.curSex == BirdInfo.SexCategory.male)
            {
                refImage = entryImgBox.transform.Find("House Sparrow Image Male").gameObject;
            }
            else if (birdInfo.curSex == BirdInfo.SexCategory.female)
            {
                refImage = entryImgBox.transform.Find("House Sparrow Image Female").gameObject;
            }

            if (refImage != null)
            {
                refImage.SetActive(true);
            }

            foreach (Transform child in entryImgBox.transform)
            {
                if(child.gameObject != refImage)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        

        // Assign species name
        TMP_Text speciesText = entry.transform.Find("Species Text").GetComponent<TMP_Text>();
        speciesText.text = birdInfo.species;

        // Assign gender
        TMP_Text genderText = entry.transform.Find("Gender Text").GetComponent<TMP_Text>();
        genderText.text = birdInfo.curSex.ToString();

        // Assign length and weight
        TMP_Text lengthText = entry.transform.Find("Length Text").GetComponent<TMP_Text>();
        lengthText.text = $"Length: {birdInfo.length:F1} in";

        TMP_Text weightText = entry.transform.Find("Weight Text").GetComponent<TMP_Text>();
        weightText.text = $"Weight: {birdInfo.weight:F1} oz";

        // Assign additional info
        TMP_Text infoText = entry.transform.Find("Info Text").GetComponent<TMP_Text>();
        infoText.text = birdInfo.info;
    }

    // Update the positions of all active entries
    private void UpdateEntryPositions()
    {
        for (int i = 0; i < activeEntries.Count && i < entryPositions.Count; i++)
        {
            activeEntries[i].transform.localPosition = entryPositions[i].localPosition;
        }
    }
}
