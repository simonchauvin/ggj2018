using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManipulation
{
    private AudioMixer masterMix;
    public AudioSource audioSource;
    /*
    public AudioLowPassFilter lowPass;
    public AudioHighPassFilter highPass;
    public AudioDistortionFilter distortion;
    */
    public float minStep = 2000;

    public IEnumerator fadeSound;

    float ecartMin = 500f;

    public float startVolume = 1f;
    public bool inFadeOut = false;
    /*
    public float lowPassStart;
    public float highPassStart;
    public float lowPassEcho;
    public float highPassEcho;
    public float distortionStart;
    public int autoUpdateV = 0;
    */
    public float amplitude;
    public float ppStep;

    private int defaultSnap;

    private AudioMixerSnapshot[] audioSnaps;

    /*
    public AudioManipulation(AudioSource pAudioSource) {
        audioSource = pAudioSource;
        lowPassStart = 10000f;
        lowPassEcho = 1f;
        highPassEcho = 1f;
        highPassStart = 2000f;
        distortionStart = 0f;
    }
    
    public AudioManipulation(AudioSource pAudioSource, float lp, float hp, float d, float lpE, float hpE) {
        audioSource = pAudioSource;
        lowPassStart = lp;
        highPassStart = hp;
        distortionStart = d;
        lowPassEcho = lpE;
        highPassEcho = hpE;
    }
    */
    public AudioManipulation(AudioMixer pMixer, AudioMixerSnapshot[] pAudioSnaps, AudioSource pAudioSource, int pDefaultSnap) {
        masterMix = pMixer;
        audioSource = pAudioSource;
        audioSnaps = pAudioSnaps;
        defaultSnap = pDefaultSnap;
    }

    public void initializeIt() {
        /*
        lowPass = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
        highPass = audioSource.gameObject.AddComponent<AudioHighPassFilter>();
        distortion = audioSource.gameObject.AddComponent<AudioDistortionFilter>();
        */
        initializeParams();
    }

    public void initializeParams() {
        audioSource.volume = startVolume;
        masterMix.TransitionToSnapshots(new AudioMixerSnapshot[] {audioSnaps[defaultSnap]}, new float[]{1f}, 0f);
        /*
        lowPass.cutoffFrequency = lowPassStart;
        highPass.cutoffFrequency = highPassStart;
        distortion.distortionLevel = distortionStart;
        highPass.highpassResonanceQ = highPassEcho;
        lowPass.lowpassResonanceQ = lowPassEcho;
        */
        //masterMix.TransitionToSnapshots()
    }

    public void startSound() {
        initializeParams();
        audioSource.Play();
    }

    public void changeParamPitch(string param, int sign, float value = 0f) {
        float currentValue;
        masterMix.GetFloat(param, out currentValue);
        float newValue = currentValue + (sign * value);

        if (newValue > 150f) {
            newValue = 150f;
        } else if (newValue < 50f) {
            newValue = 50f;
        }

        masterMix.SetFloat(param, newValue);
    }

    public void changeParamPass(string param, int sign, float value = 0f) {
        float currentValue;
        masterMix.GetFloat(param, out currentValue);
        Debug.Log(currentValue.ToString());
        if (value == 0f) {
            value = stepFunc(currentValue);
        }
       
        float newValue = currentValue + (sign*value);
        if(newValue > 22000f) {
            newValue = 22000f;
        }else if(newValue < 10f) {
            newValue = 10f;
        }
        masterMix.SetFloat(param, newValue);
    }

    public float stepFunc(float entry) {
        float step = (((entry * entry) / 100000) + minStep) * Time.deltaTime;
        return step;
    }

    public void highPassUp() {
        changeParamPass("highpassCutoff", 1);
        /*
        highPass.cutoffFrequency += stepFunc(highPass.cutoffFrequency);
        if (highPass.cutoffFrequency > 22000f) {
            highPass.cutoffFrequency = 22000f;
        }
        if(highPass.cutoffFrequency > lowPass.cutoffFrequency) {
            highPass.cutoffFrequency = lowPass.cutoffFrequency - ecartMin;
        }
        */
    }

    public void highPassDown() {
        changeParamPass("highpassCutoff", -1);
        /*
        highPass.cutoffFrequency -= stepFunc(highPass.cutoffFrequency);
        if (highPass.cutoffFrequency < 10f) {
            highPass.cutoffFrequency = 10f;
        }
        */
    }

    public void lowPassUp() {
        changeParamPass("lowpassCutoff", 1);
        /*
        lowPass.cutoffFrequency += stepFunc(lowPass.cutoffFrequency);
        if (lowPass.cutoffFrequency > 22000f) {
            lowPass.cutoffFrequency = 22000f;
        }
        */
    }

    public void lowPassDown() {
        changeParamPass("lowpassCutoff", -1);
        /*
        lowPass.cutoffFrequency -= stepFunc(lowPass.cutoffFrequency);
        if (lowPass.cutoffFrequency < 10f) {
            lowPass.cutoffFrequency = 10f + ecartMin;
        }
        if (lowPass.cutoffFrequency < highPass.cutoffFrequency) {
            lowPass.cutoffFrequency = highPass.cutoffFrequency + ecartMin;
        }
        */
    }

    public void passUp() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        float stepIt = stepFunc(currentValueH);
        if (currentValueL + stepIt > 22000f){
            changeParamPass("highpassCutoff", 1, Mathf.Max((22000f - currentValueL - ecartMin), 0f));
            changeParamPass("lowpassCutoff", 1, stepIt);
        } else {
            changeParamPass("highpassCutoff", 1, stepIt);
            changeParamPass("lowpassCutoff", 1, stepIt);
        }
    }

    public void passDown() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        float stepIt = stepFunc(currentValueH);
        if (currentValueH - stepIt < 10f) {
            changeParamPass("lowpassCutoff", -1, Mathf.Max(currentValueH - 10f - ecartMin, 0f));
            changeParamPass("highassCutoff", -1, stepIt);

        } else {
            changeParamPass("lowpassCutoff", -1, stepIt);
            changeParamPass("highassCutoff", -1, stepIt);
        }
    }

    public void passWidden() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        float stepItH = stepFunc(currentValueH);
        //float stepItL = stepFunc(currentValueL);
        if (currentValueH - stepItH < 10f){
            changeParamPass("lowpassCutoff", 1, stepItH*2f);
        } else if(currentValueL + stepItH > 22000f) {
            changeParamPass("highpassCutoff", -1, stepItH * 2f);
        } else {
            changeParamPass("lowpassCutoff", 1, stepItH);
            changeParamPass("highpassCutoff", -1, stepItH);
        }
    }

    public void passTighten() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        float stepItH = stepFunc(currentValueH);
        //float stepItL = stepFunc(currentValueL);

        if (currentValueL - stepItH - ecartMin < currentValueH + stepItH) {
            //donothing
        } else {
            changeParamPass("lowpassCutoff", -1, stepItH);
            changeParamPass("highpassCutoff", 1, stepItH);
        }
    }
    /*
    public void distortionUp() {
        if(distortion.distortionLevel <= 0.98f) { 
            distortion.distortionLevel += 0.2f * Time.deltaTime;
        }
    }

    public void distortionDown() {
        if (distortion.distortionLevel >= 0.1f) {
            distortion.distortionLevel -= 0.2f * Time.deltaTime;
        }
    }
    */
    public void pitchUp() {
        changeParamPitch("pitch", 1, 10f*Time.deltaTime);
    }

    public void pitchDown() {
        changeParamPitch("pitch", -1, 10f*Time.deltaTime);
    }


    public void tremble1() {
        amplitude = 0.2f;
        ppStep = 1000f;
        float pingpong = Mathf.PingPong(Time.time, amplitude);

        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);

        if (currentValueH + (pingpong - amplitude / 2f) * ppStep < 10f
            || currentValueL + (pingpong - amplitude / 2f) * ppStep > 22000f) {
        } else {
            changeParamPass("lowpassCutoff", 1, (pingpong - amplitude / 2f) * ppStep);
            changeParamPass("highpassCutoff", 1, (pingpong - amplitude / 2f) * ppStep);
        }
    }

    public void tremble2() {
        amplitude = 0.5f;
        ppStep = 500f;
        float pingpong2 = Mathf.PingPong(Time.time, amplitude);

        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);

        if (currentValueH + (pingpong2 - amplitude / 2f) * ppStep < 10f
            || currentValueL + (pingpong2 - amplitude / 2f) * ppStep > 22000f) {
        } else {
            changeParamPass("lowpassCutoff", 1, (pingpong2 - amplitude / 2f) * ppStep);
            changeParamPass("highpassCutoff", 1, (pingpong2 - amplitude / 2f) * ppStep);
        }
    }
}

public static class AudioFadeOut
{

    public static IEnumerator FadeOut(AudioManipulation AM, float FadeTime) {
        AudioSource audioSource = AM.audioSource;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        AM.inFadeOut = false;
    }

}

