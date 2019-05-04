using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NpcController : MonoBehaviour
{
    [HideInInspector] public Quest quest;
    [HideInInspector] public bool shouldDestroy;

    public string myName;

    //Conversation
    public List<string> conversationBefore = new List<string>();
    public List<string> conversationDuring = new List<string>();
    public List<string> conversationAfter = new List<string>();

    //Quest
    public string type;
    public string location;
    public int goalAmount;
    public GameObject QuestGuiObject;
    public string roomUnlockedOnBegin;
    public string roomUnlockedOnFinish;

    public bool sarah_before;
    public bool sarah_after;

    private PlayerController player;
    private GameObject messageHandler;
    private GameObject mask;
    private static int calloutCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        messageHandler = GameObject.FindGameObjectWithTag("MessageBox");
        mask = GameObject.FindGameObjectWithTag("Mask");
        shouldDestroy = false;
        if (type != "")
        {
            quest = new Quest();
            quest.type = type;

            quest.location = location;
            quest.alreadyGivenQuest = false;
            quest.goalAmount = goalAmount;

            if (player.demo)
                quest.goalAmount = 0;

            quest.givenBy = myName;
            quest.onScreen = false;

            quest.roomUnlockedOnBegin = roomUnlockedOnBegin;
            quest.roomUnlockedOnFinish = roomUnlockedOnFinish;
        }
        else { quest = null; }
    }

    public void PlayConvoBefore()
    {
        GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(4);
        calloutCounter = 0;
        StartCoroutine(ConvoBefore());
    }

    public void PlayConvoAfter()
    {
        GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(4);
        calloutCounter = 0;
        StartCoroutine(ConvoAfter());
    }
    public void PlayConvoDuring()
    {
        GameObject.Find("Active Menu").GetComponent<menuSwitch>().changeMenu(4);
        calloutCounter = 0;
        StartCoroutine(ConvoDuring());
    }


    IEnumerator ConvoBefore()
    {
        player.Stop(true);
        messageHandler = GameObject.FindGameObjectWithTag("MessageBox");
        GameObject prev = new GameObject();
        messageHandler.GetComponent<MessageHandler>().setSender(myName);

        string convoline = conversationBefore[calloutCounter];
        messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
        if (!conversationBefore[conversationBefore.Count - 1].Equals(""))
        {
            conversationBefore.Add("");
        }


        while (calloutCounter < conversationBefore.Count)
        {
            if (Input.GetMouseButtonDown(0))
            {
                calloutCounter++;
                convoline = "";
                if (calloutCounter % 2 == 0)
                {
                    messageHandler.GetComponent<MessageHandler>().setSender(myName);
                }
                else
                {
                    messageHandler.GetComponent<MessageHandler>().setSender("%p (You)");
                }

                convoline = conversationBefore[calloutCounter];
                messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
                messageHandler.GetComponent<MessageHandler>().Activate();
                //calloutCounter++;

                if (calloutCounter == conversationBefore.Count - 1)
                {
                    calloutCounter += 2;
                    player.Unfreeze();
                    messageHandler.GetComponent<MessageHandler>().Deactivate();
                    if (mask)
                    {
                        mask.SetActive(false);
                    }

                }
                yield return new WaitForSeconds(0);
            }
            yield return null;
        }
        player.inConvo = false;
        player.Unfreeze();

        if(myName == "???")
          mask.SetActive(false);

        Debug.Log("Exiting before");
    }


    IEnumerator ConvoAfter()
    {
        player.Stop(true);
        messageHandler = GameObject.FindGameObjectWithTag("MessageBox");
        string convoline = conversationAfter[0];
        messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
        messageHandler.GetComponent<MessageHandler>().setSender("%p (You)");

        if (!conversationAfter[conversationAfter.Count - 1].Equals(""))
        {
            //Debug.Log("Convo After[-1]: " + conversationAfter[conversationAfter.Count - 1]);
            conversationAfter.Add("");
        }
        calloutCounter++;
        GameObject prev = new GameObject();
        while (calloutCounter < conversationAfter.Count)
        {

            if (Input.GetMouseButtonDown(0))
            {
                //calloutCounter++;
                //Debug.Log(calloutCounter);
                convoline = "";
                if (calloutCounter % 2 == 0)
                {
                    messageHandler.GetComponent<MessageHandler>().setSender("%p (You)");
                }
                else
                {
                    messageHandler.GetComponent<MessageHandler>().setSender(myName);
                }

                convoline = conversationAfter[calloutCounter];
                messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
                messageHandler.GetComponent<MessageHandler>().Activate();
                calloutCounter++;

                if (calloutCounter == conversationAfter.Count)
                {
                    calloutCounter += 2;
                    player.Unfreeze();
                    messageHandler.GetComponent<MessageHandler>().Deactivate();
                }
                yield return new WaitForSeconds(0);
            }
            yield return null;
        }

        if (sarah_before)
        {
            Debug.Log("Changing");
            myName = "Sarah";
            sarah_before = false;
            sarah_after = true;

            quest = new Quest();
            quest.type = "PickUp";

            quest.goalAmount = 1;

            if (player.demo)
                quest.goalAmount = 0;

            quest.location = "Home";
            quest.alreadyGivenQuest = false;
            quest.givenBy = myName;
            quest.onScreen = false;

            player.taco = true;

            conversationBefore = new List<string>() { "...", "...how do I say hello again?" };
            conversationDuring = new List<string>() { "Maybe if I go to the taco and write it out?" };
            conversationAfter = new List<string>() { "I wrote hello on this taco for you Sarah!", "...hello! Yes, I remember!", "What happened?", "I went into town to buy groceries, and I was attacked by these monsters, but they didn't hurt me; it's like they took my language!", "*That strange voice was right, maybe I should go into the city to see what she's talking about.*" };

            player.AddQuest(this);
        }

        else if (sarah_after)
        {
            sarah_after = false;

            conversationAfter = new List<string>() { "Hey Sarah.", "Hello! I've never loved a word so much in my life." };
        }
        player.inConvo = false;
        player.Unfreeze();
        Debug.Log("Exiting after");
    }



    IEnumerator ConvoDuring()
    {
        Debug.Log("Exec: Convo During");
        player.Stop(true);
        messageHandler = GameObject.FindGameObjectWithTag("MessageBox");
        string convoline = conversationDuring[0];
        messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
        messageHandler.GetComponent<MessageHandler>().setSender("%p (You)");
        if (conversationDuring.Count == 0)
        {
            Debug.Log("Should be quitting ConvoDuring");
            player.inConvo = false;
            player.Unfreeze();
            yield return null;
        }
        if (!conversationDuring[conversationDuring.Count - 1].Equals(""))
        {
            Debug.Log("Convo During[-1]: " + conversationDuring[conversationDuring.Count - 1]);
            conversationDuring.Add("");
        }
        calloutCounter++;
        while (calloutCounter < (conversationDuring.Count))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (calloutCounter % 2 == 0)
                {
                    messageHandler.GetComponent<MessageHandler>().setSender("%p (You)");
                }
                else
                {
                    messageHandler.GetComponent<MessageHandler>().setSender(myName);
                }

                convoline = conversationDuring[calloutCounter];
                messageHandler.GetComponent<MessageHandler>().setMessage(convoline);
                messageHandler.GetComponent<MessageHandler>().Activate();
                calloutCounter++;
                if (calloutCounter == conversationDuring.Count)
                {
                    calloutCounter += 2;
                    player.Unfreeze();
                    messageHandler.GetComponent<MessageHandler>().Deactivate();
                }
                
                yield return null;
            }
            yield return null;
        }
        
        player.inConvo = false;
        player.Unfreeze();
        Debug.Log("Exiting during");
    }


}

public class Quest
{
    public string givenBy;
    public bool alreadyGivenQuest;
    public string type;
    public string location;
    public int currentAmount;
    public int goalAmount;
    public bool onScreen;
    public string roomUnlockedOnBegin;
    public string roomUnlockedOnFinish;
    public bool tracked = false;

    public bool completed()
    {
        if (currentAmount < goalAmount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
