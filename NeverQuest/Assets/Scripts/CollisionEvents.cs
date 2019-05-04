using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
    public int id;
    public GameObject SoundObj;

    private AudioSource aud;
    /*
     * 0: Play Sound attached to object
     * 1: Etc.
    */
    void Start()
    {
        aud = SoundObj.GetComponent<AudioSource>();
    }
    public void OnCollisionEnter2D(Collision2D coll)
    {
        //Debug.Log("Collider " + coll.gameObject.name + " detected");
        switch (id)
        {
            case 0:
                //GameObject.DontDestroyOnLoad(this.gameObject);
                //AudioSource aud = SoundObj.GetComponent<AudioSource>();
                //aud.Play();
                Debug.Log("Playing DoorSound");
                //StartCoroutine(DestroyAfter(1f));
                break;
            default:
                return;
                
        }
    }

    IEnumerator DestroyAfter(float s)
    {
        while (s > 0)
        {
            s = 0;
            yield return new WaitForSeconds(s);
        }
        Destroy(this.gameObject);
        yield return null;

    }
}
