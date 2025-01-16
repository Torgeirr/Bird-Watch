using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BirdManager : MonoBehaviour
{
    public static BirdManager Instance { get; private set; } //Make singleton so only one BirdManager exists
    public GameObject bird, HSparrow, Cardinal;
    public List<TreeLanding> trees = new List<TreeLanding>();
    public List<Despawner> despawners = new List<Despawner>();
    public List<Spawner> spawners = new List<Spawner>();
    public List<int> birdIDs = new List<int>();

    public void Awake()
    {
        //Singleton setup
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        trees.AddRange(FindObjectsOfType<TreeLanding>());
        despawners.AddRange(FindObjectsOfType<Despawner>());
        spawners.AddRange(FindObjectsOfType<Spawner>());

        InvokeRepeating("SpawnBird", 1f, 1.5f);
    }

    public void SpawnBird()
    {
        if(GetRandomAvailableTree() != null)
        {
            int selection = BirdSelector(Mathf.RoundToInt(Random.Range(1f, 4f)));

            if (selection == 1)
            {
                Debug.Log("BirdSelector() returned 1");
                Instantiate(HSparrow, GetRandomSpawner().transform.position, Quaternion.identity);
            }
            else if(selection == 2)
            {
                Debug.Log("BirdSelector() returned 1");
                Instantiate(Cardinal, GetRandomSpawner().transform.position, Quaternion.identity);
            }
            else if (selection == 3)
            {
                Debug.Log("BirdSelector() returned 1");
                Instantiate(HSparrow, GetRandomSpawner().transform.position, Quaternion.identity);
            }
            else if (selection == 4)
            {
                Debug.Log("BirdSelector() returned 1");
                Instantiate(Cardinal, GetRandomSpawner().transform.position, Quaternion.identity);
            }
            else if (selection < 1 || selection > 4)
            {
                Instantiate(HSparrow, GetRandomSpawner().transform.position, Quaternion.identity);
                Debug.Log("Well this wasn't supposed to happen. BirdSelector() returned outside of its parameter range! Spawning House Sparrow just to be safe.");
            }
        }
    }

    public int BirdSelector(float rando)
    {
        if((rando >= 1) && (rando < 2))
        {
            return 1;
        }
        else if((rando >= 2) && (rando < 3))
        {
            return 2;
        }
        else if ((rando >= 3) && (rando < 4))
        {
            return 3;
        }
        else if ((rando >= 4))
        {
            return 4;
        }
        else
        {
            return 0;
        }
        

    }

    public TreeLanding GetRandomAvailableTree()
    {
        List<TreeLanding> availableTrees = trees.FindAll(tree => !tree.treeFull);
        if (availableTrees.Count == 0)
        {
            return null; // No available trees
        } 
        return availableTrees[Random.Range(0, availableTrees.Count)];
    }

    public Spawner GetRandomSpawner()
    {
        return spawners[Random.Range(0, spawners.Count)];
    }
    public Despawner GetRandomDespawner()
    {
        Despawner selectedDespawner = despawners[Random.Range(0, despawners.Count)];

        return selectedDespawner;
    }

    public Transform GetRandomAvailableLanding(TreeLanding tree) // Makes sure the tree is not taken
    {
        List<Transform> availableLandings = new List<Transform>();
        if (!tree.landingTaken) availableLandings.Add(tree.landing.transform);
        if (!tree.landing1Taken) availableLandings.Add(tree.landing1.transform);
        if (!tree.landing2Taken) availableLandings.Add(tree.landing2.transform);
        if (!tree.landing3Taken) availableLandings.Add(tree.landing3.transform);

        if (availableLandings.Count == 0) return null; // No available landings on this tree
        return availableLandings[Random.Range(0, availableLandings.Count)];
    }
}
