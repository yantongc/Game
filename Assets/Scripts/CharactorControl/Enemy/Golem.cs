using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyManager
{
    [Header("Skill")]
    public float kickForce = 20;

    public GameObject rockPrefabs;

    public Transform handPos;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null)
        {
            var targetSatats = attackTarget.GetComponent<CharectorState>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            NavMeshAgent navMeshAgent = attackTarget.GetComponent<NavMeshAgent>();
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = direction * kickForce;
            targetSatats.TakeDamage(charectorState, targetSatats);
            attackTarget.GetComponent<Animator>().SetTrigger("IsDizzy");
        }
    }

    public void ThrowRock()
    {
        if (attackTarget!=null)
        {
            var rock = Instantiate(rockPrefabs, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
