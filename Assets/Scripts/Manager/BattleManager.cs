using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    private GameObject curAttackEnemy;

    public GameObject CurAttackEnemy
    {
        get { return curAttackEnemy; }
        set { curAttackEnemy = value; OnAttackEnemyChange.Invoke(curAttackEnemy); } }

    public event Action<GameObject> OnAttackEnemyChange;

    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
