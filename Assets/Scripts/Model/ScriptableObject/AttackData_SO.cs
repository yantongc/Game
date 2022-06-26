using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Data",menuName = "Model/AttackData")]
public class AttackData_SO :ScriptableObject
{
    public float attackRange;

    public float skillRange;

    public float cdTime;

    public float minDamage;

    public float maxDamage;

    public float criticalMultiplier;

    public float criticalRate;
}
