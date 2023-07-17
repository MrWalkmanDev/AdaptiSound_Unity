using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

using System.Linq;

public class AdaptiNode : MonoBehaviour
{
    [SerializeField] private AudioClip intro_clip;
    
    [SerializeField] private LoopClass[] loops;

    [System.Serializable]
    public class LoopClass
    {
        public ParallelLayer[] loop_layers;
        //public int[] initial_layers;
        public int bpm;
        public int metric;
        public int total_beat_count;
        public int[] keys_loop_measure;
        public int[] keys_to_outro_measure;
    }

    [SerializeField] private AudioClip outro_clip;

    [System.Serializable]
    public class ParallelLayer
    {
        public AudioClip clip;
        public string layer_name;
        //public bool always = true;
        public bool mute;
    }


    // AudioSources //
    private Adapti_AudioSource intro_source;
    private Adapti_AudioSource outro_source;
    private Dictionary<int, Adapti_AudioSource[]> loops_sources = new Dictionary<int, Adapti_AudioSource[]>();

    private Coroutine intro_coroutine;
    private Coroutine outro_coroutine;


    // Propeties //
    private float volume = 1.0f;
    //private float pitch;
    public AudioMixerGroup bus;


    // Playback //
    private int initial_loop = 0;
    private Adapti_AudioSource current_playback = null;
    private bool can_change_track = false;
    private bool can_end_track = false;
    private int loop_target;

    private float fade_out_for_measure_method = 0.7f;
    private float fade_in_for_measure_method = 0.7f;

    // Beat Count System //
    private float song_position = 0.0f;
    private int song_position_in_beats = 1;
    private float sec_per_beat = 0.0f;
    private int last_reported_beat = 0;
    private int beats_before_start = 0;

    private bool can_beat = false;
    private bool can_first_beat = true;
    private int _beat = 0;
    private int beat_measure_count = 1;
    private int measures = 1;



    private void Awake() {

        sec_per_beat = 60.0f / loops[initial_loop].bpm;

        // INTRO //
        if(intro_clip != null)
        {
            intro_source = gameObject.AddComponent<Adapti_AudioSource>();
            intro_source.audio_source.clip = intro_clip;
        }


        // LOOPS //
        int index = 0;
        foreach (LoopClass one_loop in loops)
        {
            Adapti_AudioSource[] layers = new Adapti_AudioSource[one_loop.loop_layers.Length];

            for (int i = 0; i < one_loop.loop_layers.Length; i++)
            {
                ParallelLayer layer = one_loop.loop_layers[i];

                Adapti_AudioSource audioSource = gameObject.AddComponent<Adapti_AudioSource>();
                audioSource.audio_source.outputAudioMixerGroup = bus;
                audioSource.audio_source.clip = layer.clip;
                layers[i] = audioSource;
            }

            loops_sources[index] = layers;

            index++;

        }

        // OUTRO //
        if(outro_clip != null)
        {
            outro_source = gameObject.AddComponent<Adapti_AudioSource>();
            outro_source.audio_source.clip = outro_clip;
        }
    }
    

    // Beat Count System //
    private void Update() {
        if (current_playback != intro_source && current_playback != outro_source)
        {
            if (current_playback != null)
            {
                song_position = current_playback.audio_source.time;
                song_position_in_beats = Mathf.FloorToInt(song_position / sec_per_beat) + beats_before_start;
                int loop_index = get_loop_index();
                /*int total_song_position_in_beat = Mathf.FloorToInt(current_playback.audio_source.clip.length / sec_per_beat);
                if (total_song_position_in_beat % loops[loop_index].metric != 0)
                {
                    total_song_position_in_beat += 1;
                }*/
                //Debug.Log(total_song_position_in_beat);
                beat_report(loop_index);
            }
        }
    }

