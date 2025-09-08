using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    public GameObject movesTextG, boxNumTextG, stoneNumTextG, vaseNumTextG;
    private TextMeshPro movesText, boxNumText, stoneNumText, vaseNumText;
    private int moveCount, boxCount, vaseCount, stoneCount;
    public GameObject boxCheck, stoneCheck, vaseCheck;
    public GameObject popup_ui, winUI;

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

    public void Initialize(int moveCount, int boxCount, int vaseCount, int stoneCount)
    {
        movesText = movesTextG.GetComponent<TextMeshPro>();
        boxNumText = boxNumTextG.GetComponent<TextMeshPro>();
        vaseNumText = vaseNumTextG.GetComponent<TextMeshPro>();
        stoneNumText = stoneNumTextG.GetComponent<TextMeshPro>();

        this.moveCount = moveCount;
        movesText.text = moveCount.ToString();
        this.boxCount = boxCount;
        boxNumText.text = boxCount.ToString();
        this.stoneCount = stoneCount;
        stoneNumText.text = stoneCount.ToString();
        this.vaseCount = vaseCount;
        vaseNumText.text = vaseCount.ToString();
        checkObstacleState(boxCount, stoneCount, vaseCount);
    }

    public void UpdateTexts(int moveCount, int r_boxNum, int r_vaseNum, int r_stoneNum)
    {

        movesText.text = moveCount.ToString();
        boxNumText.text = r_boxNum.ToString();
        vaseNumText.text = r_vaseNum.ToString();
        stoneNumText.text = r_stoneNum.ToString();
        checkObstacleState(r_boxNum, r_vaseNum, r_stoneNum);
        //checkGameState();
    }

    public void SetMoveCount(int moveCount) { movesText.text = moveCount.ToString(); }
    public void SetBoxCount(int boxCount) { boxNumText.text = boxCount.ToString(); }
    public void SetVaseCount(int vaseCount) { vaseNumText.text = vaseCount.ToString(); }
    public void SetStoneCount(int stoneCount) { stoneNumText.text = stoneCount.ToString(); }
    public void checkObstacleState(int boxNum, int vaseNum, int stoneNum)
    {
        if (boxNum == 0)
        {
            boxCheck.SetActive(true);
            boxNumText.text = " ";
        }
        else { boxCheck.SetActive(false); }

        if (stoneNum == 0)
        {
            stoneCheck.SetActive(true);
            stoneNumText.text = " ";
        }
        else { stoneCheck.SetActive(false); }

        if (vaseNum == 0)
        {
            vaseCheck.SetActive(true);
            vaseNumText.text = " ";
        }
        else { vaseCheck.SetActive(false); }
    }
    public void SetActiveFailUI() { popup_ui.SetActive(true); }
    public void SetActiveWinUI()
    {
        StartCoroutine(WinUISequence());
        
    }

    private IEnumerator WinUISequence()
    {
        winUI.SetActive(true);
        yield return new WaitForSeconds(5);
        winUI.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
