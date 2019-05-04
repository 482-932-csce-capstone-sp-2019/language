using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public void makePopUp(string name)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("PopUp/" + name, typeof(Sprite)) as Sprite;
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 2f, Space.World);

            Color color = GetComponent<SpriteRenderer>().material.color;

            if (color.a <= 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                color.a -= 0.002f;
                GetComponent<SpriteRenderer>().material.color = color;
            }
        }
    }
}
