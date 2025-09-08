using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private string levelDataPath = "Assets/CaseStudyAssetsNoArea/Levels/level_0{0}.json";
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
    public int GetCurrentLevel() {return  PlayerPrefs.GetInt("CurrentLevel", 1); }
    public void SetCurrentLevel(int level) { currentLevel = level; }

    public Level LoadLevel(int levelnum)
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        string filePath = string.Format(levelDataPath, currentLevel % 10);
        if (currentLevel == 10 || currentLevel == 0)
        {
            filePath = "Assets/CaseStudyAssetsNoArea/Levels/level_10.json";
        }
        if (!File.Exists(filePath))
        {
            Debug.LogError("Level file not found at: " + filePath);

        }

        string jsonContent = File.ReadAllText(filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(jsonContent);

       
        return new Level(
            data.level_number,
            data.grid_width,
            data.grid_height,
            data.move_count,
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
        public string[] grid;
    }
}
