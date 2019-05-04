using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseScript : MonoBehaviour
{


    public float speed;
    public Animator animator;
    public GameObject player;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 10.0f;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;

        //Debug.Log("pos" + pos);


        Vector3 movement = Vector3.MoveTowards(pos, player.transform.position, speed * Time.deltaTime);
        //Debug.Log("movement" + movement);
        Vector3 animate = player.transform.position - pos;
        animator.SetFloat("Horizontal", animate.x);
        animator.SetFloat("Vertical", animate.y);
        animator.SetFloat("Magnitude", movement.magnitude);
        pos = movement;
        transform.position = pos;
    }
}