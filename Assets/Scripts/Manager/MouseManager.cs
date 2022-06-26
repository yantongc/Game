using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseManager : Singleton<MouseManager>
{
   
    public Texture2D point, arrow, attack, entrance, target;

    RaycastHit HitInfo;
    public event Action<Vector3> OnMouseClick;
    public event Action<GameObject> OnEnemyClick;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetMouseTexture();
        MouseControl();
    }

    private void SetMouseTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out HitInfo))
        {
            switch (HitInfo.collider.gameObject.tag)
            {
                case "Ground":
                Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(12, 0), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(0, 0), CursorMode.Auto);
                    break;
                case "Entrance":
                    Cursor.SetCursor(entrance, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    private void MouseControl()
    {
        //Å×³öÊÂ¼þ
        if (Input.GetMouseButtonDown(0)&& HitInfo.collider!=null)
        {
            if (HitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClick?.Invoke(HitInfo.point);
            }

            if (HitInfo.collider.gameObject.CompareTag("Entrance"))
            {
                OnMouseClick?.Invoke(HitInfo.point);
            }

            if (HitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClick?.Invoke(HitInfo.collider.gameObject);
            }

            if (HitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClick?.Invoke(HitInfo.collider.gameObject);
            }
        }
    }
}
