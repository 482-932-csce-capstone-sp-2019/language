using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBaddies : MonoBehaviour
{
    public GameObject baddyPrefab;

    private PlayerController playerController;
    private int i;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        i = 10;
        StartCoroutine(SpawnGuy());
    }

    IEnumerator SpawnGuy()
    {
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(1.0f);
        }
        if (i >= 10)
        {
            if (!playerController.writing)
            {
                GameObject newBaddy = Instantiate(baddyPrefab, transform.position, Quaternion.identity) as GameObject;
                //GameObject newBaddy = Instantiate(baddyPrefab) as GameObject;
                //newBaddy.transform.position = transform.position;
            }
            i = 0;
            StartCoroutine(SpawnGuy());
            yield return null;
        }
    }

}
