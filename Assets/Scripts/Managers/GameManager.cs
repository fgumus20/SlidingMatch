using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int moveCount, boxCount, vaseCount, stoneCount;
    public GameObject popup_ui;
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
    }
    public int GetMoveCount() { return moveCount; }
    public int GetBoxCount() {  return boxCount; }
    public int GetVaseCount() {  return vaseCount; }
    public int GetStoneCount() { return stoneCount; }
    public void SetGameNum(int moveCount, int boxCount, int vaseCount, int stoneCount,string pattern) 
    { 
        this.moveCount = moveCount;
        this.boxCount = boxCount;
        this.vaseCount = vaseCount;
        this.stoneCount = stoneCount;

        GameUIController.instance.Initialize(moveCount, boxCount, vaseCount, stoneCount,pattern);
        GameUIController.instance.checkObstacleState(boxCount, GetVaseCount(), GetStoneCount());
    }
    public void SetBoxCount(int boxCount)
    { 
        this.boxCount = boxCount;
        GameUIController.instance.SetBoxCount(boxCount);
        GameUIController.instance.checkObstacleState(boxCount,GetVaseCount(),GetStoneCount());
        CheckGameState();
    }
    public void SetVaseCount(int vaseCount) 
    { 
        this.vaseCount = vaseCount; 
        GameUIController.instance.SetVaseCount(vaseCount);
        GameUIController.instance.checkObstacleState(boxCount, GetVaseCount(), GetStoneCount());
        CheckGameState();
    }
    public void SetStoneCount(int stoneCount) 
    {  
        this.stoneCount = stoneCount; 
        GameUIController.instance.SetStoneCount(stoneCount);
        GameUIController.instance.checkObstacleState(boxCount, GetVaseCount(), GetStoneCount());
        CheckGameState();
    }
    public void DecreaseMoveCount () 
    { 
        this.moveCount--;
        GameUIController.instance.SetMoveCount(this.moveCount);
        GameUIController.instance.checkObstacleState(boxCount, GetVaseCount(), GetStoneCount());
        Debug.Log("aaa");
        CheckGameState();

    }
    public void DecreaseObstacleCount (ObjectType type)
    {
        if (type == ObjectType.Box)
        {
            GameManager.instance.SetBoxCount(GameManager.instance.GetBoxCount() - 1);
        }
        else if (type == ObjectType.Vase)
        {
            GameManager.instance.SetVaseCount(GameManager.instance.GetVaseCount() - 1);
        }
        else if (type == ObjectType.Stone)
        {
            GameManager.instance.SetStoneCount(GameManager.instance.GetStoneCount() - 1);
        }

        Debug.Log ("box =  " +this.GetBoxCount());
        Debug.Log("vase =  " + this.GetVaseCount());
        Debug.Log("stone =  " + this.GetStoneCount());
    }
    public void LoadMainMenu(){SceneManager.LoadScene(0);
        Debug.Log("aaa");
    }
    public void CheckGameState ()
    {
        if (GetMoveCount() == 0 && (GetBoxCount() != 0 || GetStoneCount() != 0 || GetVaseCount() != 0))
        {

            GameUIController.instance.SetActiveFailUI();
        }

        if (GetBoxCount() == 0 && GetStoneCount() == 0 && GetVaseCount() == 0)
        {
           

            LevelManager levelManager = LevelManager.GetLevelManager();
            levelManager.SetCurrentLevel(levelManager.GetCurrentLevel() + 1);
            levelManager.SaveLevelProgress();
            GameUIController.instance.SetActiveWinUI();
            
        }
    }
           
}
