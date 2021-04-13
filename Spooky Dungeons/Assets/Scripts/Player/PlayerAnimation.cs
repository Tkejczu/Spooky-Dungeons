using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    private bool walking;
    CharacterCombat combat;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CharacterCombat>();
        combat.OnAttack += OnAttack;
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            walking = false;
        }
        else
        {
            walking = true;
        }
        animator.SetBool("isWalking", walking);
    }

    protected virtual void OnAttack()
    {

        animator.SetTrigger("attack");
    }

}
