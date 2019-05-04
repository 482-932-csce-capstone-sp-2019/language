using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CVMiniGameController : MonoBehaviour
{
    public GameObject cvObject;

    //Constant or vowel
    [HideInInspector] public string wants = "";
    private Dictionary<string, string> letterNumIsVowelOrCons;
    public int numVowelsDestroyed = 0;
    private PlayerController pc;
    private bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        wants = "C";
        letterNumIsVowelOrCons = new Dictionary<string, string>();
        for (int i = 1; i <= 10; i++)
        {
            letterNumIsVowelOrCons.Add(i.ToString(), "V");
        }

        for (int i = 11; i <= 24; i++)
        {
            letterNumIsVowelOrCons.Add(i.ToString(), "C");
        }


        //Vowels
        HashSet<KeyValuePair<int, int>> usedPos = new HashSet<KeyValuePair<int, int>>();
        int randx = Random.Range(-7, 7);
        int randy = Random.Range(-7, 6);

        for (int i = 1; i <= 10; i++)
        {
            do
            {
                randx = Random.Range(-7, 7);
                randy = Random.Range(-7, 6);
            }
            while (usedPos.Contains(new KeyValuePair<int, int>(randx, randy)));

            usedPos.Add(new KeyValuePair<int, int>(randx, randy));
            Vector3 tmpPos = new Vector3(randx, randy, 0);
            GameObject tmp = Instantiate(cvObject, tmpPos, Quaternion.identity);
            tmp.GetComponent<SpriteRenderer>().sprite = Resources.Load("StrokeOrder/" + i.ToString(), typeof(Sprite)) as Sprite;
            tmp.GetComponent<CVMiniGameObject>().num = i;
        }

        pc.Thought("", "I need to collect the vowels to restore the language");
    }

    private void Update()
    {
        if (numVowelsDestroyed == 10 && firstTime)
        {
            pc.Thought("", "Now I should get all the consanants");
            firstTime = false;

            HashSet<KeyValuePair<int, int>> usedPos = new HashSet<KeyValuePair<int, int>>();
            //Constanants
            int randx = Random.Range(-7, 7);
            int randy = Random.Range(-7, 6);

            for (int i = 11; i <= 24; i++)
            {
                if (i != 17 && i != 19 && i != 20)
                {
                    int playerCurrx;
                    int playerCurry;
                    do
                    {
                        playerCurrx = (int)GameObject.Find("Player").gameObject.transform.position.x;
                        playerCurry = (int)GameObject.Find("Player").gameObject.transform.position.y;
                        randx = Random.Range(-7, 7);
                        randy = Random.Range(-7, 6);
                    }
                    while (usedPos.Contains(new KeyValuePair<int, int>(randx, randy)) || randx == playerCurrx || randx == playerCurrx - 1 || randx == playerCurrx + 1 || randy == playerCurry || randy == playerCurry - 1 || randy == playerCurry + 1);

                    usedPos.Add(new KeyValuePair<int, int>(randx, randy));
                    Vector3 tmpPos = new Vector3(randx, randy, 0);
                    GameObject tmp = Instantiate(cvObject, tmpPos, Quaternion.identity);
                    tmp.GetComponent<SpriteRenderer>().sprite = Resources.Load("StrokeOrder/" + i.ToString(), typeof(Sprite)) as Sprite;
                    tmp.GetComponent<CVMiniGameObject>().num = i;
                }
            }
        }

        // Finished Tutorial
        if (numVowelsDestroyed >= 21)
        {
            pc.Thought("", "Now I know all the characters! I should bring Sarah a flower.");
            pc.Thought("", "I should bring Sarah a flower.");
            pc.unlockRoom("Road");
            numVowelsDestroyed = 0;
        }
    }
}
