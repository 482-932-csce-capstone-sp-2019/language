using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour
{

    public void changeScene(string sceneName)
    {
      GameObject obj = GameObject.FindGameObjectWithTag("Player");
      PlayerController player = obj.GetComponent<PlayerController>();

      if(!player.writing) {
        player.currentSpeed = 0;
        player.buttonClicked = true;

        GameObject[] baddies = GameObject.FindGameObjectsWithTag("Baddy");

        foreach (GameObject baddy in baddies)
        {
            Destroy(baddy);
        }

        SceneManager.LoadScene(sceneName);
      }
        GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
        Camera.GetComponent<FollowPlayer>().reset();
    }
    // 0 means flip x, 1 means flip y
    public void flipPlayer(bool vertical)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        PlayerController player = obj.GetComponent<PlayerController>();
        float newX = player.transform.position.x < 0? -(player.transform.position.x + 1.5f) : -(player.transform.position.x - 1.5f);
        float newY = player.transform.position.y < 0? -(player.transform.position.y + 1.5f) : -(player.transform.position.y - 1.5f);


        if(!player.writing) {
          if (vertical == false)
          {
              player.transform.position = new Vector3(newX, player.transform.position.y, 0);
          }

          else if (vertical == true)
          {
              player.transform.position = new Vector3(transform.position.x, newY, 0);
          }

          player.buttonClicked = false;
      }
    }
}
