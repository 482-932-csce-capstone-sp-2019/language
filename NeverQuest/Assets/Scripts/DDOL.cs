using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public string sceneChange = "";
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        if (sceneChange != "none" && sceneChange.Length > 0)
        {
            SceneManager.LoadScene(sceneChange);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 trans = player.transform.position;
            trans.x = -10.43f;
            trans.y = 5.97f;
            trans.z = 0.0f;
            player.transform.position = trans;

        }
    }
}
