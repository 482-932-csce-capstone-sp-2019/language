using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public AudioSource aud;

    public float Volume;
    public AudioClip bm1;
    public AudioClip bm2;
    public AudioClip bm3;
    public AudioClip bm4;
    public AudioClip bm5;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        aud.clip = bm2;
        aud.volume = Volume;
        Debug.Log("Playing Music");
        aud.Play();
        
    }

    void Pause()
    {
        aud.Pause();
    }

    void Play()
    {
        aud.Play();
    }

    public IEnumerator FadeIn(float FadeTime)
    {
        float endVolume = Volume;
        aud.volume = 0;
        aud.Play();
        while (aud.volume < endVolume)
        {
            aud.volume += 1.0f * Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float FadeTime, AudioClip replacement)
    {
        float endVolume = Volume;
        aud.volume = 0;
        aud.clip = replacement;
        aud.Play();
        while(aud.volume < endVolume)
        {
            aud.volume +=  Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float FadeTime, int replacementI)
    {
        float endVolume = Volume;
        aud.volume = 0.0f;
        changeMusic(replacementI);
        //Debug.Log("aud.volume " + aud.volume);
        //Debug.Log("endVolume " + endVolume);
        while (aud.volume < endVolume)
        {
            aud.volume +=  Time.deltaTime / FadeTime;
            //Debug.Log("Volume" + aud.volume);
            yield return null;
        }
    }

 


    
    public IEnumerator FadeOut(float FadeTime)
    {
        float startVolume = aud.volume;
        while (aud.volume > 0)
        {
            aud.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        aud.Stop();
    }

    public IEnumerator FadeOut(float FadeTime, int replacement)
    {
        float startVolume = aud.volume;
        while (aud.volume > 0)
        {
            aud.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        aud.Stop();
        StartCoroutine(FadeIn(FadeTime, replacement));
    }

    public void runFadeOut(float fadeTime)
    {
        StartCoroutine(FadeOut(fadeTime));
    }
    public void runFadeReplace(float fadeTime, int replacement)
    {
        StartCoroutine(FadeOut(fadeTime, replacement));
    }

    // Update is called once per frame

    public void changeMusic(int index)
    {
        AudioClip clip;
        switch (index)
        {
            case (2): clip = bm2;
                break;//etc.
            default:  clip = bm1;
                break;
        }
        aud.clip = clip;
        aud.Play();
        aud.loop = true;
    }

    void changeMusic(AudioClip clip)
    {
        aud.clip = clip;
        aud.volume = Volume;
        aud.Play();
    }
}
