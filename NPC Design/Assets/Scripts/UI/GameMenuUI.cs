using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private bool openOnAwake = false;
    [SerializeField] private bool cannotBeClosed = false;

    private CanvasGroup canvasGroup;

    private bool alreadyShowedMenuOnPlayerDead = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            Debug.LogWarning("Canvas Group component not found on UI menu: " + name + ". UI Menu disabled!");
            gameObject.SetActive(false);
            return;
        }

        if (openOnAwake || cannotBeClosed)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnEnable()
    {
        PlayerDead.OnPlayerDead += ShowGameMenuOnPlayerDead;
    }

    private void OnDisable()
    {
        PlayerDead.OnPlayerDead -= ShowGameMenuOnPlayerDead;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cannotBeClosed) return;

            if(canvasGroup.alpha == 1f)
            {
                ShowGameMenu(false);
                return;
            }

            ShowGameMenu(true);
        }
    }

    public void ShowGameMenu(bool show)
    {
        if (show)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            Time.timeScale = 0f;
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;
    }

    public void ShowGameMenuOnPlayerDead()
    {
        if (alreadyShowedMenuOnPlayerDead) return;

        StartCoroutine(ShowGameMenuOnPlayerDeadCoroutine());
    }

    private IEnumerator ShowGameMenuOnPlayerDeadCoroutine()
    {
        yield return new WaitForSeconds(5f);

        ShowGameMenu(true);
        alreadyShowedMenuOnPlayerDead = true;
    }

    public void OnStartBossFightClicked(string sceneName)
    {
        if(sceneName == string.Empty)
        {
            Debug.LogWarning("Scene name is empty at: " + name);
            return;
        }

        if(Time.timeScale < 1f) Time.timeScale = 1f;
        Scene bossFightSc = SceneManager.GetSceneByName(sceneName);
        if (bossFightSc == null)
        {
            Debug.LogWarning("Scene Based On Scene Name not found!");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        Scene mainMenu = SceneManager.GetSceneByName("MainMenu");
        if (mainMenu == null) return;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRestartButtonClick()
    {
        Time.timeScale = 1f;
        cannotBeClosed = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
