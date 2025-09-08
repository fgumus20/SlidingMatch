using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    public GameObject buttonText;
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
}
