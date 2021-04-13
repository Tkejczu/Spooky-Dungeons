using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    public Interactable focus;
    PlayerAction action;
    void Start()
    {
        action = GetComponent<PlayerAction>();
    }

    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                action.MoveToPoint(hit.point);
                RemoveFocus();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }

        void SetFocus(Interactable newFocus)
        {
            if (newFocus != focus)
            {
                if (focus !=null)
                    focus.OnDefocused();

                focus = newFocus;
                action.FollowTarget(newFocus);
            }

            newFocus.OnFocused(transform);
            
        }

        void RemoveFocus()
        {
            if (focus != null)
                focus.OnDefocused();

            focus = null;
            action.StopFollowingTarget();
        }

    }      
}