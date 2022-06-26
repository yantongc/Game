using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data",menuName = "Model/CharactorData")]
public class CharactorData_SO : ScriptableObject
{
    [Header("Base Info")]
    public float maxHp;

    public float curHp;

    public float defense;

    public float curDefense;

    [Header("Loot Info")]
    public int expPoint;

    [Header("Level Info")]
    public int curLevel;

    public int maxLevel;

    public int curExp;

    public int baseExp;

    public float levelBuff;

    public float LevelMutiplier
    {
        get { return 1 + (curLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int value)
    {
        curExp += value;
        if (curExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (curLevel<maxLevel)
        {
            curExp = curExp - baseExp;
            curLevel++;
            maxHp += (int)(20 * levelBuff * LevelMutiplier);
            curHp = maxHp;
            defense++;
            baseExp += (int)(baseExp * LevelMutiplier);
            Debug.Log("curLevel"+ curLevel+ "maxHp"+ maxHp+ "baseExp"+ baseExp);
        }
    }
}
