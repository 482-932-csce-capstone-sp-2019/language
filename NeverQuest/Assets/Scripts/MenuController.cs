using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [HideInInspector] public int langNum = 1;

    public void clickStart()
    {
        SceneManager.LoadScene("HUD");
    }


}

