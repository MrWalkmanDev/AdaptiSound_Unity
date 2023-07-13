using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    //public AudioClip myAudioClip;
    public string targetSceneName;

    void Start()
    {
        //AudioManager.Instance.play_music("Theme1", 1.0f, 2.0f, 2.0f);
        //AudioManager.Instance.PlayAudioClip(myAudioClip);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SceneManager.LoadScene(targetSceneName);
            AudioManager.Instance.play_music("Theme1", 1.0f, 2.0f, 2.0f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //AudioManager.Instance.mute_layer("Theme1", 2, 2.0f, false);
            AudioManager.Instance.change_loop("Theme1", 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //AudioManager.Instance.mute_layer("Theme1", 2, 2.0f, true);
            AudioManager.Instance.change_loop("Theme1", 0);
        }
    }
}
