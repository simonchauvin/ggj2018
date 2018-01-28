using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManipulation
{
    private AudioMixer masterMix;
    public AudioSource audioSource;

    public float minStep = 2000;

    public IEnumerator fadeSound;

    float ecartMin = 500f;
    private float freqMax = 8000f;
    private float freqMin = 10f;
    private float pitchMin = 0f;
    private float pitchMax = 2.5f;
    private float minPass = 6000f; /*le max en fait*/

    public float startVolume = 1f;
    public bool inFadeOut = false;


    public float lowPassStart;
    public float highPassStart;
    public float lowPassResonance;
    public float highPassResonance;
    public float pitch;

    public float amplitude;
    public float ppStep;

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

        if (newValue > pitchMax) {
            newValue = pitchMax;
        } else if (newValue < pitchMin) {
            newValue = pitchMin;
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
    }

    public void highPassDown() {
        changeParamPass("highpassCutoff", -1);
    }

    public void lowPassUp() {
        changeParamPass("lowpassCutoff", 1);
    }

    public void lowPassDown() {
        changeParamPass("lowpassCutoff", -1);
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
        if ((currentValueL + stepItH) - (currentValueH - stepItH) >= minPass) {
        } else if (currentValueH - stepItH < freqMin) {
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

        if (currentValueL - stepItH - ecartMin < currentValueH + stepItH) {
            //donothing
        } else {
            changeParamPass("lowpassCutoff", -1, stepItH);
            changeParamPass("highpassCutoff", 1, stepItH);
        }
    }

    public void pitchUp() {
        changeParamPitch("pitch", 1, 0.07f * Time.deltaTime);
    }

    public void pitchDown() {
        changeParamPitch("pitch", -1, 0.07f * Time.deltaTime);
    }


    public void tremble1() {
        amplitude = 0.5f;
        ppStep = 500f;
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
        amplitude = 1f;
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

        if ((currentPitch + (pingpong3 - (amplitude / 2f)) * ppStep < pitchMin)
            || currentPitch + (pingpong3 - (amplitude / 2f)) * ppStep > pitchMax) {
        } else {
            changeParamPitch("pitch", 1, (pingpong3 - (amplitude / 2f)) * ppStep);
        }
    }

    public void trembleP2() {
        amplitude = 0.8f;
        ppStep = 0.20f;
        float pingpong4 = Mathf.PingPong(Time.time, amplitude);
        float currentPitch;
        masterMix.GetFloat("pitch", out currentPitch);

        if ((currentPitch + (pingpong4 - (amplitude / 2f)) * ppStep < pitchMin)
            || currentPitch + (pingpong4 - (amplitude / 2f)) * ppStep > pitchMax) {
        } else {
            changeParamPitch("pitch", 1, (pingpong4 - (amplitude / 2f)) * ppStep);
        }
    }

    public void updateVolume() {
        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);
        if (currentValueH <= 50f && currentValueL >= 16000f) {
            masterMix.SetFloat("volume", 0f);
            return;
        }
        float total = currentValueH + currentValueL;
        float currentVolume;
        masterMix.GetFloat("volume", out currentVolume);
        //Debug.Log("currentVol " + currentVolume);
        float t = Mathf.InverseLerp(freqMin, freqMax*2f, total) ;
        float newVolume = Mathf.Lerp(-10, 20, t);

        masterMix.SetFloat("volume", Mathf.Min(Mathf.Max(newVolume,-5f),10f));
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
