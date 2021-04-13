using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    CharacterCombat combat;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        combat = GetComponent<CharacterCombat>();
    }

  
    void Update()
    {
        if (target == null)
            target = PlayerManager.instance.player.transform;   
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= lookRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            if (distance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", true);
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                if(targetStats != null)
                {
                    combat.Attack(targetStats);
                }
                FaceTarget();
            }
        }        
        else
        {
            agent.isStopped = true;
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
