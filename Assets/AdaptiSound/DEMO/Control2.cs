using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AdaptiSound; // use this or AdaptiSound.AudioManager.Instance.play_music("track_name");
namespace AdaptiSound
{

public class Control2 : MonoBehaviour
{

    public Button playButton;
    public Button loop1;
    public Button loop2;
    public Toggle layer1;
    public Toggle layer2;
    public Toggle layer3;
    public Button outroButton;
    public Button stopButton;


    private void Start() 
    {

        playButton.onClick.AddListener(onPlayButton);
        loop1.onClick.AddListener(onLoopButton1);
        outroButton.onClick.AddListener(onOutroButton);
        stopButton.onClick.AddListener(onStopButton);

        layer1.onValueChanged.AddListener(onLayer1);
        layer2.onValueChanged.AddListener(onLayer2);
        layer3.onValueChanged.AddListener(onLayer3);

    }

    private void onPlayButton()
    {
        AudioManager.Instance.playMusic("Theme2");
    }

    private void onLoopButton1()
    {
        AudioManager.Instance.changeLoop("Theme2", 0);
    }

    private void onOutroButton()
    {
        AudioManager.Instance.toOutro("Theme2");
    }

    private void onStopButton()
    {
        AudioManager.Instance.stopMusic(true);
    }

    private void onLayer1(bool value)
    {
        if (value)
        {
            AudioManager.Instance.muteLayer("Theme2", 0, false);
        }
        else
        {
            AudioManager.Instance.muteLayer("Theme2", 0, true);
        }
        
    }

    private void onLayer2(bool value)
    {
        if (value)
        {
            AudioManager.Instance.muteLayer("Theme2", 1, false);
        }
        else
        {
            AudioManager.Instance.muteLayer("Theme2", 1, true);
        }
    }

    private void onLayer3(bool value)
    {
        if (value)
        {
            AudioManager.Instance.muteLayer("Theme2", 2, false);
        }
        else
        {
            AudioManager.Instance.muteLayer("Theme2", 2, true);
        }
    }
}

}
