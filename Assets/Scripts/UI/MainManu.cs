using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class MainManu : MonoBehaviour
{
    Button newGame;
    Button resumeGame;
    Button quitGame;

    PlayableDirector director;

    private void Awake()
    {
        newGame = transform.GetChild(0).GetComponent<Button>();
        resumeGame = transform.GetChild(1).GetComponent<Button>();
        quitGame = transform.GetChild(2).GetComponent<Button>();

        newGame.onClick.AddListener(playStartTimelineMovie);
        resumeGame.onClick.AddListener(ResumeGame);
        resumeGame.interactable = PlayerPrefs.GetFloat("hasSaveDataInFile") > 0;

        quitGame.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
    }

    void playStartTimelineMovie()
    {
        director.Play();
        director.stopped += NewGame;
    }

    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionNewGame();
    }

    void ResumeGame()
    {
        SceneController.Instance.TransitionResumeGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
