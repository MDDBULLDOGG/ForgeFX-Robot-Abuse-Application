using System;
using UnityEngine;


public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private BoxCollider rayCastPlaneCollider;
    [SerializeField] private GameObject snapIndicatorPrefab;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float snapDistance;
    
    private LimbController selection;
    private GameObject snapIndicatorObject;
    private bool mouseIsDragging = false;

    public static event Action<LimbController> OnMouseEnterLimb = delegate { };
    public static event Action<LimbController> OnMouseExitLimb = delegate { };
    public static event Action<LimbController, Vector2> OnMouseDragLimb = delegate { };
    public static event Action<LimbController> OnSnapLimb = delegate { };


    private void OnEnable()
    {
        SelectionManager.OnMouseEnterLimb += HandleMouseOverLimb;
        SelectionManager.OnMouseExitLimb += HandleMouseExitLimb;
    }

    private void OnDisable()
    {
        SelectionManager.OnMouseEnterLimb -= HandleMouseOverLimb;
        SelectionManager.OnMouseExitLimb -= HandleMouseExitLimb;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (!mouseIsDragging)
        {
            if (Physics.Raycast(ray, out hit))
            {
                LimbController limb = hit.transform.GetComponent<LimbController>();
            
                if (limb)
                {
                    OnMouseEnterLimb(limb);
                }
                else
                { 
                    OnMouseExitLimb(selection);
                }
            }
            else
            {
                if (selection)
                {
                    OnMouseExitLimb(selection);
                }
            }
        }

        // TODO: This is a bit messy and should be cleaned up particularly with the snapPos assignments and snapIndicatorObject
        if (mouseIsDragging)
        {
            if (!rayCastPlaneCollider.Raycast(ray, out hit, 10)) 
                return;
            
            selection.snapPos = (Quaternion.Euler(selection.limbValues.originalParent.transform.eulerAngles) * selection.limbValues.originalLocalPosition) +
                            selection.limbValues.originalParent.transform.position;
            
            Vector3 distance = new Vector3(hit.point.x, hit.point.y, 0) -
                               new Vector3(selection.snapPos.x, selection.snapPos.y, 0);

            if (distance.sqrMagnitude > snapDistance)
            {
                if (snapIndicatorObject == null)
                {
                    snapIndicatorObject = Instantiate(snapIndicatorPrefab, selection.snapPos, Quaternion.identity);
                    snapIndicatorObject.transform.localScale = new Vector3(snapDistance, snapDistance, snapDistance);
                }
                
                OnMouseDragLimb(selection, new Vector2(hit.point.x, hit.point.y));
            }
            else
            {
                if (snapIndicatorObject != null)
                {
                    Destroy(snapIndicatorObject);
                }
                
                if (selection.attached == false)
                {
                    OnSnapLimb(selection);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (snapIndicatorObject != null)
                {
                    Destroy(snapIndicatorObject);
                }
                
                mouseIsDragging = false;
            }
        }

        // This will currently detect a simple click as a 'drag'
        // TODO: Add delay/positional change marker?
        if (Input.GetMouseButtonDown(0) && selection) 
            mouseIsDragging = true;
    }

    private void HandleMouseOverLimb(LimbController limb)
    {
        if (limb != selection)
        {
            ClearSelection();

            selection = limb;

            foreach (LimbController child in selection.GetComponentsInChildren<LimbController>())
            {
                child.limbValues.meshRenderer.material = highlightMaterial;
            }
        }
    }

    private void HandleMouseExitLimb(LimbController limb)
    {
        ClearSelection();
    }
    
    private void ClearSelection()
    {
        if (selection != null)
        {
            foreach (LimbController child in selection.GetComponentsInChildren<LimbController>())
            {
                child.limbValues.meshRenderer.material = child.limbValues.originalMaterial;
            }
            selection = null;
        }
    }
}
