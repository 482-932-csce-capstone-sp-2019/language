using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public int langNum = 0;
    [HideInInspector] public int orientation; //0: down; 1: left; 2: up; 3: right;
    [HideInInspector] public Dictionary<int, string> languages = new Dictionary<int, string>();
    [HideInInspector] public bool buttonClicked;
    [HideInInspector] public Dictionary<string, Dictionary<string, Quest>> inProgressQuests;
    [HideInInspector] public Dictionary<string, bool> rooms;
    [HideInInspector] public Dictionary<string, string> reasons;
    [HideInInspector] public string currScene = "Home";
    [HideInInspector] public bool switched = false;
    [HideInInspector] public float health = 100f;
    [HideInInspector] public GameObject menu;
    [HideInInspector] public HashSet<PlayerDictionary.Word> wordsSeen;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public bool writing = false;
    [HideInInspector] public bool inConvo = false;

    public bool demo;
    public float speed;

    public Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;
    public bool shouldAnimate;
    public bool frozen;
    public bool taco;

    private GameObject mask;
    private bool mouseDown;
    private Vector3 target;
    private Vector3 animate;
    private GameObject messageBox;

    void Start()
    {
        if (GameObject.Find("MasterCanvas") == null)
        {
            Destroy(this.gameObject);
            SceneManager.LoadScene("HUD");
        }
        maxSpeed = speed * 1.5f;
        mask = GameObject.FindGameObjectWithTag("Mask");

        shouldAnimate = true;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        inProgressQuests = new Dictionary<string, Dictionary<string, Quest>>();
        rooms = new Dictionary<string, bool>();
        reasons = new Dictionary<string, string>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        wordsSeen = new HashSet<PlayerDictionary.Word>();

        // Player is already created
        if (objs.Length > 1)
        {
            Debug.Log("objs.Length exceeded 1");
            Destroy(this.gameObject);
        }

        languages.Add(1, "Korean");
        languages.Add(2, "Mandarin");
        languages.Add(3, "Spanish");

        DontDestroyOnLoad(this.gameObject);

        //initialize target to player position
        target = transform.position;
        messageBox = GameObject.FindGameObjectWithTag("MessageBox");
        messageBox.GetComponent<MessageHandler>().Deactivate();

        InitializeDictionary();

        menu = GameObject.Find("Active Menu");
    }


    void Update()
    {
        if (frozen || writing || inConvo)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            GameObject[] objs = GameObject.FindGameObjectsWithTag("GUI");   //DO NOT REMOVE

            StartCoroutine(movePlayer(currScene));
            //shouldAnimate = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            StopCoroutine(movePlayer(currScene));
            animator.SetFloat("Magnitude", 0);
        }
    }

    public IEnumerator movePlayer(string sceneMov)
    {
        while (mouseDown && !frozen && !writing && !inConvo && sceneMov.Equals(currScene))
        {
            shouldAnimate = false;
            if (!(speed < 0.01))
            {
                shouldAnimate = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                shouldAnimate = false;
                //yield return null;
            }
            //currentSpeed = speed;

            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Set target z to equal player z
            target.z = transform.position.z;
            //shouldAnimate = true;
            //agent.SetDestination(target);

            switched = false;
            int dir = vecDirection(target - transform.position);
            //int dir = vecDirection(agent.destination - transform.position);
            UpdateOrientation(dir);
            animate = target - transform.position;

            currentSpeed = Mathf.Min(6 + animate.magnitude, maxSpeed);

            Vector3 movement = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
            transform.position = movement;

            //Animate the movement:
            animator.SetFloat("Horizontal", animate.x);
            animator.SetFloat("Vertical", animate.y);
            if (messageBox && messageBox.GetComponent<MessageHandler>())
            {
                if (shouldAnimate)
                {
                    messageBox.GetComponent<MessageHandler>().Deactivate();
                    float animateSpeed = Mathf.Min(currentSpeed - 6, 3.0f);
                    animator.SetFloat("Magnitude", animateSpeed);
                    //animator.SetFloat("Magnitude", animate.magnitude);
                }
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator UnfreezeAfter(float sec)
    {
        bool on = true;
        while (on)
        {
            on = false;
            yield return new WaitForSeconds(sec);
        }
        Unfreeze();
        yield return null;
    }


    public void Stop(bool freeze = false)
    {
        currentSpeed = 0;
        shouldAnimate = false;
        frozen = true;
    }

    public void StopFor(float sec)
    {

        frozen = true;
        shouldAnimate = false;
        currentSpeed = 0f;
        StartCoroutine(UnfreezeAfter(sec));
    }



    public void Unfreeze()
    {
        currentSpeed = 0.1f;
        shouldAnimate = true;
        writing = false;
        inConvo = false;
        frozen = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        currentSpeed = 0;
    }

    int vecDirection(Vector3 vec)
    {
        vec = Vector3.Normalize(vec);
        //Debug.Log("X: " + vec.x);
        //Debug.Log("Y: " + vec.y);
        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
        {
            if (vec.x > 0)
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (vec.y > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }

    public void UpdateOrientation(int orientation)
    {
        //Debug.Log(orientation);
        this.orientation = orientation;
        animator.SetInteger("Direction", orientation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Stop player movement
        currentSpeed = 0;
        animator.SetFloat("Magnitude", 0);

        shouldAnimate = false;

        if (collision.gameObject.CompareTag("NPC") || collision.gameObject.CompareTag("NPC-SelfDestruct"))
        {
            NpcController npcCont = collision.gameObject.GetComponent<NpcController>();
            inConvo = true;
            if (npcCont.shouldDestroy)
            {
                Debug.Log("Destroying NPC");
                collision.gameObject.SetActive(false);
                inConvo = false;
                shouldAnimate = true;
                return;
            }
            if (collision.gameObject.CompareTag("NPC-SelfDestruct"))
            {
                npcCont.shouldDestroy = true;
            }
            foreach (Dictionary<string, Quest> currQuestMap in inProgressQuests.Values)
            {
                foreach (Quest currQuest in currQuestMap.Values)
                {
                    //Already Have quest

                    if (npcCont.myName == currQuest.givenBy)
                    {
                        //Done
                        if (currQuest.completed())
                        {
                            //inProgressQuests[currQuest.type].Remove(currQuest.location);
                            if (GameObject.Find(currQuest.type + currQuest.location))
                            {
                                GameObject.Find(currQuest.type + currQuest.location).GetComponent<Image>().color = new Color32(150, 23, 94, 123);
                            }

                            OpenMessage();
                            npcCont.PlayConvoAfter();
                            unlockRoom(currQuest.roomUnlockedOnFinish);
                            Debug.Log(currQuest.roomUnlockedOnFinish + " unlocked!");
                            return;
                        }
                        //Middle
                        else
                        {
                            OpenMessage();
                            npcCont.PlayConvoDuring();
                            Debug.Log("Still need to do stuff!");
                            return;
                        }
                    }
                }
            }

            // First time
            OpenMessage();
            npcCont.PlayConvoBefore();

            if (npcCont.myName != "???")
                AddQuest(npcCont);
        }

        //Doors
        else if (collision.gameObject.CompareTag("Door") || collision.gameObject.CompareTag("VertDoor"))
        {
            string dest = collision.gameObject.GetComponent<Text>().text;

            if (rooms[dest])
            {
                if (collision.gameObject.CompareTag("Door") && !switched)
                {
                    StartCoroutine(Mask());
                    currScene = dest;
                    Debug.Log(dest);
                    GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                    cam.GetComponent<sceneChange>().flipPlayer(false);
                    cam.GetComponent<sceneChange>().changeScene(dest);
                    switched = true;

                    GameObject.FindGameObjectWithTag("GUI").GetComponentInChildren<MiniMap>().UpdateMinimap(dest);
                }
                //VERTICAL DOORS ARE HANDLED BY THE DOORS THEMSELVES
                else if (collision.gameObject.CompareTag("VertDoor") && !switched)
                {
                    StartCoroutine(Mask());
                    currScene = dest;
                    Debug.Log(dest);
                    GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                    cam.GetComponent<sceneChange>().flipPlayer(true);
                    cam.GetComponent<sceneChange>().changeScene(dest);
                    switched = true;

                    Debug.Log(dest);
                    GameObject.FindGameObjectWithTag("GUI").GetComponentInChildren<MiniMap>().UpdateMinimap(dest);

                }
            }
            else
            {
                Thought(dest);
                Debug.Log("DENIED accsess to: " + dest);
            }
        }
    }

    public void Thought(string room, string msg = "")
    {
        StartCoroutine(ThoughtBubble(room, msg));
    }

    IEnumerator ThoughtBubble(string room, string msg = "")
    {
        Stop(true);

        string convoline = "NOOOOOO";
        if (msg == "")
        {
            OpenMessage("%p (You)", reasons[room]);
        }
        else
        {
            OpenMessage("%p (You)", msg);
        }

        yield return new WaitForSeconds(2);

        Unfreeze();
    }

    IEnumerator Mask()
    {
        mask.SetActive(true);
        yield return new WaitForSeconds(.3F);
        mask.SetActive(false);
    }

    public void UpdateHealth()
    {
        GameObject.Find("RealHealthFront").transform.localScale = new Vector3(Mathf.Max(health / 100f, 0.0f), 1f, 1f);

        if (health <= 0)
        {
            print("Player dead");
            Application.Quit();
        }
    }

    //initializing room access and reasons
    private void InitializeDictionary()
    {
        reasons["Home"] = "";
        reasons["Tutorial Area"] = "I don't need to go into the side room right now.";
        reasons["Road"] = "What's the matter with Sarah?";
        reasons["City"] = "I need to get that flower for Sarah...";
        reasons["Downtown"] = "There was someone back at the road, I should ask if he needs anything before I go downtown...";
        reasons["Pier"] = "Not really the time to fish...";
        reasons["Restaurant"] = "No way the restaurant is open...";
        reasons["Grocery"] = "Hardly the time to buy groceries...";
        reasons["Field"] = "That's where the baddies are coming from, noted...";
        reasons["HQ"] = "There's something blocking the way, maybe if I fight more of the baddies...";

        if (!demo)
        {
            rooms["Home"] = true;
            rooms["Tutorial Area"] = false;
            rooms["Road"] = false;
            rooms["City"] = false;
            rooms["Downtown"] = false;
            rooms["Pier"] = false;
            rooms["Restaurant"] = false;
            rooms["Grocery"] = false;
            rooms["Field"] = false;
            rooms["HQ"] = false;
        }
        else
        {
            rooms["Home"] = true;
            rooms["Tutorial Area"] = true;
            rooms["Road"] = true;
            rooms["City"] = true;
            rooms["Downtown"] = true;
            rooms["Pier"] = true;
            rooms["Restaurant"] = true;
            rooms["Grocery"] = true;
            rooms["Field"] = true;
            rooms["HQ"] = true;
        }
    }

    public void unlockRoom(string scene)
    {
        if (scene != null)
        {
            rooms[scene] = true;
        }
    }

    public int numberOfQuests()
    {
        int cnt = 0;
        foreach (KeyValuePair<string, Dictionary<string, Quest>> currPair in inProgressQuests)
        {
            foreach (KeyValuePair<string, Quest> currr in currPair.Value)
            {
                cnt++;
            }
        }

        return cnt;
    }

    public void OpenMessage(string sender = "", string message = "")
    {
        messageBox.GetComponent<MessageHandler>().setSender(sender);
        messageBox.GetComponent<MessageHandler>().setMessage(message);
        messageBox.GetComponent<MessageHandler>().Activate();
    }

    public void AddQuest(NpcController npcCont)
    {
        unlockRoom(npcCont.roomUnlockedOnBegin);
        Debug.Log(npcCont.roomUnlockedOnBegin + " unlocked!");
        Debug.Log(npcCont.quest.location);

        Dictionary<string, Quest> tmp = new Dictionary<string, Quest>();
        if (npcCont.quest != null)
        {
            tmp.Add(npcCont.quest.location, npcCont.quest);

            if (!inProgressQuests.ContainsKey(npcCont.quest.type))
            {
                inProgressQuests.Add(npcCont.quest.type, tmp);
            }
            else
            {
                inProgressQuests[npcCont.quest.type].Add(npcCont.quest.location, npcCont.quest);
            }

            GameObject newQuest = Instantiate(npcCont.QuestGuiObject, new Vector3(0, 100 - (30 * (numberOfQuests() - 1)), 0), Quaternion.identity);
            newQuest.name = npcCont.quest.type + npcCont.quest.location;
            Debug.Log(menu);
            newQuest.transform.SetParent(menu.GetComponent<menuSwitch>().menu4.transform, false);
            newQuest.GetComponentInChildren<Text>().text = npcCont.quest.type + " in " + npcCont.quest.location + "! " + npcCont.quest.currentAmount + " out of " + npcCont.quest.goalAmount;
        }
    }
}
