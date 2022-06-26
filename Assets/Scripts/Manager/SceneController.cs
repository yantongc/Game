using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public enum SceneName { MainScene, VillageScene, DesertScene }

    public GameObject playerPrefabs;

    public SceneFader sceneFaderpfb;

    bool faderFinish;

    GameObject player;

    NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        faderFinish = true;
        GameManager.Instance.AddAbserver(this);
    }

    public void TransitionNewGame()
    {
        StartCoroutine(LoadGameSceneByName("VillageScene"));
    }

    public void TransitionResumeGame()
    {
        StartCoroutine(LoadGameSceneByName(SaveManager.Instance.SceneName));
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMainScene());
    }

    public void TransitionToDestination(EntrancePortal entrance)
    {
        switch (entrance.portalType)
        {
            case EntrancePortal.PortalType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, entrance.destinationTag));
                break;
            case EntrancePortal.PortalType.DifferentScene:
                StartCoroutine(Transition(entrance.connectSceneName, entrance.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, DestinationPortal.DestinationTag destinationTag)
    {
        //±£´æÊý¾Ý
        SaveManager.Instance.SavePlayerData();

        if (sceneName.Equals(SceneManager.GetActiveScene().name))
        {
            player = GameManager.Instance.playerStats.gameObject;
            navMeshAgent = player.GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);
            navMeshAgent.enabled = true;
            yield return null;
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefabs, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
    }

    private DestinationPortal GetDestination(DestinationPortal.DestinationTag destinationTag)
    {
        var destination = FindObjectsOfType<DestinationPortal>();
        for (int i = 0; i < destination.Length; i++)
        {
            if (destination[i].destinationTag == destinationTag)
            {
                return destination[i];
            }
        }
        return null;
    }

    IEnumerator LoadGameSceneByName(string scenename)
    {
        SceneFader fader = Instantiate<SceneFader>(sceneFaderpfb);
        if (scenename != "")
        {
            yield return StartCoroutine(fader.SceneFadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scenename);
            yield return player = Instantiate(playerPrefabs, GameManager.Instance.GetEntrance(scenename).position, GameManager.Instance.GetEntrance(scenename).rotation);
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fader.SceneFadeIn(2f));
            yield break;
        }
    }

    IEnumerator LoadMainScene()
    {
        SceneFader fader = Instantiate<SceneFader>(sceneFaderpfb);
        yield return StartCoroutine(fader.SceneFadeOut(2f));
        yield return SceneManager.LoadSceneAsync("MainScene");
        yield return StartCoroutine(fader.SceneFadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (faderFinish)
        {
            faderFinish = false;
            PlayerPrefs.DeleteAll();
            StartCoroutine(LoadMainScene());
        }
    }
}
