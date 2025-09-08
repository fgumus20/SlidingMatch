using UnityEditor;
using UnityEngine;

public class LevelEditorTools
{
    [MenuItem("Set Current Level/Level 1")]
    private static void SetLevel1()
    {
        SetCurrentLevel(1);
    }

    [MenuItem("Set Current Level/Level 2")]
    private static void SetLevel2()
    {
        SetCurrentLevel(2);
    }

    [MenuItem("Set Current Level/Level 3")]
    private static void SetLevel3()
    {
        SetCurrentLevel(3);
    }

    [MenuItem("Set Current Level/Level 4")]
    private static void SetLevel4()
    {
        SetCurrentLevel(4);
    }

    [MenuItem("Set Current Level/Level 5")]
    private static void SetLevel5()
    {
        SetCurrentLevel(5);
    }

    [MenuItem("Set Current Level/Level 6")]
    private static void SetLevel6()
    {
        SetCurrentLevel(6);
    }

    [MenuItem("Set Current Level/Level 7")]
    private static void SetLevel17()
    {
        SetCurrentLevel(7);
    }

    [MenuItem("Set Current Level/Level 8")]
    private static void SetLevel8()
    {
        SetCurrentLevel(8);
    }

    [MenuItem("Set Current Level/Level 9")]
    private static void SetLevel9()
    {
        SetCurrentLevel(9);
    }

    [MenuItem("Set Current Level/Level10")]
    private static void SetLevel10()
    {
        SetCurrentLevel(10);
    }

    private static void SetCurrentLevel(int level)

    {

        LevelManager levelManager = LevelManager.GetLevelManager();

        if (levelManager != null)
        {
            levelManager.SetCurrentLevel(level);
            levelManager.SaveLevelProgress();
            MainMenuManager.instance.UpdateText();
        }
        else
        {
            Debug.LogError("LevelManager instance not found!");
        }
    }
}
