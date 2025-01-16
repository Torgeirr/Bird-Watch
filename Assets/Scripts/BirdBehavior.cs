using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets.DynamicMoveProvider;

public class BirdBehavior : MonoBehaviour
{
    // All bird spawners and despawners
    public TreeLanding selectedTree;
    public Exit exit, exit1, exit2, exit3;
    public Transform selectedLanding;
    public Despawner selectedExit;
    public GameObject tree, tree1, tree2, tree3, landing, landing1, landing2, landing3, spawner, spawner1, spawner2, spawner3;
    private enum BehaviorStates{ none, flyingToBranch, landed, flyingToDespawn};
    private BehaviorStates currentState = BehaviorStates.none;
    public float flySpeed = 5f, flightProgress = 0f;
    public Transform spawnPoint;
    public bool doneSitting = false;
    private BirdManager manager;
    private Transform playerCamera;
    public Renderer birdRenderer;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<BirdManager>();
        playerCamera = Camera.main.transform;
        if(manager == null)
        {
            Debug.LogError("LandingSpotManager not found!");
            return;
        }
        else
        {
            PickTreeAndLanding();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckBehavior();
        FacePlayer();
    }

    public void CheckBehavior()
    {
        if (currentState != BehaviorStates.none)
        {
            if (currentState == BehaviorStates.flyingToBranch)
            {
                flightProgress += flySpeed * Time.deltaTime / Vector3.Distance(transform.position, selectedLanding.transform.position);

                transform.position = Vector3.Slerp(transform.position, selectedLanding.transform.position, flightProgress);
                if(flightProgress >= 1f)
                {
                    transform.position = selectedLanding.transform.position;
                    currentState = BehaviorStates.landed;
                    StartCoroutine(SitOnBranch());
                }

            }
            else if (currentState == BehaviorStates.landed)
            {
                if (!doneSitting)
                {
                    return;
                }
                else
                {
                    ResetSelectedLandingOnTree(selectedLanding);
                    AssignExit();
                    currentState = BehaviorStates.flyingToDespawn;
                }
            }
            else if (currentState == BehaviorStates.flyingToDespawn)
            {
                flightProgress += flySpeed * Time.deltaTime / Vector3.Distance(transform.position, selectedExit.transform.position);

                transform.position = Vector3.Slerp(transform.position, selectedExit.transform.position, flightProgress);
                if (flightProgress >= 1f)
                {
                    Despawn();
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void PickTreeAndLanding()
    {
        // Find a tree that isn't full
        selectedTree = manager.GetRandomAvailableTree();
        if(selectedTree == null)
        {
            Debug.LogWarning("All trees full");
            return;
        }

        //Pick a random landing spot on tree
        selectedLanding = manager.GetRandomAvailableLanding(selectedTree);
        if (selectedLanding == null)
        {
            Debug.LogWarning("No available landings on the selected tree.");
            return;
        }

        // Mark the landing spot as taken
        MarkLandingSpotAsTaken(selectedLanding);

        currentState = BehaviorStates.flyingToBranch;
    }

    private void MarkLandingSpotAsTaken(Transform landingSpot)
    {
        if (landingSpot.name == selectedTree.landing.name) selectedTree.landingTaken = true;
        else if (landingSpot.name == selectedTree.landing1.name) selectedTree.landing1Taken = true;
        else if (landingSpot.name == selectedTree.landing2.name) selectedTree.landing2Taken = true;
        else if (landingSpot.name == selectedTree.landing3.name) selectedTree.landing3Taken = true;

        Debug.Log("Selected Landing: " + selectedLanding.name);
        // Update the tree's full status
        selectedTree.CheckTreeFull();
    }

    public void FacePlayer()
    {
        if (playerCamera != null && birdRenderer != null)
        {
            Vector3 toPlayer = playerCamera.position - transform.position;
            toPlayer.y = -90; // Ignore vertical difference
            Quaternion targetRotation = Quaternion.LookRotation(toPlayer);

            birdRenderer.transform.rotation = targetRotation;

            // Compare current position to last to determine which direction to face
            Vector3 movementDirection = transform.position - lastPosition;

            // Flip based on direction relative to player
            if (movementDirection.x > 0)
            {
                birdRenderer.transform.localScale = new Vector3(0.105f, 0.105f, 0.105f); // Facing right
            }
            else if (movementDirection.x < 0)
            {
                birdRenderer.transform.localScale = new Vector3(-0.105f, 0.105f, 0.105f); // Facing left
            }

            lastPosition = transform.position;
        }
        else
        {
            Debug.LogError("Either playerCamera is null or birdRenderer is null");
        }
    }
    private void AssignLandings()
    {
        landing = selectedTree.GetComponent<TreeLanding>().landing;
        landing1 = selectedTree.GetComponent<TreeLanding>().landing1;
        landing2 = selectedTree.GetComponent<TreeLanding>().landing2;
        landing3 =  selectedTree.GetComponent<TreeLanding>().landing3;
    }

    private void ResetSelectedLandingOnTree(Transform landingSpot)
    {
        if (landingSpot.name == selectedTree.landing.name) selectedTree.landingTaken = false;
        else if (landingSpot.name == selectedTree.landing1.name) selectedTree.landing1Taken = false;
        else if (landingSpot.name == selectedTree.landing2.name) selectedTree.landing2Taken = false;
        else if (landingSpot.name == selectedTree.landing3.name) selectedTree.landing3Taken = false;

        // Update the tree's full status
        selectedTree.CheckTreeFull();
    }
    private Despawner AssignExit()
    {
        selectedExit = manager.GetRandomDespawner();
        return selectedExit;
    }

    private IEnumerator SitOnBranch()
    {
        flightProgress = 0f;
        float timer = Random.Range(1f, 15f);
        yield return new WaitForSeconds(timer);

        doneSitting = true;
    }
    public void Despawn()
    {
       
        BirdInfo birdInfo = gameObject.GetComponent<BirdInfo>();
        if (birdInfo != null)
        {
            int thisID = birdInfo.ID;

            // Remove this bird's ID from the birdIDs List in BirdManager
            BirdManager.Instance.birdIDs.Remove(thisID);
        }

        // Set the bird inactive
        gameObject.SetActive(false);
    }
    
}
