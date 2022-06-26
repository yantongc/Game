using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockState { HitPlayer, HitEnemy, HitNothing }
    public RockState rockState;

    public GameObject breakEffect;

    private Rigidbody rb;
    private Vector3 direction;


    [Header("Basic Setting")]
    public float force;

    public int damage;

    public GameObject target;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;

        rockState = RockState.HitPlayer;
        FlyToTarget();
    }
    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockState = RockState.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerManager>().gameObject;
        }
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(rockState)
        {
            case RockState.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    collision.gameObject.GetComponent<Animator>().SetTrigger("IsDizzy");
                    collision.gameObject.GetComponent<CharectorState>().TakeDamage(damage, collision.gameObject.GetComponent<CharectorState>());
                    //fixme 粒子系統沒有銷毀
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    rockState = RockState.HitNothing;
                }
                break;
            case RockState.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())
                {
                    var otherState = collision.gameObject.GetComponent<CharectorState>();
                    otherState.TakeDamage(damage, otherState);
                    //fixme 粒子系統沒有銷毀
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    rockState = RockState.HitNothing;
                }
                break;
            case RockState.HitNothing:
                break;
        }
    }
}
