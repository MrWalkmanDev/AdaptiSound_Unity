using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AdaptiSound; // use this or AdaptiSound.AudioManager.Instance.play_music("track_name");
namespace AdaptiSound
{

public class Control : MonoBehaviour
{

    public Button playButton;
    public Button loop1;
    public Button loop2;
    public Button outroButton;
    public Button stopButton;


    private void Start() 
    {

        playButton.onClick.AddListener(onPlayButton);
        loop1.onClick.AddListener(onLoopButton1);
        loop2.onClick.AddListener(onLoopButton2);
        outroButton.onClick.AddListener(onOutroButton);
        stopButton.onClick.AddListener(onStopButton);
    }

    private void onPlayButton()
    {
        AudioManager.Instance.playMusic("Theme1");
    }

    private void onLoopButton1()
    {
        AudioManager.Instance.changeLoop("Theme1", 0);
    }

    private void onLoopButton2()
    {
        AudioManager.Instance.changeLoop("Theme1", 1);
    }

    private void onOutroButton()
    {
        AudioManager.Instance.toOutro("Theme1");
    }

    private void onStopButton()
    {
        AudioManager.Instance.stopMusic(true);
    }
}

}