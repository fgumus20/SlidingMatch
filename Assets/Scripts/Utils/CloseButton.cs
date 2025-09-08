using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour
{
    GameObject tryAgainButton;
    private void OnMouseDown()
    {
        SceneManager.LoadScene(0);
    }

    


}
