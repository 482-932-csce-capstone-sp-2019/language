using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameHolder : MonoBehaviour
{
    public string name;

    public void updateName(string n)
    {
        name = n;
    }

    public void updateName(Text n)
    {
        name = n.text;
    }

    public string getName()
    {
        return name;
    }
}