    private void beat_report(int loop_index)
    {
        if (song_position_in_beats <= 0 && last_reported_beat == loops[loop_index].total_beat_count - 1 && can_first_beat)
        {
            can_first_beat = false;
            _beat = 0;
            beat_measure_count = 1;
            measures = 1;
            AudioManager.print("log", "Measure count: 1 Loop");
            change_measure_tracks();
        }

        if (_beat < song_position_in_beats)
        {
            can_beat = true;
        }

        if (can_beat)
        {
            can_beat = false;
            _beat += 1;
            beat_measure_count += 1;
            if (beat_measure_count > loops[loop_index].metric)
            {
                measures += 1;
                beat_measure_count = 1;
                can_first_beat = true;
                change_measure_tracks();
                AudioManager.print("log", "Measure count: " + measures);
            }
            last_reported_beat = song_position_in_beats;
        }
    }

    private void change_measure_tracks()
    {
        int current_loop_index = get_loop_index();
        bool contain_key = loops[current_loop_index].keys_loop_measure.Contains(measures);

        // Change Loops by Keys //
        if (contain_key && can_change_track)
        {
            change_track(loop_target, fade_out_for_measure_method, fade_in_for_measure_method);
            return;
        }

        bool contain_key2 = loops[current_loop_index].keys_to_outro_measure.Contains(measures);   
        // Change to End by Keys //
        if (contain_key2 && can_end_track)
        {
            can_end_track = false;
            change_outro(current_loop_index, fade_out_for_measure_method, fade_in_for_measure_method);
            return;
        }    
    }



    public void on_play(float duration, float targetVolume, bool fadeType, int loop_index = 0)
    {

        volume = targetVolume;

        if (intro_clip != null)
        {
            intro_source.StartFade(duration, targetVolume, fadeType);
            intro_source.audio_source.loop = false;
            intro_source.audio_source.outputAudioMixerGroup = bus;
            intro_source.audio_source.Play();
            intro_coroutine = StartCoroutine(intro_finish(loop_index));

            current_playback = intro_source;
        }
        else
        {
            current_playback = loops_sources[loop_index][0];
            change_track(loop_index, 0.0f, duration);
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
        if (loop_key == -1)
        {
            AudioManager.print("warning", "Loop not playing");
            return;
        }


        if (layer > (loops_sources[loop_key].Length - 1))
        {
            AudioManager.print("warning", "Invalid layer index");
            return;
        }

        if (fade_type)
        {
            if (fade_time != 0.0f)
            {
                loops_sources[loop_key][layer].StartFade(fade_time, 0.0f, false, false);
            }
            else
            {
                loops_sources[loop_key][layer].audio_source.volume = 0.0f;
                loops_sources[loop_key][layer].stop_coroutine();
            }
           
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
                loops_sources[loop_key][layer].stop_coroutine();
                //loops_sources[loop_key][layer].StartFade(fade_time, volume, true, false);
            }
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
            can_change_track = false;
            AudioManager.print("log", "Loop continue");
            return;
        }

        fade_in_for_measure_method = fade_in;
        fade_out_for_measure_method = fade_out;

        if (loops[loop_index].keys_loop_measure.Length > 0)
        {
            loop_target = loop_index;
            can_change_track = true;
            AudioManager.print("log", "Prepare loop change");
        }
        else
        {
            change_track(loop_index, fade_out, fade_in);
        }
    }


