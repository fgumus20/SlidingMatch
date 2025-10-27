using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    public GameObject buttonText;

    public GameObject levelsPanel;
    public GameObject levelButtonPrefab;
    public Transform levelsContainer;
    public GameObject playButton;
    public GameObject levelsButton;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateText();
        
    }
    public void UpdateText()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel",1);
        
        if (level > 10)
        {
            buttonText.GetComponent<TextMeshPro>().text = "Finished ";
        }
        else
        {
            buttonText.GetComponent<TextMeshPro>().text = "Level " + level.ToString();

        }
    }
    public void SceneLoader(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void OpenLevelsPanel()
    {
        levelsPanel.SetActive(true);
        playButton.SetActive(false);
        levelsButton.SetActive(false);
        PopulateLevelButtons();

        var rect = levelsPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.one * 0.8f;
        rect.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }
    public void CloseLevelsPanel()
    {
        var rect = levelsPanel.GetComponent<RectTransform>();
        rect.DOScale(0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() => { 

            levelsPanel.SetActive(false);
            playButton.SetActive(true);
            levelsButton.SetActive(true);
        }); 

    }
    public void LoadLevel(int levelIndex)
    {
        LevelManager.instance.SetCurrentLevel(levelIndex);
        LevelManager.instance.SaveLevelProgress();
        SceneManager.LoadScene(1);
    }

    public void PopulateLevelButtons()
    {
        foreach (Transform child in levelsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= 5; i++)
        {
            GameObject btn = Instantiate(levelButtonPrefab, levelsContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            int levelIndex = i;
            btn.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }


}
