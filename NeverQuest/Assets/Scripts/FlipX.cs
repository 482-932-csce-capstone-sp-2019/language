using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipX : MonoBehaviour
{
    public void flip()
    {
        float x = -gameObject.transform.localScale.x;
        float y = gameObject.transform.localScale.y;
        float z = gameObject.transform.localScale.z;
        Vector3 vec = new Vector3(x, y, z);
        gameObject.transform.localScale = vec;
            
    }
}
