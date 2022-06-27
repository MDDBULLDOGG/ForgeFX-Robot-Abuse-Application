using System;
using UnityEngine;

/// <summary>
/// The LimbController script is responsible for the logic that drives the limb selection/movement
/// feature, as well as the events for changing attachment status.
/// </summary>

public class LimbController : MonoBehaviour
{
    // Inspector Vars
    public GameObject snapIndicatorPrefab;
    public static event Action<LimbController> OnLimbStatusChanged = delegate { };

    public class OriginalValues
    {
        public Renderer meshRenderer;
        public Material originalMaterial;
        public GameObject originalParent;
        public Vector3 originalLocalPosition;
        
        public OriginalValues(LimbController ctx)
        {
            meshRenderer = ctx.GetComponent<Renderer>();
            originalMaterial = meshRenderer.material;
            originalParent = ctx.transform.parent.gameObject;
            originalLocalPosition = ctx.transform.localPosition;
        }
    }
    
    // Runtime Vars
    private BoxCollider rayCastPlaneCollider;
    public bool attached = true;
    private Vector3 openSocketPos;
    private GameObject snapIndicatorObject;
    public ObjectController rootObject;

    public OriginalValues limbValues;
    
    // isBeingDragged is necessary because our mouse can leave the limb while it's being dragged,
    // and we don't want the highlight to change until OnMouseUp in that case.
    private bool isBeingDragged;

    // Arbitrary distance for when we should snap a limb to it's socket
    private float snapDistance = 0.0025f;

    private void Start()
    {
        limbValues = new OriginalValues(this);
        
        rayCastPlaneCollider = rootObject.raycastPlane.GetComponent<BoxCollider>();
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
                OnLimbStatusChanged(this);
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
                OnLimbStatusChanged(this);
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

    public void ResetLimb()
    {
        openSocketPos = (Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition) +
                        limbValues.originalParent.transform.position;
        
        this.transform.parent = limbValues.originalParent.transform;
        this.transform.position = openSocketPos;
                    
        attached = true;
        OnLimbStatusChanged(this);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var newPos = Quaternion.Euler(limbValues.originalParent.transform.eulerAngles) * limbValues.originalLocalPosition;
        Gizmos.DrawWireSphere(limbValues.originalParent.transform.position + newPos, 0.025f);
    }
}
