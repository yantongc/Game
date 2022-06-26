using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    Text levelText;

    Image healthSlider;

    Image expSlider;

    private void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();

        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        if (GameManager.Instance.playerStats.charactorData == null) return;
        levelText.text = "Level: " + GameManager.Instance.playerStats.charactorData.curLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateHealth()
    {
        float percent = GameManager.Instance.playerStats.CurHp / GameManager.Instance.playerStats.MaxHp;
        healthSlider.fillAmount = percent;
    }

    private void UpdateExp()
    {
        float percent = (float)GameManager.Instance.playerStats.charactorData.curExp / GameManager.Instance.playerStats.charactorData.baseExp;
        expSlider.fillAmount = percent;
    }
}
