using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class NoMove : MonoBehaviour, IPointerDownHandler// required interface when using the OnPointerDown method.
{
    private PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !controller.inConvo && !controller.writing)
        {
            //Debug.Log("GO");
            controller.Unfreeze();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("STOP");
        controller.Stop(true);
    }
}
