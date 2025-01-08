using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BirdManager : MonoBehaviour
{
    public GameObject bird;
    public List<TreeLanding> trees = new List<TreeLanding>();
    public List<Despawner> despawners = new List<Despawner>();
    public List<Spawner> spawners = new List<Spawner>();

    public void Awake()
    {
        trees.AddRange(FindObjectsOfType<TreeLanding>());
        despawners.AddRange(FindObjectsOfType<Despawner>());
        spawners.AddRange(FindObjectsOfType<Spawner>());

        InvokeRepeating("SpawnBird", 1f, 3f);
    }

    public void SpawnBird()
    {
        if(GetRandomAvailableTree() != null)
        {
            Instantiate(bird, GetRandomSpawner().transform.position, Quaternion.identity);
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
