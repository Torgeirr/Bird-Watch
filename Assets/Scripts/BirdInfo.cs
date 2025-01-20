using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdInfo : MonoBehaviour
{
    public string species, latin, info;
    public int ID;
    public float length, weight, lengthMin, lengthMax, weightMin, weightMax, wingMin;
    public GameObject mockSprite;
    //public BirdManager manager;
    public enum SpeciesList { none, hSparrow, cardinal, chickadee, baldEagle };
    public SpeciesList curSpecies = SpeciesList.none;
    public enum SexCategory { none, male, female };
    public SexCategory curSex = SexCategory.none;
    public Renderer birdRenderer; //Ref to the bird's renderer, to swap male/female materials
    public Material maleMaterial, femaleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        SetStats(); //Set bird stats
    }

    private void SetStats()
    {
        SetID(); // Give this bird a unique ID
        if(curSpecies != SpeciesList.none)
        {
            if(curSpecies == SpeciesList.hSparrow)
            {
                //If the bird is a house sparrow, give it house sparrow stats and info
                species = "House Sparrow";
                latin = "Passer domesticus";
                info = "A common type of sparrow in the North Florida area. Introduced in 1851, they have come to thrive in both American continents.";
                lengthMin = 5.9f;
                lengthMax = 6.7f;
                weightMin = 0.9f;
                weightMax = 1.1f;

                // Give random length to bird, then round to two decimals
                float exactLength = (Random.Range(lengthMin, lengthMax));
                length = Mathf.Round(exactLength * 100f) / 100f;

                // Assign the ratio of length and max length to determine weight proportion [
                float ratio = (length - lengthMin) / (lengthMax - lengthMin);

                // Determine the weight by the same ratio
                float exactWeight = weightMin + ratio * (weightMax - weightMin);
                weight = Mathf.Round(exactWeight * 100f) / 100f;

                //Randomly select sex
                int rando = Random.Range(0, 100);
                if(rando <= 50f)
                {
                    curSex = SexCategory.female;
                    birdRenderer.material = femaleMaterial;
                }
                else
                {
                    curSex = SexCategory.male;
                    birdRenderer.material = maleMaterial;
                }
            }
            else if (curSpecies == SpeciesList.cardinal)
            {
                //If the bird is a cardinal, give it cardinal stats and info
                species = "Northern Cardinal";
                latin = "Cardinalis cardinalis";
                info = "Found foraging on the ground and singing high in the trees, Cardinals are vibrant and territorial natives to the Eastern United States.";
                lengthMin = 8.3f;
                lengthMax = 9.3f;
                weightMin = 1.19f;
                weightMax = 2.29f;

                // Give random length to bird, then round to two decimals
                float exactLength = (Random.Range(lengthMin, lengthMax));
                length = Mathf.Round(exactLength * 100f) / 100f;

                // Assign the ratio of length and max length to determine weight proportion [
                float ratio = (length - lengthMin) / (lengthMax - lengthMin);

                // Determine the weight by the same ratio
                float exactWeight = weightMin + ratio * (weightMax - weightMin);
                weight = Mathf.Round(exactWeight * 100f) / 100f;

                //Randomly select sex
                int rando = Random.Range(0, 100);
                if (rando <= 50f)
                {
                    curSex = SexCategory.female;
                    birdRenderer.material = femaleMaterial;
                }
                else
                {
                    curSex = SexCategory.male;
                    birdRenderer.material = maleMaterial;
                }
            }
        }
    }

    public void SetID()
    {
        int tryID; // Ref to the ID number to try assigning

        // do/while loop will rerun the RNG whenever tryID's value is already in the birdIDs List
        do
        {
            tryID = Mathf.FloorToInt(Random.Range(100000, 999999));
        }
        while (BirdManager.Instance.birdIDs.Contains(tryID));

        //If not already in birdIDs List, add the new ID to the List and assign the global ID variable the value
        BirdManager.Instance.birdIDs.Add(tryID);
        this.ID = tryID;
    }
}
