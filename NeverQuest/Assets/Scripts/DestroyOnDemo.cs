using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().demo)
        {
            if (gameObject)
            {
                gameObject.SetActive(false);
            }
            GameObject mask = GameObject.FindGameObjectWithTag("Mask");
            if (mask)
            {
                mask.SetActive(false);
            }
        }
    }

}
