using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private string levelDataPath = "Assets/Arts/Levels/level_{0:00}.json";
    [SerializeField] private int maxLevel = 10;

    private const string CurrentLevelKey = "CurrentLevel";

    private int currentLevel = 1;
    private int storedLevel = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            storedLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
            currentLevel = Mathf.Clamp(storedLevel, 1, maxLevel);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static LevelManager GetLevelManager() { return instance; }

    public int MaxLevel => maxLevel;

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetStoredLevel()
    {
        return storedLevel;
    }

    public bool HasCompletedAllLevels()
    {
        return storedLevel > maxLevel;
    }

    public void SetCurrentLevel(int level)
    {
        storedLevel = level;
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
    }

    public Level LoadLevel(int levelnum)
    {
        currentLevel = Mathf.Clamp(levelnum, 1, maxLevel);
        string filePath = string.Format(levelDataPath, currentLevel);
        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file not found at: " + filePath);
            return null;
        }

        string jsonContent = File.ReadAllText(filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(jsonContent);

        return new Level(
            data.level_number,
            data.grid_width,
            data.grid_height,
            data.move_count,
            data.pattern,
            new List<string>(data.grid)
        );
    }

    public void SaveLevelProgress()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, storedLevel);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class LevelData
    {
        public int level_number;
        public int grid_width;
        public int grid_height;
        public int move_count;
        public string pattern;
        public string[] grid;
    }
}
