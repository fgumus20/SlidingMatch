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

    [Header("Level Selection")]
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private RectTransform levelButtonsParent;
    [SerializeField] private GameObject levelButtonTemplate;
    [SerializeField] private Button closeLevelsButton;
    [SerializeField] private int levelButtonsPerRow = 5;
    [SerializeField] private Vector2 levelButtonSpacing = new Vector2(24f, 24f);
    [SerializeField] private Vector2 levelButtonSize = new Vector2(140f, 70f);
    [SerializeField] private int fallbackMaxLevel = 10;

    private bool levelButtonsCreated = false;

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

        InitializeLevelPanel();
    }

    void Start()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(false);
        }

        UpdateText();
    }

    void InitializeLevelPanel()
    {
        if (levelButtonTemplate != null)
        {
            levelButtonTemplate.SetActive(false);
        }

        if (closeLevelsButton != null)
        {
            closeLevelsButton.onClick.RemoveAllListeners();
            closeLevelsButton.onClick.AddListener(HideLevelPanel);
        }
    }

    public void UpdateText()
    {
        if (buttonText == null)
        {
            return;
        }

        var textComponent = buttonText.GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
        {
            return;
        }

        LevelManager levelManager = LevelManager.instance;
        int storedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int maxLevel = levelManager != null ? levelManager.MaxLevel : fallbackMaxLevel;

        if ((levelManager != null && levelManager.HasCompletedAllLevels()) ||
            (levelManager == null && storedLevel > maxLevel))
        {
            textComponent.text = "Finished";
            return;
        }

        int level = levelManager != null ? levelManager.GetCurrentLevel() : Mathf.Clamp(storedLevel, 1, maxLevel);
        textComponent.text = $"Level {level}";
    }

    public void SceneLoader(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void ShowLevelPanel()
    {
        if (levelPanel == null)
        {
            return;
        }

        EnsureLevelButtons();
        levelPanel.SetActive(true);
    }

    public void HideLevelPanel()
    {
        if (levelPanel == null)
        {
            return;
        }

        levelPanel.SetActive(false);
    }

    void EnsureLevelButtons()
    {
        if (levelButtonsCreated)
        {
            return;
        }

        if (levelButtonsParent == null || levelButtonTemplate == null)
        {
            return;
        }

        levelButtonsCreated = true;

        RectTransform templateRect = levelButtonTemplate.GetComponent<RectTransform>();
        Vector2 elementSize = levelButtonSize;
        if (elementSize == Vector2.zero && templateRect != null)
        {
            elementSize = templateRect.sizeDelta;
        }
        if (elementSize == Vector2.zero)
        {
            elementSize = new Vector2(140f, 70f);
        }

        int totalLevels = fallbackMaxLevel;
        if (LevelManager.instance != null)
        {
            totalLevels = Mathf.Max(1, LevelManager.instance.MaxLevel);
        }

        int columns = Mathf.Max(1, levelButtonsPerRow);
        int rows = Mathf.CeilToInt(totalLevels / (float)columns);

        Vector2 start = new Vector2(
            -(columns - 1) * (elementSize.x + levelButtonSpacing.x) * 0.5f,
            (rows - 1) * (elementSize.y + levelButtonSpacing.y) * 0.5f
        );

        for (int i = 0; i < totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonTemplate, levelButtonsParent);
            buttonObj.name = $"LevelButton_{i + 1}";
            buttonObj.SetActive(true);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = elementSize;
                int column = i % columns;
                int row = i / columns;
                rect.anchoredPosition = new Vector2(
                    start.x + column * (elementSize.x + levelButtonSpacing.x),
                    start.y - row * (elementSize.y + levelButtonSpacing.y)
                );
            }

            TextMeshProUGUI label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"Level {i + 1}";
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                int levelIndex = i + 1;
                button.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
            }
        }
    }

    void OnLevelButtonClicked(int level)
    {
        LevelManager levelManager = LevelManager.instance;
        if (levelManager != null)
        {
            levelManager.SetCurrentLevel(level);
            levelManager.SaveLevelProgress();
        }
        UpdateText();
        SceneManager.LoadScene(1);
    }
}
