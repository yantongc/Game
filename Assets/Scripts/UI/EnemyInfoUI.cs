using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoUI : MonoBehaviour
{
    Text enemyName;

    Image enemyHpSlider;

    private void Awake()
    {
        enemyName = transform.GetChild(0).GetComponent<Text>();
        enemyHpSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        if (BattleManager.Instance.CurAttackEnemy != null)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            var charectorState = BattleManager.Instance.CurAttackEnemy.GetComponent<CharectorState>();
            enemyName.text = charectorState.charactorName;
            var percent = charectorState.charactorData.curHp / charectorState.charactorData.maxHp;
            enemyHpSlider.fillAmount = percent;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
