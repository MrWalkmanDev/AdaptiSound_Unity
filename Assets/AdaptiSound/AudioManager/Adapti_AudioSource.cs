using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adapti_AudioSource : MonoBehaviour
{

    public AudioSource audio_source;
    public Coroutine fade_coroutine;

    private void Awake() {
        audio_source = gameObject.AddComponent<AudioSource>();
        audio_source.loop = true;
    }

    public void stop_coroutine()
    {
        if (fade_coroutine != null)
        {
            // Detener la coroutine anterior si existe
            StopCoroutine(fade_coroutine);
        }
    }

    public void StartFade(float duration, float targetVolume, bool fadeType, bool can_stop = true)
    {
        if (fade_coroutine != null)
        {
            // Detener la coroutine anterior si existe
            StopCoroutine(fade_coroutine);
        }

        fade_coroutine = StartCoroutine(StartFadeCoroutine(duration, targetVolume, fadeType, can_stop));
    }

    public IEnumerator StartFadeCoroutine(float duration, float targetVolume, bool fade_type, bool can_stop)
    {
        if (fade_type)
        {
            if (!audio_source.isPlaying)
            {
                audio_source.volume = 0.0f;
                //audio_source.Play();                // Only plays if track is not Playing
            }
        }

        float currentTime = 0;
        float start = audio_source.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audio_source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            //Debug.Log("Counre");

        if (audio_source.volume == 0.0f && can_stop)
        {
            audio_source.Stop();
        }
            yield return null;
        }
        yield break;
    }
}
