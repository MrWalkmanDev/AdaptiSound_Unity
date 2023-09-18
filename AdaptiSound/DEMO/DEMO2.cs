using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdaptiSound
{

public class DEMO2 : MonoBehaviour
{
    public Button battle;
    public Button exit_battle;

    [HideInInspector] public int area = 0;

    void Start()
    {
        AudioManager.Instance.playMusic("Theme3");

        battle.onClick.AddListener(onEnterBattle);
        exit_battle.onClick.AddListener(onExitBattle);
    }

    private void onEnterBattle()
    {
        AudioManager.Instance.playMusic("Theme1");
    }

    private void onExitBattle()
    {
        AudioManager.Instance.toOutro("Theme1");
        AudioManager.Instance.setSequence("Theme3");

        AdaptiNode track = AudioManager.Instance.get_abgm_track("Theme1");
        if (track != null)
        {
            track.endMusicSignal.AddListener(onEndMusic);
        }
    }

    private void onEndMusic()
    {
        AudioManager.Instance.muteLayer("Theme3", area, false);
    }
}

}
