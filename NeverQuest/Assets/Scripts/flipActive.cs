using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipActive : MonoBehaviour
{
    public GameObject closeButton;
    public void flip()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }
}
