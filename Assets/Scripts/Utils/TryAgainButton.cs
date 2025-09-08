using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryAgainButton : MonoBehaviour
{
    public GameObject popUP;
    private void OnMouseDown()
    {
        SceneManager.LoadScene(1);
        popUP.SetActive(false);
    }
}
