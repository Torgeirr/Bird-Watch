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
                info = "A common type of sparrow in the North Florida area. Introduced from Europe, but they thrive here. Especially around human habitation.";
                lengthMin = 5.9f;
                lengthMax = 6.7f;
                weightMin = 0.9f;
                weightMax = 1.1f;

                // Assign randomized length for bird within its range
                length = Random.Range(lengthMin, lengthMax);
                // Assign the ration of the actual length and the max length
                float ratio = length / lengthMax;
                // Determine the weight by the same ratio
                weight = (weightMin + ratio);

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
                info = "Makes its nests in shrubs and spends its time foraging on the ground and singing in the trees.";
                lengthMin = 8.3f;
                lengthMax = 9.3f;
                weightMin = 1.19f;
                weightMax = 2.29f;

                // Assign randomized length for bird within its range
                length = Random.Range(lengthMin, lengthMax);
                // Assign the ration of the actual length and the max length
                float ratio = length / lengthMax;
                // Determine the weight by the same ratio
                weight = (weightMin + ratio);

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
