using System;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private Color originalColor;
    private BoxCollider rayCastPlaneCollider;
    public bool attached = true;
    private GameObject originalParent;
    private Vector3 originalLocalPosition;
    private Renderer meshRenderer;
    private bool isBeingDragged = false;
    private Vector3 openSocketPos;

    private float snapDistance = 0.005f;
    

    private void Start()
    {
        originalColor = this.GetComponent<Renderer>().material.color;
        originalParent = this.transform.parent.gameObject;
        originalLocalPosition = this.transform.localPosition;
        rayCastPlaneCollider = CoreController.Instance.raycastPlane.GetComponent<BoxCollider>();
        meshRenderer = this.GetComponent<Renderer>();
    }
    
    private void OnMouseEnter()
    {
        HighlightAllChildren();
    }

    private void OnMouseDrag()
    {
        if (!isBeingDragged)
            isBeingDragged = true;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (rayCastPlaneCollider.Raycast(ray, out hit, 10))
        {
            // This is calculating the local socket position every frame when the mouse is dragging.
            // TODO: This is inefficient as we should only be checking the distance itself, fix it
            openSocketPos = (Quaternion.Euler(originalParent.transform.eulerAngles) * originalLocalPosition) +
                                originalParent.transform.position;

            // Calculating distance as a 2d length against the raycastPanel to get around the depth issue.
            // This only works because our camera is fixed.
            Vector3 distance = new Vector3(hit.point.x, hit.point.y, 0) -
                               new Vector3(openSocketPos.x, openSocketPos.y, 0);
            
            if (attached)
            {
                if (distance.sqrMagnitude > snapDistance)
                {
                    this.transform.parent = null;

                    attached = false;
                    CoreController.Instance.attachmentStatusChanged.Invoke(this.name, attached);
                }
            } 
            else
            {
                if (distance.sqrMagnitude < snapDistance)
                {
                    this.transform.parent = originalParent.transform;
                    this.transform.position = openSocketPos;
                    
                    attached = true;
                    CoreController.Instance.attachmentStatusChanged.Invoke(this.name, attached);
                }
                else
                {
                    this.transform.position = hit.point;
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (isBeingDragged)
            isBeingDragged = false;
        
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform limb in this.GetComponentsInChildren<Transform>())
        {
            // limb.GetComponent<SelectionController>().ToggleHighlight();
            limb.GetComponent<SelectionController>().Unhighlight();
        }
    }

    private void OnMouseExit()
    {
        if(isBeingDragged)
            return;
        
        UnHighlightAllChildren();
    }

    private void HighlightAllChildren()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (SelectionController limb in this.GetComponentsInChildren<SelectionController>())
        {
            limb.Highlight();
        }
    }

    private void Highlight()
    {
        meshRenderer.material.color = Color.yellow;
    }

    private void UnHighlightAllChildren()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (SelectionController limb in this.GetComponentsInChildren<SelectionController>())
        {
            limb.Unhighlight();
        }
    }
    
    private void Unhighlight()
    {
        meshRenderer.material.color = originalColor;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var newPos = Quaternion.Euler(originalParent.transform.eulerAngles) * originalLocalPosition;
        Gizmos.DrawWireSphere(originalParent.transform.position + newPos, 0.025f);
    }
}
