using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWordsSeen : MonoBehaviour
{
    public Text wordsSeen;

    public void Start()
    {
        wordsSeen.text = "";
    }

    public void Refresh()
    {
        wordsSeen.text = "";
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        foreach(PlayerDictionary.Word word in GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().wordsSeen)
        {
            wordsSeen.text += word.translation.PadRight(10) + "\t\t" + word.koreanText + "\n";
        }
    }
}
