using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharectorState playerStats;

    private CinemachineFreeLook freeLook;


    List<IEndGameObserver> endGameObserver = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharectorState player)
    {
        playerStats = player;

        freeLook = FindObjectOfType<CinemachineFreeLook>();
        if (freeLook != null)
        {
            freeLook.Follow = playerStats.transform.GetChild(0);
            freeLook.LookAt = playerStats.transform.GetChild(3);
        }
    }

    public void AddAbserver(IEndGameObserver observer)
    {
        endGameObserver.Add(observer);
    }

    public void RemoveAbserver(IEndGameObserver observer)
    {
        endGameObserver.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObserver)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance(string scenename)
    {
        foreach (var item in FindObjectsOfType<DestinationPortal>())
        {
            if (item && item.destinationTag == DestinationPortal.DestinationTag.Enter)
            {
                return item.transform;
            }
        }
        return null;
    }
}
