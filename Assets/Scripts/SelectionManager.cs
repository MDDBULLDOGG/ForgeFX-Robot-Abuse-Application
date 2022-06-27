using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Material highlightMaterial;

    private LimbController selection;
    
    void OnEnable()
    {
        LimbController.OnLimbStatusChanged += HandleLimbStatusChanged;
    }

    void OnDisable()
    {
        LimbController.OnLimbStatusChanged -= HandleLimbStatusChanged;
    }

    private void HandleLimbDragged(LimbController limbRef)
    {
        Debug.LogError("Dragging limb!");
    }
    
    private void HandleLimbStatusChanged(LimbController limbRef)
    {
        Debug.LogError("Limb status changed!");
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            LimbController limb = hit.transform.GetComponent<LimbController>();

            if (limb && limb != selection)
            {
                UpdateSelection(limb);
            }
            else if (limb == null && selection)
            {
                ClearSelection();
            }
        }
        else
        {
            ClearSelection();
        }
    }

    private void UpdateSelection(LimbController limb)
    {
        ClearSelection();
        
        selection = limb;
        selection.limbValues.meshRenderer.material = highlightMaterial;
    }

    private void ClearSelection()
    {
        if (selection != null)
        {
            selection.limbValues.meshRenderer.material = selection.limbValues.originalMaterial;
            selection = null;
        }
    }
}
