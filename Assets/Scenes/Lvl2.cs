using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lvl2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       AudioManager.Instance.play_music("JazzBase", 1.0f, 0.5f, 1.0f);
       //AudioManager.Instance.stop_music();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Scene1");
        }
    }
}
