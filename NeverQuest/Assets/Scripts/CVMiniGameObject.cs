using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CVMiniGameObject : MonoBehaviour
{
    [HideInInspector] public string type = "";
    [HideInInspector] public int num = 404;
    private CVMiniGameController yeet;
    private void Start()
    {
        yeet = GameObject.Find("Brain").GetComponent<CVMiniGameController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Write(true);
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().expectedWord =
            new PlayerDictionary.Word(
                "",
                new List<PlayerDictionary.Syllable>()
                {
                    new PlayerDictionary.Syllable(7, new List<string>(){ num.ToString() })
                });

            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().makeTraceBoxes();

        }
    }

    public void handleSubmit()
    {
        if (GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().isGood)
        {
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Close();
            GameObject.Find("Player").GetComponent<PlayerController>().Unfreeze();
            Destroy(gameObject);
            yeet.numVowelsDestroyed++;
        }
    }


}
