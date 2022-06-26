using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private CharectorState charectorState;

    private int attackCount;
    private bool isRotating;
    private bool isDead;

    public float speed;

    private GameObject curTarget;
    private float lastAttackTime;
    private float stopDistance;

    Vector3 movement;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        charectorState = GetComponent<CharectorState>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClick += MoveToTarget;
        MouseManager.Instance.OnEnemyClick += EventAttack;
        GameManager.Instance.RigisterPlayer(charectorState);
    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClick -= MoveToTarget;
        MouseManager.Instance.OnEnemyClick -= EventAttack;
    }

    private void Update()
    {
        isDead = charectorState.CurHp <= 0;
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }
        AnimationUpdate();
        lastAttackTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && BattleManager.Instance.CurAttackEnemy != null)
        {
            EventAttack(BattleManager.Instance.CurAttackEnemy);
        }

        //自由转向
        RotationManage();
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.isStopped = false;
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(target);
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            BattleManager.Instance.CurAttackEnemy = target;
            charectorState.isCritical = UnityEngine.Random.value < charectorState.attackData.criticalRate;
            StartCoroutine(MoveToEnemyTarget());
        }
    }

    IEnumerator MoveToEnemyTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = charectorState.attackData.attackRange;
        transform.LookAt( BattleManager.Instance.CurAttackEnemy.transform);

        while (Vector3.Distance(BattleManager.Instance.CurAttackEnemy.transform.position, transform.position) > charectorState.attackData.attackRange)
        {
            agent.destination = BattleManager.Instance.CurAttackEnemy.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", charectorState.isCritical);
            anim.SetTrigger("IsAttacking");
            lastAttackTime = charectorState.attackData.cdTime;
        }
    }

    void AnimationUpdate()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    void Hit()
    {
        var curTarget = BattleManager.Instance.CurAttackEnemy;
        if (curTarget == null)
        {
            return;
        }
        if (curTarget.CompareTag("Attackable"))
        {
            if (curTarget.GetComponent<Rock>())
            {
                curTarget.GetComponent<Rock>().rockState = Rock.RockState.HitEnemy;
                curTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                curTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetSatats = curTarget.GetComponent<CharectorState>();

            targetSatats.TakeDamage(charectorState, targetSatats);
        }
    }

    void RotationManage()
    {
        isRotating = Input.GetMouseButton(1);

        if (isRotating)
        {
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up, fMouseX * Time.deltaTime* 1000f, Space.Self);
        }
    }
}
