using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public Transform interactionTransform;
    bool isFocused = false;
    Transform player;

    bool hasInteracted = false;

    public virtual void Interact()
    {

    }


    public void Update()
    {
        if (isFocused)
        {
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            if (!hasInteracted && distance <= radius)
            {
                hasInteracted = true;
                Interact();
            }
        }
    }


    public void OnFocused(Transform playerTransform)
    {
        isFocused = true;
        hasInteracted = false;
        player = playerTransform;
        
    }

    public void OnDefocused()
    {
        isFocused = false;
        hasInteracted = false;
        player = null;
        
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
