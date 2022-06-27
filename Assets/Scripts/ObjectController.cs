using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ObjectController script is responsible for initializing sceneObject and all of it's parts,
/// and then communicating that list to the CoreController for UI init.
/// </summary>

public class ObjectController : MonoBehaviour
{
    public GameObject root;
    public GameObject snapIndicatorPrefab;
    public GameObject raycastPlane;

    public List<LimbController> limbsList = new List<LimbController>();
    
    private void Start()
    {
        // Usually I would have this script's Initialize() method as part of Start(), however
        // I separated them out here just for unit-testing demonstration purposes
        Initialize();
    }

    public void Initialize()
    {
        // Initialize colliders for all objects we want to consider movable and form a list
        foreach (Transform limb in root.GetComponentsInChildren<Transform>())
        {
            if (limb.transform == root.transform) continue;
            
            limb.gameObject.AddComponent<BoxCollider>();
            LimbController limbController = limb.gameObject.AddComponent<LimbController>();
            limbController.snapIndicatorPrefab = snapIndicatorPrefab;
            limbController.rootObject = this;
            
            limbsList.Add(limbController);
        }
        
        Actions.OnNewObjectInitialized(this);
    }

    public void ResetLimbs()
    {
        foreach (LimbController limb in limbsList)
        {
            // Since we want to consider our root to be immovable, we skip this transform
            if (limb.transform == root.transform) continue;

            if(limb.attached == false)
                limb.ResetLimb();
        }
    }

    private void HandleSelectionChanged(LimbController limb)
    {
        Debug.LogError("Selection changed to: " + limb.name + "");
    }
}
