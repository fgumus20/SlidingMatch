using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private int currentLevel = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static LevelManager GetLevelManager() { return instance; }
    public int GetCurrentLevel() { return PlayerPrefs.GetInt("CurrentLevel", 1); }
    public void SetCurrentLevel(int level) { currentLevel = level; }

    public Level LoadLevel(int levelnum)
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        string levelName = currentLevel < 10 ? $"Levels/level_0{currentLevel}" : "Levels/level_10";

        TextAsset jsonFile = Resources.Load<TextAsset>(levelName);


        LevelData data = JsonUtility.FromJson<LevelData>(jsonFile.text);

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
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
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
