using UnityEngine;

/// <summary>
/// The LimbController script is responsible for the logic that drives the limb selection/movement
/// feature, as well as the events for changing attachment status.
/// </summary>

public class LimbController : MonoBehaviour
{
    // Inspector Vars
    public GameObject snapIndicatorPrefab;

    public class OriginalValues
    {
        public Renderer meshRenderer;
        public Color originalColor;
        public GameObject originalParent;
        public Vector3 originalLocalPosition;
        public Material originalMaterial;
    }
    public OriginalValues limbValues = new OriginalValues();
    
    // Runtime Vars
    private BoxCollider rayCastPlaneCollider;
    public bool attached = true;
    private Vector3 openSocketPos;
    private GameObject snapIndicatorObject;
    public ObjectController rootObject;
    
    // isBeingDragged is necessary because our mouse can leave the limb while it's being dragged,
    // and we don't want the highlight to change until OnMouseUp in that case.
    private bool isBeingDragged;

    // Arbitrary distance for when we should snap a limb to it's socket
    private float snapDistance = 0.0025f;
    
    // void OnEnable()
    // {
    //     Actions.OnMouseOver += HandleMouseOver;
    // }
    //
    // void OnDisable()
    // {
    //     Actions.OnMouseOver -= HandleMouseOver;
    // }
    
    private void Start()
    {
        limbValues.originalColor = this.GetComponent<Renderer>().material.color;
        limbValues.originalParent = this.transform.parent.gameObject;
        limbValues.originalLocalPosition = this.transform.localPosition;
        limbValues.meshRenderer = this.GetComponent<Renderer>();
        limbValues.originalMaterial = this.GetComponent<Renderer>().material;

        rayCastPlaneCollider = rootObject.raycastPlane.GetComponent<BoxCollider>();
    }
    
    // private void OnMouseEnter()
    // {
    //     HighlightAllChildren();
    // }

    private void HandleMouseOver()
    {
        HighlightAllChildren();
    }
    
    private void OnMouseDrag()
    {
        if (!isBeingDragged)
            isBeingDragged = true;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (!rayCastPlaneCollider.Raycast(ray, out hit, 10)) 
            return;
        
        // This is calculating the local socket position every frame when the mouse is dragging.
        // TODO: This is inefficient as we should only be checking the distance itself, fix it
        openSocketPos = (Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition) +
                        limbValues.originalParent.transform.position;

        // Calculating distance as a 2d length against the raycastPanel to get around the depth issue.
        // This only works because our camera is fixed.
        Vector3 distance = new Vector3(hit.point.x, hit.point.y, 0) -
                           new Vector3(openSocketPos.x, openSocketPos.y, 0);
            
        if (attached)
        {
            if (snapIndicatorObject != null)
            {
                Destroy(snapIndicatorObject);
            }
            
            if (distance.sqrMagnitude > snapDistance)
            {
                this.transform.parent = null;

                attached = false;
                Actions.OnLimbStatusChanged(this);

                // CoreController.Instance.attachmentStatusChanged.Invoke(this.name, attached);
            }
        } 
        else
        {
            if (snapIndicatorObject == null)
            {
                snapIndicatorObject = Instantiate(snapIndicatorPrefab, openSocketPos, Quaternion.identity);
                snapIndicatorObject.transform.localScale = new Vector3(snapDistance, snapDistance, snapDistance);
            }
            
            if (distance.sqrMagnitude < snapDistance)
            {
                this.transform.parent = limbValues.originalParent.transform;
                this.transform.position = openSocketPos;
                    
                attached = true;
                Actions.OnLimbStatusChanged(this);

                // CoreController.Instance.attachmentStatusChanged.Invoke(this.name, attached);
            }
            else
            {
                this.transform.position = hit.point;
            }
        }
    }

    private void OnMouseUp()
    {
        if (isBeingDragged)
            isBeingDragged = false;
        
        if (snapIndicatorObject != null)
        {
            Destroy(snapIndicatorObject);
        }
    }

    private void OnMouseExit()
    {
        if(isBeingDragged)
            return;
        
        UnHighlightAllChildren();
    }

    public void ResetLimb()
    {
        openSocketPos = (Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition) +
                        limbValues.originalParent.transform.position;
        
        this.transform.parent = limbValues.originalParent.transform;
        this.transform.position = openSocketPos;
                    
        attached = true;
        Actions.OnLimbStatusChanged(this);
    }

    private void HighlightAllChildren()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (LimbController limb in this.GetComponentsInChildren<LimbController>())
        {
            limb.Highlight();
        }
    }

    private void Highlight()
    {
        limbValues.meshRenderer.material.color = Color.yellow;
    }

    private void UnHighlightAllChildren()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (LimbController limb in this.GetComponentsInChildren<LimbController>())
        {
            limb.Unhighlight();
        }
    }
    
    private void Unhighlight()
    {
        limbValues.meshRenderer.material.color = limbValues.originalColor;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var newPos = Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition;
        Gizmos.DrawWireSphere(limbValues.originalParent.transform.position + newPos, 0.025f);
    }
}
