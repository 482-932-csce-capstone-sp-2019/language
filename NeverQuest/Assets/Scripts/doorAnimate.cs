using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class doorAnimate : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public bool advance;
    public bool scene = false;

    private AudioSource sound;
    private GameObject player;

    void OnCollisionEnter2D(Collision2D col)
    {
        sound.Play();
        anim.SetBool("Enter", true);

    }
    void Start()
    {
        anim.SetBool("Enter", false);
        sound = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (scene)
        {
            changeScene();
        }
    }
    void changeScene()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        string dest = GetComponent<Text>().text;
        Debug.Log(dest);
        cam.GetComponent<sceneChange>().flipPlayer(true);
        cam.GetComponent<sceneChange>().changeScene(dest);
        player.GetComponent<PlayerController>().switched = true;
    }
}
