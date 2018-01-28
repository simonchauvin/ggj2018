﻿using System.Collections;
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
    private float freqMax = 15000f;
    private float freqMin = 10f;

    public float startVolume = 1f;
    public bool inFadeOut = false;


    public float lowPassStart;
    public float highPassStart;
    public float lowPassResonance;
    public float highPassResonance;
    public float pitch;

    public float amplitude;
    public float ppStep;

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
    public AudioManipulation(AudioMixer pMixer, AudioSource pAudioSource, float lp, float hp, float lpE, float hpE, float p) {
        masterMix = pMixer;
        audioSource = pAudioSource;
        lowPassStart = lp;
        highPassStart = hp;
        highPassResonance = hpE;
        lowPassResonance = lpE;
        pitch = p;
    }

    public void initializeIt() {
        /*
        lowPass = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
        highPass = audioSource.gameObject.AddComponent<AudioHighPassFilter>();
        distortion = audioSource.gameObject.AddComponent<AudioDistortionFilter>();
        */
        /*
        masterMix.GetFloat("lowpassCutoff", out lowPassStart);
        masterMix.GetFloat("highpassCutoff", out highPassStart);
        masterMix.GetFloat("highpassResonance", out highPassResonance);
        masterMix.GetFloat("lowpassResonance", out lowPassResonance);
        masterMix.GetFloat("pitch", out pitch);
        */
        initializeParams();
    }

    public void initializeParams() {
        audioSource.volume = startVolume;
        masterMix.SetFloat("lowpassCutoff", lowPassStart);
        masterMix.SetFloat("highpassCutoff", highPassStart);
        masterMix.SetFloat("highpassResonance", highPassResonance);
        masterMix.SetFloat("lowpassResonance", lowPassResonance);
        masterMix.SetFloat("pitch", pitch);
    }

    public void startSound() {
        initializeParams();
        audioSource.Play();
    }

    public void changeParamPitch(string param, int sign, float value = 0f) {
        float currentValue;
        masterMix.GetFloat(param, out currentValue);
        float newValue = currentValue + (sign * value);

        if (newValue > 1.50f) {
            newValue = 1.50f;
        } else if (newValue < 0.50f) {
            newValue = 0.50f;
        }

        masterMix.SetFloat(param, newValue);
    }

    public void changeParamPass(string param, int sign, float value) {
        float currentValue;
        masterMix.GetFloat(param, out currentValue);

        float newValue = currentValue + (sign * value);
        if (newValue > freqMax) {
            newValue = freqMax;
        } else if (newValue < freqMin) {
            newValue = freqMin;
        }
        masterMix.SetFloat(param, newValue);
    }

    public void changeParamPass(string param, int sign) {
        float currentValue;
        masterMix.GetFloat(param, out currentValue);
        float value = stepFunc(currentValue);
        changeParamPass(param, sign, value);
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
        if (currentValueL + stepIt >= freqMax) {
            changeParamPass("highpassCutoff", 1, Mathf.Max((freqMax - currentValueL - ecartMin), 0f));
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
        if (currentValueH - stepIt <= freqMin) {
            changeParamPass("lowpassCutoff", -1, Mathf.Max(currentValueH - freqMin - ecartMin, 0f));
            changeParamPass("highpassCutoff", -1, stepIt);

        } else {
            changeParamPass("lowpassCutoff", -1, stepIt);
            changeParamPass("highpassCutoff", -1, stepIt);
        }
    }

    public void passWidden() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        float stepItH = stepFunc(currentValueH);
        //float stepItL = stepFunc(currentValueL);
        if (currentValueH - stepItH < freqMin) {
            changeParamPass("lowpassCutoff", 1, stepItH * 2f);
        } else if (currentValueL + stepItH > freqMax) {
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
        changeParamPitch("pitch", 1, 0.07f * Time.deltaTime);
    }

    public void pitchDown() {
        changeParamPitch("pitch", -1, 0.07f * Time.deltaTime);
    }


    public void tremble1() {
        amplitude = 0.2f;
        ppStep = 1000f;
        float pingpong = Mathf.PingPong(Time.time, amplitude);

        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);

        if (currentValueH + (pingpong - amplitude / 2f) * ppStep < freqMin
            || currentValueL + (pingpong - amplitude / 2f) * ppStep > freqMax) {
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

        if (currentValueH + (pingpong2 - amplitude / 2f) * ppStep < freqMin
            || currentValueL + (pingpong2 - amplitude / 2f) * ppStep > freqMax) {
        } else {
            changeParamPass("lowpassCutoff", 1, (pingpong2 - amplitude / 2f) * ppStep);
            changeParamPass("highpassCutoff", 1, (pingpong2 - amplitude / 2f) * ppStep);
        }
    }

    public void trembleP1() {
        amplitude = 0.5f;
        ppStep = 0.02f;
        float pingpong3 = Mathf.PingPong(Time.time, amplitude);
        float currentPitch;
        masterMix.GetFloat("pitch", out currentPitch);

        Debug.Log("pingpong " + pingpong3);
        Debug.Log("currentPitch " + currentPitch);
        Debug.Log("value " + (pingpong3 - (amplitude / 2f)) * ppStep);
        
        if ((currentPitch + (pingpong3 - (amplitude / 2f)) * ppStep < 0.5f)
            || currentPitch + (pingpong3 - (amplitude / 2f)) * ppStep > 1.5f) {
        } else {
            changeParamPitch("pitch", 1, (pingpong3 - (amplitude / 2f)) * ppStep);
        }
    }

    public void trembleP2() {
        amplitude = 1f;
        ppStep = 0.5f;
        float pingpong4 = Mathf.PingPong(Time.time, amplitude);
        float currentPitch;
        masterMix.GetFloat("pitch", out currentPitch);

        Debug.Log("pingpong " + pingpong4);
        Debug.Log("currentPitch " + currentPitch);
        Debug.Log("value " + (pingpong4 - (amplitude / 2f)) * ppStep);

        if ((currentPitch + (pingpong4 - (amplitude / 2f)) * ppStep < 0.5f)
            || currentPitch + (pingpong4 - (amplitude / 2f)) * ppStep > 1.5f) {
        } else {
            changeParamPitch("pitch", 1, (pingpong4 - (amplitude / 2f)) * ppStep);
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

