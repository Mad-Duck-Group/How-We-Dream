using System.Collections;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoSingleton<MainMenuManager>
{
    [SerializeField] private Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        GlobalSoundManager.Instance.PlayBGM("MainMenuBGM", duration: 0.5f);
    }

    private void StartGame()
    {
        GlobalSoundManager.Instance.PlayUISFX("StartGame");
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.GamePlay, LoadSceneMode.Additive, false);
    }
}
