using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    //Scene name, overlyObject
    public Dictionary<string, GameObject> overlays = new Dictionary<string, GameObject>();


    public GameObject tutBox;
    public GameObject homeBox;
    public GameObject roadBox;
    public GameObject cityBox;
    public GameObject downtownBox;
    public GameObject pierBox;

    private void Start()
    {
        overlays.Add("Tutorial Area", tutBox);
        overlays.Add("Home", homeBox);
        overlays.Add("Road", roadBox);
        overlays.Add("City", cityBox);
        overlays.Add("Downtown", downtownBox);
        overlays.Add("Pier", pierBox);
    }

    public void UpdateMinimap(string sceneName)
    {
        foreach (GameObject overlay in overlays.Values)
        {
            overlay.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
        }
        overlays[sceneName].GetComponent<Image>().color = new Color32(240, 240, 70, 150);
    }
}
