using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    public GameObject blockPrefab;
    [HideInInspector] public int wordIndex = 0;
    public string wordPoolName = "Colors";
    public bool useSpecficWord = false;
    public string specficWord = "";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(3);

        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (name == "Taco" && !playerController.taco)
            return;

        //print(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict.Count + " " + wordPoolName);
        List<PlayerDictionary.Word> words = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[wordPoolName];
        playerController.Stop(true);
        if (useSpecficWord)
        {
            if (specficWord == "Yellow")
            {
                wordIndex = 0;
            }
            else if (specficWord == "Red")
            {
                wordIndex = 1;
            }
            else if (specficWord == "Black")
            {
                wordIndex = 2;
            }
            else if (specficWord == "Hello")
            {
                wordIndex = 0;
            }
        }
        else
        {
            wordIndex = Random.Range(0, words.Count);
        }
        PlayerDictionary.Word word = words[wordIndex];

        GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Write();
        GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().expectedWord = word;
        GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().makeTraceBoxes();
        GameObject.Find("TextYet").GetComponent<Text>().text = word.koreanText;
        GameObject.Find("TopMostText").GetComponent<Text>().text = "\"" + word.translation + "\"";

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().wordsSeen.Add(word);
    }
    public void handleSubmit()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


        if (playerController.inProgressQuests.ContainsKey("PickUp") &&
            playerController.inProgressQuests["PickUp"].ContainsKey(playerController.currScene) &&
            (playerController.inProgressQuests["PickUp"][playerController.currScene].completed() == false))
        {
            playerController.inProgressQuests["PickUp"][playerController.currScene].currentAmount++;
            if (playerController.inProgressQuests["PickUp"].ContainsKey(playerController.currScene) && GameObject.Find("PickUp" + playerController.currScene) != null)
            {
                GameObject.Find("PickUp" + playerController.currScene).GetComponentInChildren<Text>().text = "PickUp in " + playerController.currScene + "! " + playerController.inProgressQuests["PickUp"][playerController.currScene].currentAmount + " out of " + playerController.inProgressQuests["PickUp"][playerController.currScene].goalAmount;
                GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(4);
            }
        }
        if (GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().isGood)
        {
            GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(4);
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Close();
            playerController.Unfreeze();
            Destroy(gameObject);
        }

    }
}
