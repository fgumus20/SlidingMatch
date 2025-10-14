using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    public GameObject movesTextG, boxNumTextG, stoneNumTextG, vaseNumTextG;
    private TextMeshPro movesText, boxNumText, stoneNumText, vaseNumText;
    private int moveCount, boxCount, vaseCount, stoneCount;
    public GameObject boxCheck, stoneCheck, vaseCheck;
    public GameObject popup_ui, winUI;
    public GameObject x2Cubes, lineCubes;

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

    public void Initialize(int moveCount, int boxCount, int vaseCount, int stoneCount,string pattern)
    {
        movesText = movesTextG.GetComponent<TextMeshPro>();
        boxNumText = boxNumTextG.GetComponent<TextMeshPro>();
        vaseNumText = vaseNumTextG.GetComponent<TextMeshPro>();
        stoneNumText = stoneNumTextG.GetComponent<TextMeshPro>();
        Debug.Log("[UI] movesText is " + (movesText ? "OK" : "NULL"));

        SetPatternUI(pattern);
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
        Debug.Log($"[UI] UpdateTexts moves={moveCount} -> {moveCount}, box={r_boxNum}, vase={r_vaseNum}, stone={r_stoneNum}");

        MatchFX.I?.BumpCounter(movesTextG.GetComponent<RectTransform>());


        if (this.moveCount != moveCount)
        {
            MatchFX.I?.BumpCounter(movesTextG.GetComponent<RectTransform>());
            this.moveCount = moveCount;
        }
        if (this.boxCount != r_boxNum)
        {
            MatchFX.I?.BumpCounter(boxNumTextG.GetComponent<RectTransform>());
            this.boxCount = r_boxNum;
        }
        if (this.vaseCount != r_vaseNum)
        {
            MatchFX.I?.BumpCounter(vaseNumTextG.GetComponent<RectTransform>());
            this.vaseCount = r_vaseNum;
        }
        if (this.stoneCount != r_stoneNum)
        {
            MatchFX.I?.BumpCounter(stoneNumTextG.GetComponent<RectTransform>());
            this.stoneCount = r_stoneNum;
        }
    }


    public void SetMoveCount(int moveCount)
    {
        movesText.text = moveCount.ToString();
        if (this.moveCount != moveCount)
        {
            MatchFX.I?.BumpCounter(movesTextG.GetComponent<RectTransform>());
            this.moveCount = moveCount;
        }
    }

    public void SetBoxCount(int boxCount)
    {
        boxNumText.text = boxCount.ToString();
        if (this.boxCount != boxCount)
        {
            MatchFX.I?.BumpCounter(boxNumTextG.GetComponent<RectTransform>());
            this.boxCount = boxCount;
        }
        checkObstacleState(this.boxCount, this.vaseCount, this.stoneCount);
    }

    public void SetVaseCount(int vaseCount)
    {
        vaseNumText.text = vaseCount.ToString();
        if (this.vaseCount != vaseCount)
        {
            MatchFX.I?.BumpCounter(vaseNumTextG.GetComponent<RectTransform>());
            this.vaseCount = vaseCount;
        }
        checkObstacleState(this.boxCount, this.vaseCount, this.stoneCount);
    }

    public void SetStoneCount(int stoneCount)
    {
        stoneNumText.text = stoneCount.ToString();
        if (this.stoneCount != stoneCount)
        {
            MatchFX.I?.BumpCounter(stoneNumTextG.GetComponent<RectTransform>());
            this.stoneCount = stoneCount;
        }
        checkObstacleState(this.boxCount, this.vaseCount, this.stoneCount);
    }

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

    private void SetPatternUI(string pattern)
    {
        Debug.Log(pattern);
        if(pattern.Equals("square"))
        {
            x2Cubes.SetActive(true);
            lineCubes.SetActive(false);
        }
        else
        {
            x2Cubes.SetActive(false);
            lineCubes.SetActive(true);
        }
    }
}
