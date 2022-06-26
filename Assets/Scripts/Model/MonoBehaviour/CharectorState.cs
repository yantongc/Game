using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharectorState : MonoBehaviour
{
    public event Action<float, float> UpdateHealthBarOnAttack;

    public string charactorName;

    public CharactorData_SO templateData;
    public CharactorData_SO charactorData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if (templateData!=null)
        {
            charactorData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    public float MaxHp
    {
        get{return charactorData != null ? charactorData.maxHp : 0;}
        set{charactorData.maxHp = value;}
    }

    public float CurHp
    {
        get { return charactorData != null ? charactorData.curHp : 0; }
        set { charactorData.curHp = value; }
    }


    public float Defense
    {
        get { return charactorData != null ? charactorData.defense : 0; }
        set { charactorData.defense = value; }
    }

    public float CurDefense
    {
        get { return charactorData != null ? charactorData.curDefense : 0; }
        set { charactorData.curDefense = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharectorState attackers, CharectorState defener)
    {
        float damage = Mathf.Max(0, attackers.CurDamage() - defener.CurDefense);
        CurHp = Mathf.Max(0, CurHp - damage);

        if (attackers.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("IsHit");
        }
        //更新血量显示
        UpdateHealthBarOnAttack?.Invoke(CurHp,MaxHp);
        //更新经验显示
        if (CurHp <= 0)
        {
            attackers.charactorData.UpdateExp(charactorData.expPoint);
        }
    }

    public void TakeDamage(int damage, CharectorState defener)
    {
        float currentDamage = Mathf.Max(0, damage - defener.CurDefense);
        CurHp = Mathf.Max(0, CurHp - currentDamage);
        defener.GetComponent<Animator>().SetTrigger("IsHit");
        UpdateHealthBarOnAttack?.Invoke(CurHp, MaxHp);
        //更新经验显示
        if (CurHp <= 0)
        {
            GameManager.Instance.playerStats.charactorData.UpdateExp(charactorData.expPoint);
        }
    }

    private float CurDamage()
    {
        float coreDamege = UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);

        return isCritical?coreDamege*attackData.criticalMultiplier: coreDamege;
    }
    #endregion
}
