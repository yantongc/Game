using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{GUARD,PATROL,CHASE,DEAD};

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharectorState))]
public class EnemyManager : MonoBehaviour, IEndGameObserver
{
    protected CharectorState charectorState;
    protected GameObject attackTarget;
    
    private NavMeshAgent agent;
    private Animator anim;
   
    private Collider coll;
    private Vector3 originBornPoint;
    private Vector3 curPatrolPoint;
    private Quaternion guardRotation;
    private EnemyState enemyState;
    private float speed;
    private float curTime;
    private float lastAttackTime;
    private bool isSelected;
    private bool isNear;
    private bool isWalk;
    private bool isChase;
    private bool isDead;
    private bool isPlayerDead;

    [Header("Base Settings")]
    public float sightRadius;
    public bool isGuard;
    public float lookAtTime;

    [Header("Patrol State")]
    public float patrolRange;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        charectorState = GetComponent<CharectorState>();
        coll = GetComponent<Collider>();
    }

    void Start()
    {
        curTime = 0;
        lastAttackTime = 0;
        speed = agent.speed;
        originBornPoint = transform.position;
        guardRotation = transform.rotation;
        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL;
        GetNewPatrolPoint();
    }

    private void OnEnable()
    {
        GameManager.Instance.AddAbserver(this);
        BattleManager.Instance.OnAttackEnemyChange += OnAttackEnemyChange;
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveAbserver(this);
        BattleManager.Instance.OnAttackEnemyChange -= OnAttackEnemyChange;
    }

    void Update()
    {
        if (charectorState.CurHp<=0)
        {
            isDead = true;
        }
        if (isPlayerDead)
        {
            return;
        }
        transform.GetChild(3).gameObject.SetActive(isSelected);
        SwitchState();
        SwitchAnimations();
        lastAttackTime -= Time.deltaTime;
    }

    public void EndNotify()
    {
        anim.SetBool("Win", true);
        isPlayerDead = true;
        isNear = false;
        isChase = false;
        attackTarget = null;

    }

    private void SwitchAnimations()
    {
        anim.SetBool("Near", isNear);
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Critical", charectorState.isCritical);
        anim.SetBool("Death", isDead);
    }

    private void SwitchState()
    {
        if (isDead)
        {
            enemyState = EnemyState.DEAD;
        }
        else if (FindPlayer())
        {
            enemyState = EnemyState.CHASE;
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                agent.speed = speed*0.5f;
                isChase = false;
                if (transform.position!= originBornPoint)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = originBornPoint;

                    if (Vector3.SqrMagnitude(originBornPoint - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
                    }
                }
                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 3.5f;
                if (Vector3.Distance(transform.position, curPatrolPoint)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (curTime<lookAtTime)
                    {
                        curTime += Time.deltaTime;
                    }
                    else
                    {
                        curTime = 0;
                        GetNewPatrolPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = curPatrolPoint;
                }
                break;
            case EnemyState.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (attackTarget)
                {
                    isNear = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                else
                {
                    isNear = false;
                    if (curTime < lookAtTime)
                    {
                        curTime += Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else
                    {
                        curTime = 0;
                        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL;
                    }
                }

                //¹¥»÷·¶Î§ÄÚ
                if (TargetInAttackRange()||TargetInSkillRange())
                {
                    isNear = false;
                    agent.isStopped = true;
                    if (lastAttackTime<=0)
                    {
                        lastAttackTime = charectorState.attackData.cdTime;

                        charectorState.isCritical = UnityEngine.Random.value < charectorState.attackData.criticalRate;

                        Attack();
                    }
                }
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                if (BattleManager.Instance.CurAttackEnemy&&BattleManager.Instance.CurAttackEnemy.Equals(gameObject))
                {
                    BattleManager.Instance.CurAttackEnemy = null;
                }
                Destroy(gameObject, 2f);
                break;
        }
    }

    private void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInSkillRange())
        {
            //¼¼ÄÜ¹¥»÷
            anim.SetTrigger("Skill");
        }
        else if(TargetInAttackRange())
        {
            //ÆÕÍ¨¹¥»÷
            if (UnityEngine.Random.value > 0.5f)
            {
                anim.SetTrigger("Attack");
            }
            else
            {
                anim.SetTrigger("Skill");
            };
        }
    }

    private bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= charectorState.attackData.attackRange;
        }
        return false;
    }

    private bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= charectorState.attackData.skillRange;
        }
        return false;
    }

    private bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

    private void GetNewPatrolPoint()
    {
        float xValue = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float zValue = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 newPos = new Vector3(originBornPoint.x + xValue, transform.position.y, originBornPoint.z + zValue);
        NavMeshHit hit;
        curPatrolPoint = NavMesh.SamplePosition(newPos,out hit, patrolRange,1) ? newPos: transform.position;
    }

    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetSatats = attackTarget.GetComponent<CharectorState>();
            targetSatats.TakeDamage(charectorState, targetSatats);
        }
    }

    private void OnAttackEnemyChange(GameObject obj)
    {
        isSelected = true;
    }
}
