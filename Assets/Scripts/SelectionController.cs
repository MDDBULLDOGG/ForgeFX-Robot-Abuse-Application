using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public ObjectController sceneObjectController;
    
    private Color originalColor;
    private BoxCollider rayCastPlaneCollider;
    private bool highlighted;
    private bool detached;
    private GameObject originalParent;
    private Vector3 originalLocalPosition;

    private float snapDistance = 0.005f;
    

    private void Start()
    {
        originalColor = this.GetComponent<Renderer>().material.color;
        originalParent = this.transform.parent.gameObject;
        originalLocalPosition = this.transform.localPosition;
        rayCastPlaneCollider = CoreController.Instance.raycastPlane.GetComponent<BoxCollider>();
    }
    
    private void OnMouseEnter()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in this.GetComponentsInChildren<Transform>())
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }

    private void OnMouseDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (rayCastPlaneCollider.Raycast(ray, out hit, 10))
        {
            // This is calculating the local socket position every frame when the mouse is dragging.
            // TODO: This is inefficient as we should only be checking the distance itself, fix it
            var openSocketPos = (Quaternion.Euler(originalParent.transform.eulerAngles) * originalLocalPosition) +
                                originalParent.transform.position;

            // Calculating distance as a 2d length against the raycastPanel to get around the depth issue.
            // This only works because our camera is fixed.
            Vector3 distance = new Vector3(hit.point.x, hit.point.y, 0) -
                               new Vector3(openSocketPos.x, openSocketPos.y, 0);
            
            if (detached)
            {
                if (distance.sqrMagnitude < snapDistance)
                {
                    detached = false;
                    this.transform.parent = originalParent.transform;
                    this.transform.position = openSocketPos;
                }
                else
                {
                    this.transform.position = hit.point;
                }
            }

            if (detached == false)
            {
                if (distance.sqrMagnitude > snapDistance)
                {
                    detached = true;
                    this.transform.parent = null;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        // Since the children will be following the parent we want to highlight/unhighlight all children
        foreach (Transform child in this.GetComponentsInChildren<Transform>())
        {
            child.GetComponent<SelectionController>().ToggleHighlight();
        }
    }

    private void ToggleHighlight()
    {
        if (highlighted)
        {
            this.GetComponent<Renderer>().material.color = originalColor;
            highlighted = false;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.yellow;
            highlighted = true;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var newPos = Quaternion.Euler(originalParent.transform.eulerAngles) * originalLocalPosition;
        Gizmos.DrawWireSphere(originalParent.transform.position + newPos, 0.025f);
    }
}
