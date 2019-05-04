using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiController : MonoBehaviour
{
    public Button OpenWriteButton;
    public Button CloseWriteButton;
    public GameObject WriteBox;
    public GameObject WriteBoxMini;
    public Button SubmitButton;
    public Button Clear;
    public Button CheatCode;

    public GameObject popUp;
    public GameObject TraceTemp;
    public GameObject SyllableTemp1;
    public GameObject SyllableTemp2;
    public GameObject SyllableTemp3;
    public GameObject SyllableTemp4;
    public GameObject SyllableTemp5;
    public GameObject SyllableTemp6;
    public GameObject SyllableTemp7;


    private GameObject player;
    private PlayerController playerController;
    private GameObject template;
    private int templateNum = 0;
    private int currSyllableAttempt = 0;
    private int currCharAttempt = 0;
    private string completedChars = "";
    private string nonCompletedChars = "";
    private int numPopUps = 0;

    [HideInInspector] public PlayerDictionary.Word expectedWord;

    [HideInInspector] public bool isGood = false;

    private List<GameObject> traceBoxes;

    private void Start()
    {
        GameObject.FindGameObjectWithTag("GUI").GetComponentInChildren<MiniMap>().UpdateMinimap("Home");
        template = new GameObject();
        traceBoxes = new List<GameObject>();

    }

    public void AssignSceneCamera()
    {
        GameObject.Find("MasterCanvas").GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void Write(bool openMiniVersion = false)
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        playerController.writing = true;

        if (openMiniVersion)
        {
            temp7();
            WriteBoxMini.gameObject.SetActive(true);
            WriteBoxMini.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template), true);
        }
        else
        {
            WriteBox.gameObject.SetActive(true);
            temp1();
            GameObject.Find("TextDone").GetComponent<Text>().text = completedChars;
            GameObject.Find("TextYet").GetComponent<Text>().text = nonCompletedChars;
            WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
        }

        OpenWriteButton.gameObject.SetActive(false);
        CloseWriteButton.gameObject.SetActive(true);
        SubmitButton.gameObject.SetActive(true);
        Clear.gameObject.SetActive(true);

        completedChars = "";
        nonCompletedChars = "";
        currSyllableAttempt = 0;
        currCharAttempt = 0;
        numPopUps = 0;

    }

    public void Close()
    {
        if (playerController.writing)
        {
            playerController.writing = false;
            if (WriteBox)
            {
                WriteBox.GetComponent<writtenInput>().Clear();
                WriteBox.GetComponent<writtenInput>().CancelInvoke();
                WriteBox.gameObject.SetActive(false);
            }
            if (WriteBoxMini)
            {
                WriteBoxMini.GetComponent<writtenInput>().Clear();
                WriteBoxMini.GetComponent<writtenInput>().CancelInvoke();
                WriteBoxMini.gameObject.SetActive(false);
            }

            OpenWriteButton.gameObject.SetActive(true);
            CloseWriteButton.gameObject.SetActive(false);
            SubmitButton.gameObject.SetActive(false);
            Clear.gameObject.SetActive(false);
            Destroy(template);

            foreach (GameObject item in traceBoxes)
            {
                Destroy(item);
            }
        }
    }

    public void Submit(bool ForceCorrect = false)
    {
        numPopUps = 0;
        List<string> rawNumInput;
        if (player.GetComponent<PlayerController>().currScene == "Tutorial Area")
        {
            rawNumInput = WriteBoxMini.GetComponent<writtenInput>().SubmitInput(templateNum, GetChildren(template));
        }
        else
        {
            rawNumInput = WriteBox.GetComponent<writtenInput>().SubmitInput(templateNum, GetChildren(template));
        }


        string koreanInput = "";

        foreach (string yuh in rawNumInput)
        {
            koreanInput += PlayerDictionary.numToKorean[yuh];
        }


        Vector3 popUpSpawn = Camera.main.ScreenToWorldPoint(CloseWriteButton.transform.position);
        popUpSpawn.z = 0;

        if (ForceCorrect == false)
        {

            if (expectedWord.syllables[currSyllableAttempt].tempNum != templateNum)
            {
                GameObject tmp = Instantiate(popUp, popUpSpawn, Quaternion.identity);
                tmp.GetComponent<PopUpController>().makePopUp("WrongL");
                popUpSpawn.y -= 1.5f;
            }

            for (int i = 0; i < koreanInput.Length && i < expectedWord.syllables[currSyllableAttempt].koreanString.Length; i++)
            {
                if (koreanInput[i] != expectedWord.syllables[currSyllableAttempt].koreanString[i])
                {
                    GameObject tmp = Instantiate(popUp, popUpSpawn, Quaternion.identity);
                    tmp.GetComponent<PopUpController>().makePopUp("Wrong" + (i + 1).ToString());
                    popUpSpawn.y -= 1.5f;
                }
            }
        }

        if (koreanInput == expectedWord.syllables[currSyllableAttempt].koreanString || ForceCorrect == true)
        {
            isGood = true;
            Destroy(template);
            foreach (GameObject item in traceBoxes)
            {
                Destroy(item);
            }
        }
        else
        {
            isGood = false;
        }


        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController.currScene == "Tutorial Area")
        {
            GameObject currClosest = new GameObject();
            float currMinDist = 10000;
            foreach (GameObject picks in GameObject.FindGameObjectsWithTag("CV"))
            {
                if (Vector3.Distance(picks.transform.position, player.transform.position) < currMinDist)
                {
                    currMinDist = Vector3.Distance(picks.transform.position, player.transform.position);
                    currClosest = picks;
                }
            }
            currClosest.GetComponent<CVMiniGameObject>().handleSubmit();
        }

        if (GameObject.FindGameObjectWithTag("PickUp") || GameObject.FindGameObjectWithTag("Baddy"))
        {

            GameObject currClosest = new GameObject();

            if (GameObject.FindGameObjectWithTag("PickUp"))
            {
                float currMinDist = 10000;
                foreach (GameObject picks in GameObject.FindGameObjectsWithTag("PickUp"))
                {
                    if (Vector3.Distance(picks.transform.position, player.transform.position) < currMinDist)
                    {
                        currMinDist = Vector3.Distance(picks.transform.position, player.transform.position);
                        currClosest = picks;
                    }
                }

            }
            else if (GameObject.FindGameObjectWithTag("Baddy"))
            {
                //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[GameObject.FindGameObjectWithTag("Baddy").GetComponent<BaddyController>().wordPoolName][GameObject.FindGameObjectWithTag("Baddy").GetComponent<PickUpController>().wordIndex].numsSeen++;
            }

            if (isGood)
            {
                GameObject tmp = Instantiate(popUp, popUpSpawn, Quaternion.identity);
                tmp.GetComponent<PopUpController>().makePopUp("NiceJ");
                currCharAttempt += expectedWord.syllables[currSyllableAttempt].koreanString.Length;
                completedChars = expectedWord.koreanText.Substring(0, currCharAttempt);
                nonCompletedChars = expectedWord.koreanText.Substring(currCharAttempt, expectedWord.koreanText.Length - currCharAttempt);


                if (GameObject.Find("TextDone"))
                {
                    GameObject.Find("TextDone").GetComponent<Text>().text = completedChars;
                }
                if (GameObject.Find("TextYet"))
                {
                    GameObject.Find("TextYet").GetComponent<Text>().text = nonCompletedChars;
                }
                currSyllableAttempt++;

                if (GameObject.FindGameObjectWithTag("PickUp"))
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[currClosest.GetComponent<PickUpController>().wordPoolName][currClosest.GetComponent<PickUpController>().wordIndex].numsSeen++;
                }
                else if (GameObject.FindGameObjectWithTag("Baddy") && playerController.currScene != "Tutorial Area")
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[GameObject.FindGameObjectWithTag("Baddy").GetComponent<BaddyController>().wordPoolName][GameObject.FindGameObjectWithTag("Baddy").GetComponent<BaddyController>().wordIndex].numsSeen++;
                }

                //If done with word
                if (currSyllableAttempt >= expectedWord.syllables.Count)
                {
                    currCharAttempt = 0;
                    completedChars = "";
                    nonCompletedChars = "";
                    if (GameObject.Find("TextDone"))
                    {
                        GameObject.Find("TextDone").GetComponent<Text>().text = "";
                    }
                    if (GameObject.Find("TextYet"))
                    {
                        GameObject.Find("TextYet").GetComponent<Text>().text = "";
                    }
                    currSyllableAttempt = 0;

                    //Finds closest pickup
                    if (GameObject.FindGameObjectWithTag("PickUp"))
                    {
                        currClosest.GetComponent<PickUpController>().handleSubmit();
                    }
                    //Assumes only one baddy
                    else if (GameObject.FindGameObjectWithTag("Baddy") && playerController.currScene != "Tutorial Area")
                    {
                        GameObject.FindGameObjectWithTag("Baddy").GetComponent<BaddyController>().handleSubmit();
                    }
                    Close();
                }
                //Not done, show next
                else
                {

                }
            }
            //incorrect input
            else
            {

                if (GameObject.FindGameObjectWithTag("PickUp"))
                {
                    float currMinDist = 10000;
                    foreach (GameObject picks in GameObject.FindGameObjectsWithTag("PickUp"))
                    {
                        if (Vector3.Distance(picks.transform.position, player.transform.position) < currMinDist)
                        {
                            currMinDist = Vector3.Distance(picks.transform.position, player.transform.position);
                            currClosest = picks;
                        }
                    }
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[currClosest.GetComponent<PickUpController>().wordPoolName][currClosest.GetComponent<PickUpController>().wordIndex].numsMissed++;
                }
                else if (GameObject.FindGameObjectWithTag("Baddy") && player.GetComponent<PlayerController>().currScene != "Tutorial Area")
                {
                    playerController.health -= 10;
                    Debug.Log("Health: " + playerController.health);
                    playerController.UpdateHealth();
                }

                //Clear and reopen either the tutorial write box or the actual game write box
                if (player.GetComponent<PlayerController>().currScene == "Tutorial Area")
                {
                    WriteBoxMini.GetComponent<writtenInput>().Clear();
                    WriteBoxMini.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template), true);
                }
                else
                {
                    WriteBox.GetComponent<writtenInput>().Clear();
                    WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
                }
            }
        }
    }

    public void CheatCodeClicked()
    {
        Submit(true);
    }

    public void temp1()
    {
        WriteBox.GetComponent<writtenInput>().Clear();
        WriteBox.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp1, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 1;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp2()
    {
        WriteBox.GetComponent<writtenInput>().Clear();
        WriteBox.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp2, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 2;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp3()
    {
        WriteBox.GetComponent<writtenInput>().Clear();
        WriteBox.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp3, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 3;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp4()
    {
        WriteBox.GetComponent<writtenInput>().Clear();
        WriteBox.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp4, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 4;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp5()
    {
        if (WriteBox.GetComponent<writtenInput>() != null)
        {
            WriteBox.GetComponent<writtenInput>().Clear();
            WriteBox.GetComponent<writtenInput>().CancelInvoke();
        }
        Destroy(template);
        template = Instantiate(SyllableTemp5, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 5;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp6()
    {
        WriteBox.GetComponent<writtenInput>().Clear();
        WriteBox.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp6, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 6;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBox.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template));
    }
    public void temp7()
    {
        WriteBoxMini.GetComponent<writtenInput>().Clear();
        WriteBoxMini.GetComponent<writtenInput>().CancelInvoke();
        Destroy(template);
        template = Instantiate(SyllableTemp7, new Vector3(-2, -1.5f, 3), Quaternion.identity);
        template.transform.SetParent(GameObject.Find("Main Camera").transform, false);
        templateNum = 7;

        if (expectedWord != null)
        {
            makeTraceBoxes();
        }

        WriteBoxMini.GetComponent<writtenInput>().WriteBoxOpened(GetChildren(template), true);
    }

    public void makeTraceBoxes()
    {
        foreach (GameObject item in traceBoxes)
        {
            Destroy(item);
        }

        List<GameObject> boxes = GetChildren(template);
        float opacityValue;
        if (expectedWord.numsSeen == 0)
        {
            opacityValue = 100;
        }
        else
        {
            opacityValue = ((expectedWord.numsMissed / 3) + 1) / (expectedWord.numsSeen - (expectedWord.numsMissed / 3));
        }

        for (int i = 0; i < Mathf.Min(expectedWord.syllables[currSyllableAttempt].koreanString.Length, boxes.Count); i++)
        {
            GameObject tmp = Instantiate(TraceTemp, new Vector3(boxes[i].transform.position.x, boxes[i].transform.position.y, boxes[i].transform.position.z), Quaternion.identity);
            traceBoxes.Add(tmp);
            tmp.transform.SetParent(GameObject.Find("Main Camera").transform, true);
            tmp.transform.GetComponent<SpriteRenderer>().sortingOrder = 1;

            Color tmpColor = tmp.GetComponent<SpriteRenderer>().color;
            tmpColor.a = opacityValue;
            tmp.GetComponent<SpriteRenderer>().color = tmpColor;
            tmp.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);

            tmp.GetComponent<SpriteRenderer>().sprite = Resources.Load("StrokeOrder/" + expectedWord.syllables[currSyllableAttempt].characterNumsList[i], typeof(Sprite)) as Sprite;
        }
    }

    List<GameObject> GetChildren(GameObject parentDude)
    {
        List<GameObject> returnMe = new List<GameObject>();
        for (int i = 0; i < parentDude.transform.childCount; i++)
        {
            returnMe.Add(parentDude.transform.GetChild(i).gameObject);
        }
        return returnMe;
    }

}
