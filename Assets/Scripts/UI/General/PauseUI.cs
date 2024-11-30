using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseCanvasGroup;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    private bool isPaused;
    private bool allowPause;
    private Tween fadeTween;

    private void OnEnable()
    {
        LevelManager.OnLevelStart += OnLevelStart;
    }
    
    private void OnDisable()
    {
        LevelManager.OnLevelStart -= OnLevelStart;
    }

    private void OnLevelStart()
    {
        allowPause = true;
    }

    void Start()
    {
        pauseCanvasGroup.gameObject.SetActive(false);
        resumeButton.onClick.AddListener(Unpause);
        mainMenuButton.onClick.AddListener(ToMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowPause) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        if (fadeTween.IsActive()) fadeTween.Kill();
        pauseCanvasGroup.gameObject.SetActive(true);
        pauseCanvasGroup.alpha = 0;
        fadeTween = pauseCanvasGroup.DOFade(1, 0.5f).SetUpdate(true);
        pauseCanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0;
    }
    
    private void Unpause()
    {
        isPaused = false;
        if (fadeTween.IsActive()) fadeTween.Kill();
        pauseCanvasGroup.blocksRaycasts = false;
        fadeTween = pauseCanvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => pauseCanvasGroup.gameObject.SetActive(false));
        Time.timeScale = 1;
    }
    
    private void ToMainMenu()
    {
        Time.timeScale = 1;
        pauseCanvasGroup.blocksRaycasts = false;
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.MainMenu, LoadSceneMode.Additive, false);
    }
}