    private void change_track(int loop_index, float fade_out, float fade_in)
    {
        // Stop Current Loop //
        int loop_key = get_loop_index();
        if (loop_key == -1)
        {
            AudioManager.print("log", "Loop not playing");
            return;
        }
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

        can_change_track = false;
        reset_loop_values(loop_index);

        // If ParallelTrack //
        if (loops[loop_index].loop_layers.Length > 1)
        {
            for (int i = 0; i < loops[loop_index].loop_layers.Length; i++)
            {
                if (loops[loop_index].loop_layers[i].mute)
                {
                    loops_sources[loop_index][i].audio_source.volume = 0.0f;
                }
                else
                {
                    loops_sources[loop_index][i].audio_source.volume = volume;
                }
                
                loops_sources[loop_index][i].audio_source.outputAudioMixerGroup = bus;

                if (fade_time != 0.0f)
                {
                    if (!loops[loop_index].loop_layers[i].mute)
                    {
                        loops_sources[loop_index][i].audio_source.volume = 0.0f;
                        loops_sources[loop_index][i].StartFade(fade_time, volume, true);
                    }
                    loops_sources[loop_index][i].audio_source.Play();
                }
                else
                {
                    loops_sources[loop_index][i].stop_coroutine();
                    loops_sources[loop_index][i].audio_source.Play();
                }
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
                loops_sources[loop_index][0].stop_coroutine();
                loops_sources[loop_index][0].audio_source.Play();
            }
        }

        current_playback = loops_sources[loop_index][0];
    }



    public void on_outro(float fade_out, float fade_in)
    {
        int loop_index = get_loop_index();

        if (loop_index == -1)
        {
            can_end_track = false;
            AudioManager.print("warning", "No loop playing");
            return;
        } 

        can_change_track = false;
        fade_in_for_measure_method = fade_in;
        fade_out_for_measure_method = fade_out;

        if (loops[loop_index].keys_to_outro_measure.Length > 0)
        {
            can_end_track = true;
            AudioManager.print("log", "Prepare to outro change");
        }
        else
        {
            change_outro(loop_index, fade_out, fade_in);
        }
    }

    private void change_outro(int loop_index, float fade_out, float fade_in)
    {
        outro_source.audio_source.volume = volume;
        
        Adapti_AudioSource[] current_track = loops_sources[loop_index];
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

        // Play Outro //
        if (fade_in != 0.0f)
        {
            outro_source.audio_source.volume = 0.0f;
            outro_source.StartFade(fade_in, volume, true);
        }
        outro_source.audio_source.loop = false;
        outro_source.audio_source.outputAudioMixerGroup = bus;
        outro_source.audio_source.Play();

        current_playback = outro_source;

        outro_coroutine = StartCoroutine(outro_finish());
    }


    IEnumerator outro_finish()
    {
        yield return new WaitForSeconds(outro_clip.length);

        on_stop();
    }



    public void on_stop(bool can_fade = false, float fade_out = 1.5f)
    {
        if (intro_coroutine != null)
        {
            StopCoroutine(intro_coroutine);
        }
        if (outro_coroutine != null)
        {
            StopCoroutine(outro_coroutine);
        }

        // Stop Intro //
        if (fade_out != 0.0f)
        {
            intro_source.StartFade(fade_out, 0.0f, false);
        }
        else
        {
            intro_source.audio_source.Stop();
        } 

        // Stop All Loops //
        foreach (KeyValuePair<int, Adapti_AudioSource[]> entry in loops_sources)
        {
            foreach (Adapti_AudioSource source in loops_sources[entry.Key])
            {
                if (fade_out != 0.0f)
                {
                    source.StartFade(fade_out, 0.0f, false);
                }
                else
                {
                    source.audio_source.Stop();
                }
            }
        }

        // Stop Outro //
        if (fade_out != 0.0f)
        {
            outro_source.StartFade(fade_out, 0.0f, false);
        }
        else
        {
            outro_source.audio_source.Stop();
        } 

        current_playback = null;


        // If AudioManager no current playback //
        string objectName = gameObject.name;
        if (objectName.Contains("(Clone)"))
        {
            objectName = objectName.Replace("(Clone)", "");
        }
        if (AudioManager.Instance.current_playback == objectName)
        {
            AudioManager.Instance.current_playback = "";
        }
    }



    private void reset_loop_values(int loop_index)
    {
        beat_measure_count = 1; // Problem Maybe

        measures = 1;
        last_reported_beat = 0;
        song_position_in_beats = 0;
        _beat = 0;
        sec_per_beat = 60.0f / loops[loop_index].bpm;
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
