using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    [HideInInspector] public MusicHandler mus;
    public GameObject musObj;
    public Text messageText;
    public Text messageSender;
    public Text helperText;
    public GameObject messageBackground;
    public string playerName;

    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("Placeholder"))
        {
            NameHolder nameHolder = GameObject.FindGameObjectWithTag("Placeholder").GetComponent<NameHolder>();
            playerName = nameHolder.getName();
        }
        else
        {
            playerName = "Chosen One";
        }

    }

    public void setSender(string name)
    {
        messageSender.text = name.Replace("\\n", "\n").Replace("%p", playerName);
        //Debug.Log("Sender name: " + name);
        messageSender.enabled = true;
    }
    public void setMessage(string message)
    {
        /*  A message is a string which may contain:
         *  \n      : Newline char
         *  %p      : Player's name;
         *  <E:"">  : Event "" **ONLY ONE EVENT CAN BE HANDLED PER LINE
         */

        if (message.Contains("<"))
        {
            //Message needs Event Parsing
            int start = message.IndexOf("<") + 1;
            int length = message.IndexOf(">") - start;

            string messEvent = message.Substring(start, length);
            message = message.Substring(0, start - 1);

            eventParse(messEvent);
        }

        messageText.text = message.Replace("\\n", "\n").Replace("%p", playerName);
        //Debug.Log("Message: " + message);
        messageSender.enabled = true;
    }
    public void Activate()
    {
        messageText.enabled = true;
        messageSender.enabled = true;
        helperText.enabled = true;
        messageBackground.SetActive(true);
    }
    public void Deactivate()
    {
        messageText.enabled = false;
        messageSender.enabled = false;
        helperText.enabled = false;
        messageBackground.SetActive(false);
    }


    void eventParse(string input)
    {
        //MusicHandler mus = musObj.GetComponent<MusicHandler>();
        //Debug.Log(input);
        /* Event is of the form:
         * E:"EventType:Arg1,Arg2,..."  //Without quotes
         * EventType: int code s.t.
         *      1: Music Event
         *      2: Blackout (Screen Mask) Event
         */

        int typeCode = int.Parse(input.Substring(2, 1));
        ArrayList args = new ArrayList();
        string arg = "";
        mus = musObj.GetComponent<MusicHandler>();
        foreach (char ch in input.Substring(4))
        {
            if (ch == ',')
            {
                args.Add(arg);
                //Debug.Log("Arg: " + arg);
                arg = "";
            }
            else
            {
                arg += ch;
            }
        }
        args.Add(arg);
        //Debug.Log("Arg: " + arg);

        switch (typeCode)
        {

            case 1:
                /* Arg1 = changeMusic? 0: false; 1: true, etc.
                *  Arg2 = fadeChangeMusic? 
                *  Arg3 = arg1 of function
                *  Arg4 = arg2 of function
                *  Arg5 = arg3 of function
                */
                //Debug.Log("Music Change:");
                if ((string)args[0] == "1")
                {
                    int arg1 = int.Parse((string)args[2]);
                    mus.changeMusic(arg1);
                }
                else if ((string)args[1] == "1")
                {
                    if (args.Count == 4)
                    {
                        //Fade and Replace
                        float arg1 = float.Parse((string)args[2]);
                        int arg2 = int.Parse((string)args[3]);
                        Debug.Log("F/R: " + arg1 + ", " + arg2);
                        mus.runFadeReplace(arg1, arg2);

                    }
                }
                break;
            default:
                break;
        }


    }
}
