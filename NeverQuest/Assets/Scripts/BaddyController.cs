using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaddyController : MonoBehaviour
{

    public GameObject blockPrefab;
    public Animator animator;
    public float speed;

    private GameObject player;
    [HideInInspector] public int wordIndex = 0;
    public string wordPoolName = "Verbs";
    private PlayerController playerController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        speed = 2.0f;
    }

    private void Update()
    {
        //PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (Vector2.Distance(transform.position, player.transform.position) > 1.2f)
        {
            //transform.position = Vector2.MoveTowards(transform.position, player.transform.position, .05f);

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 pos = transform.position;


            
            Vector3 movement = Vector3.MoveTowards(pos, player.transform.position, speed * Time.deltaTime);
            //Debug.Log("movement" + movement);
            Vector3 animate = player.transform.position - pos;
            animator.SetFloat("Horizontal", animate.x);
            animator.SetFloat("Vertical", animate.y);
            animator.SetFloat("Magnitude", movement.magnitude);
            pos = movement;
            transform.position = pos;
        }
        else if (!playerController.writing)
        {
            // wordPoolName = RandomValues(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict);

            List<PlayerDictionary.Word> words = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDictionary>().playerDict[wordPoolName];
            playerController.Stop(true);
            
            wordIndex = Random.Range(0, words.Count);
            PlayerDictionary.Word word = words[wordIndex];

            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Write();
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().expectedWord = word;
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().makeTraceBoxes();
            GameObject.Find("TextYet").GetComponent<Text>().text = word.koreanText;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().wordsSeen.Add(word);
            GameObject.Find("TopMostText").GetComponent<Text>().text = "\"" + word.translation + "\"";
        }
    }

    public void handleSubmit()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController.inProgressQuests.ContainsKey("Kill") &&
            playerController.inProgressQuests["Kill"].ContainsKey(playerController.currScene) &&
            playerController.inProgressQuests["Kill"][playerController.currScene].completed() == false)
        {
            playerController.inProgressQuests["Kill"][playerController.currScene].currentAmount++;
            if (playerController.inProgressQuests["Kill"].ContainsKey(playerController.currScene) && GameObject.Find("Kill" + playerController.currScene) != null)
            {
                GameObject.Find("Kill" + playerController.currScene).GetComponentInChildren<Text>().text = "Kill in " + playerController.currScene + "! " + playerController.inProgressQuests["Kill"][playerController.currScene].currentAmount + " out of " + playerController.inProgressQuests["Kill"][playerController.currScene].goalAmount;
            }
        }
        if (GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().isGood)
        {
            GameObject.FindGameObjectWithTag("GUI").GetComponent<GuiController>().Close();
            playerController.Unfreeze();
            Destroy(gameObject);
        }
        else
        {
            // playerController.health -= 10;
            // Debug.Log("Health: " + playerController.health);
            // playerController.UpdateHealth();
        }
    }

    public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while(true)
        {
            yield return values[rand.Next(size)];
        }
    }

}
