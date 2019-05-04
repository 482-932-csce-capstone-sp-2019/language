using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuSwitch : MonoBehaviour
{
    public GameObject menu1;
    public GameObject menu2;
    public GameObject menu3;
    public GameObject menu4;
    private GameObject player;

    private int activeMenu;
    private bool flip;

    void start()
    {
        flip = false;
    }

    public void changeMenu(int menu)
    {
        activeMenu = menu;
        switch (activeMenu)
        {
            case 1:
                menu1.SetActive(true);
                menu2.SetActive(false);
                menu3.SetActive(false);
                menu4.SetActive(false);
                break;
            case 2:
                menu1.SetActive(false);
                menu2.SetActive(true);
                menu3.SetActive(false);
                menu4.SetActive(false);
                GameObject.Find("Viewport").GetComponent<UpdateWordsSeen>().Refresh();
                break;
            case 3:
                menu1.SetActive(false);
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu4.SetActive(false);
                break;

            //Quests
            case 4:
                menu1.SetActive(false);
                menu2.SetActive(false);
                menu3.SetActive(false);
                menu4.SetActive(true);
                updateQuest();

                break;
            case -1:
                if (!flip)
                {
                    menu1.SetActive(false);
                    menu2.SetActive(false);
                    menu3.SetActive(false);
                    menu4.SetActive(false);
                    flip = true;
                    activeMenu = 1;
                }
                else
                {
                    menu1.SetActive(true);
                    menu2.SetActive(false);
                    menu3.SetActive(false);
                    menu4.SetActive(false);
                    flip = false;
                    activeMenu = 1;
                }
                break;
            default:
                activeMenu = 1;
                changeMenu(activeMenu);
                break;

        }
    }

    public void updateQuest()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //print("__________________");
        //foreach (KeyValuePair<string, Dictionary<string, Quest>> currPair in playerController.inProgressQuests)
        //{
        //    print(currPair.Key);
        //    foreach (KeyValuePair<string, Quest> currr in currPair.Value)
        //    {
        //        print("------" + currr.Key + " " + currr.Value);
        //    }
        //}
        //print("__________________");

        if (playerController.inProgressQuests.ContainsKey("Kill"))
        {
            //If has quest for current scene
            if (playerController.inProgressQuests["Kill"].ContainsKey(playerController.currScene))
            {
                GameObject.Find("Kill" + playerController.currScene).GetComponentInChildren<Text>().text = "Kill in " + playerController.currScene + "! " + playerController.inProgressQuests["Kill"][playerController.currScene].currentAmount + " out of " + playerController.inProgressQuests["Kill"][playerController.currScene].goalAmount;
            }
        }
        if (playerController.inProgressQuests.ContainsKey("PickUp"))
        {
            //If has quest for current scene
            if (playerController.inProgressQuests["PickUp"].ContainsKey(playerController.currScene))
            {
                GameObject.Find("PickUp" + playerController.currScene).GetComponentInChildren<Text>().text = "PickUp in " + playerController.currScene + "! " + playerController.inProgressQuests["PickUp"][playerController.currScene].currentAmount + " out of " + playerController.inProgressQuests["PickUp"][playerController.currScene].goalAmount;
            }
        }
    }
}
