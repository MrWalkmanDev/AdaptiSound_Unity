using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AdaptiNode : MonoBehaviour
{
    [SerializeField] private AudioClip intro_clip;
    
    [SerializeField] private LoopClass[] loops;

    [System.Serializable]
    public class LoopClass
    {
        [SerializeField] public AudioClip[] loop_layers;
        [SerializeField] public int[] initial_layers;
        [SerializeField] public int bpm;
        [SerializeField] public int metric;
        [SerializeField] public int total_beat_count;
    }

    [SerializeField] private AudioClip outro_clip;


    // AudioSources //
    private Adapti_AudioSource intro_source;
    private Adapti_AudioSource outro_source;
    private Dictionary<int, Adapti_AudioSource[]> loops_sources = new Dictionary<int, Adapti_AudioSource[]>();

    private Coroutine intro_coroutine;


    // Propeties //
    private float volume = 1.0f;
    //private float pitch;
    public AudioMixerGroup bus;


    // Playback //
    private int initial_loop = 0;
    private Adapti_AudioSource current_playback = null;


    // Beat Count System //


    private void Awake() {

        // INTRO //
        if(intro_clip != null)
        {
            intro_source = gameObject.AddComponent<Adapti_AudioSource>();
            //intro_source.audio_source = gameObject.AddComponent<AudioSource>();
            intro_source.audio_source.clip = intro_clip;
        }


        // LOOPS //
        int index = 0;
        foreach (LoopClass one_loop in loops)
        {
            Adapti_AudioSource[] layers = new Adapti_AudioSource[one_loop.loop_layers.Length];

            for (int i = 0; i < one_loop.loop_layers.Length; i++)
            {
                AudioClip layer = one_loop.loop_layers[i];

                Adapti_AudioSource audioSource = gameObject.AddComponent<Adapti_AudioSource>();
                //audioSource.audio_source = gameObject.AddComponent<AudioSource>();
                audioSource.audio_source.outputAudioMixerGroup = bus;
                audioSource.audio_source.clip = layer;
                layers[i] = audioSource;
            }

            loops_sources[index] = layers;

            index++;

        }

        /*foreach (int key in loops_sources.Keys)
        {
            Debug.Log(key);
        }*/

        // OUTRO //
        if(outro_clip != null)
        {
            outro_source = gameObject.AddComponent<Adapti_AudioSource>();
            //outro_source.audio_source = gameObject.AddComponent<AudioSource>();
            outro_source.audio_source.clip = outro_clip;
        }
    }

    public void on_play(float duration, float targetVolume, bool fadeType)
    {

        volume = targetVolume;

        if (intro_clip != null)
        {
            intro_source.StartFade(duration, targetVolume, fadeType);
            intro_source.audio_source.loop = false;
            intro_source.audio_source.outputAudioMixerGroup = bus;
            intro_source.audio_source.Play();
            intro_coroutine = StartCoroutine(intro_finish(initial_loop));

            current_playback = intro_source;
        }
        else
        {
            return;
        }
    }

    IEnumerator intro_finish(int loop_index)
    {
        yield return new WaitForSeconds(intro_clip.length);

        play_loop(loop_index, 0.0f);
    }



    public void mute_layer(int layer, float fade_time = 2.0f, bool fade_type = true)
    {
        // Get Loop Source Index
        int loop_key = get_loop_index();


        if (layer > (loops_sources[loop_key].Length - 1))
        {
            AudioManager.print("warning", "Invalid layer index");
            return;
        }

        if (fade_type)
        {
            loops_sources[loop_key][layer].StartFade(fade_time, 0.0f, false, false);
            
            /*foreach (int i in layers)
            {
            loops_sources[0][i].StartFade(fade_time, 0.0f, false);
            }*/
        }
        else
        {
            if (fade_time != 0.0f)
            {
                loops_sources[loop_key][layer].StartFade(fade_time, volume, true, false);
            }
            else
            {
                loops_sources[loop_key][layer].audio_source.volume = volume;
                loops_sources[loop_key][layer].StartFade(fade_time, volume, true, false);
            }
            
            /*foreach (int i in layers)
            {
            loops_sources[0][i].StartFade(fade_time, 1.0f, true);
            }*/
        }
    }



    public void change_loop(int loop_index, float fade_out = 0.5f, float fade_in = 0.5f)
    {
        if (loop_index > (loops_sources.Keys.Count - 1))
        {
            AudioManager.print("warning", "Invalid loop index");
            return;
        }

        if (current_playback == loops_sources[loop_index][0])
        {
            AudioManager.print("log", "Loop continue");
            return;
        }

        // Stop Current Loop //
        int loop_key = get_loop_index();
        Adapti_AudioSource[] current_track = loops_sources[loop_key];
        foreach (Adapti_AudioSource layer in current_track)
        {
            if (fade_out != 0.0f)
            {
                layer.StartFade(fade_out, 0.0f, false);
            }
            else
            {
                layer.audio_source.Stop();
            }
        }

        // Play New Loop //
        play_loop(loop_index, fade_in);
    }



    private void play_loop(int loop_index, float fade_time)
    {
        // If ParallelTrack //
        if (loops[loop_index].loop_layers.Length > 1)
        {
            for (int i = 0; i < loops[loop_index].loop_layers.Length; i++)
            {
                loops_sources[loop_index][i].audio_source.volume = 0.0f;
                loops_sources[loop_index][i].audio_source.outputAudioMixerGroup = bus;

                if (fade_time != 0.0f)
                {
                    loops_sources[loop_index][i].StartFade(fade_time, volume, true);
                    loops_sources[loop_index][i].audio_source.Play();
                }
                else
                {
                    loops_sources[loop_index][i].audio_source.Play();
                }
            }

            foreach (int i in loops[loop_index].initial_layers)
            {
                mute_layer(i, 0.0f, false);
            }
        }

        // If not ParallelTrack //
        else                                                                                
        {
            loops_sources[loop_index][0].audio_source.volume = volume;
            loops_sources[loop_index][0].audio_source.outputAudioMixerGroup = bus;
            if (fade_time != 0.0f)
            {
                loops_sources[loop_index][0].StartFade(fade_time, volume, true);
                loops_sources[loop_index][0].audio_source.Play();
            }
            else
            {
                loops_sources[loop_index][0].audio_source.Play();
            }
        }

        current_playback = loops_sources[loop_index][0];
    }



    public void on_outro()
    {
        
    }



    public void on_stop(bool can_fade = false, float fade_out = 1.5f)
    {
        
    }



    private int get_loop_index()
    {
        int loop_key = -1;
        foreach (int key in loops_sources.Keys)
        {
            if (loops_sources[key][0] == current_playback)
            {
                loop_key = key;
                break;
            }
        }
        if (loop_key == -1)
        {
            AudioManager.print("log", "Loop is not playing");
        }

        return loop_key;
    }
}
