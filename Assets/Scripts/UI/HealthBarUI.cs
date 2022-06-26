using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarPrefabs;

    public Transform barPoint;

    public bool isAlwaysVisible;

    public float visibleTime;

    private Image healthSlider;

    private Transform UIbar;

    private  Transform cameraPoint;

    private CharectorState currentStats;

    private float disapearTimeCount;

    private void Awake()
    {
        currentStats = GetComponent<CharectorState>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cameraPoint = Camera.main.transform;

        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.name.Equals("HealthBarCanvas"))
            {
                UIbar = Instantiate(healthBarPrefabs, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(isAlwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(float curHp, float maxHp)
    {
        if (curHp<=0)
        {
            Destroy(UIbar.gameObject);
            return;
        }
        UIbar.gameObject.SetActive(true);

        disapearTimeCount = visibleTime;

        float sliderPercent = curHp / maxHp;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar!=null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cameraPoint.forward;

            if (disapearTimeCount<=0&& !isAlwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                disapearTimeCount -= Time.deltaTime;
            }
        }
    }
}
