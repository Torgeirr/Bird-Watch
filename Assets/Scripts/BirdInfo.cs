using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdInfo : MonoBehaviour
{
    public string species, latin, info;
    public float length, weight, lengthMin, lengthMax, weightMin, weightMax, wingMin;
    public GameObject mockSprite;
    public enum SpeciesList { none, hSparrow, cardinal, chickadee, baldEagle };
    public SpeciesList curSpecies = SpeciesList.none;
    public enum SexCategory { none, male, female };
    public SexCategory curSex = SexCategory.none;
    public Renderer birdRenderer; //Ref to the bird's renderer, to swap male/female materials
    public Material maleMaterial, femaleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //birdRenderer = GetComponentInChildren<Renderer>();
        SetStats();
    }

    private void SetStats()
    {
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
        }
    }
}
