using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManipulation
{
    public AudioSource audioSource;
    public AudioLowPassFilter lowPass;
    public AudioHighPassFilter highPass;
    public AudioDistortionFilter distortion;

    public float minStep = 2000;

    public IEnumerator fadeSound;

    float ecartMin = 100f;

    public float startVolume = 1f;
    public bool inFadeOut = false;

    public float lowPassStart;
    public float highPassStart;
    public float distortionStart;
    public int autoUpdateV = 0;

    public float amplitude;
    public float ppStep;

    public AudioManipulation(AudioSource pAudioSource) {
        audioSource = pAudioSource;
        lowPassStart = 10000f;
        highPassStart = 2000f;
        distortionStart = 0f;
    }
    
    public AudioManipulation(AudioSource pAudioSource, float lp, float hp, float d, int auv) {
        audioSource = pAudioSource;
        lowPassStart = lp;
        highPassStart = hp;
        distortionStart = d;
        autoUpdateV = auv ;
    }

    public void initializeIt() {
        lowPass = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
        highPass = audioSource.gameObject.AddComponent<AudioHighPassFilter>();
        distortion = audioSource.gameObject.AddComponent<AudioDistortionFilter>();
        initializeParams();
    }

    public void initializeParams() {
        audioSource.volume = startVolume;
        lowPass.cutoffFrequency = lowPassStart;
        highPass.cutoffFrequency = highPassStart;
        distortion.distortionLevel = distortionStart;
    }

    public void startSound() {
        initializeParams();
        audioSource.Play();
    }

    /*
    public void stopSound() {
        
        //audioSource.Stop();
    }*/

    public float stepFunc(float entry) {
        float step = (((entry * entry) / 100000) + minStep) * Time.deltaTime;
        return step;
    }

    public void highPassUp() {
        highPass.cutoffFrequency += stepFunc(highPass.cutoffFrequency);
        if (highPass.cutoffFrequency > 22000f) {
            highPass.cutoffFrequency = 22000f;
        }
        if(highPass.cutoffFrequency > lowPass.cutoffFrequency) {
            highPass.cutoffFrequency = lowPass.cutoffFrequency - ecartMin;
        }
    }

    public void highPassDown() {
        highPass.cutoffFrequency -= stepFunc(highPass.cutoffFrequency);
        if (highPass.cutoffFrequency < 10f) {
            highPass.cutoffFrequency = 10f;
        }
        /*
        if (highPass.cutoffFrequency > lowPass.cutoffFrequency) {
            highPass.cutoffFrequency = lowPass.cutoffFrequency - ecartMin;
        }*/
    }

    public void lowPassUp() {
        lowPass.cutoffFrequency += stepFunc(lowPass.cutoffFrequency);
        if (lowPass.cutoffFrequency > 22000f) {
            lowPass.cutoffFrequency = 22000f;
        }
        /*
        if (lowPass.cutoffFrequency < highPass.cutoffFrequency) {
            lowPass.cutoffFrequency = highPass.cutoffFrequency + ecartMin;
        }*/
    }

    public void lowPassDown() {
        lowPass.cutoffFrequency -= stepFunc(lowPass.cutoffFrequency);
        if (lowPass.cutoffFrequency < 10f) {
            lowPass.cutoffFrequency = 10f;
        }
        if (lowPass.cutoffFrequency < highPass.cutoffFrequency) {
            lowPass.cutoffFrequency = highPass.cutoffFrequency + ecartMin;
        }
    }

    public void passUp() {
        float stepIt = stepFunc(highPass.cutoffFrequency);
        if (lowPass.cutoffFrequency + stepIt > 22000f){
            highPass.cutoffFrequency += (22000f - lowPass.cutoffFrequency - ecartMin);
            lowPass.cutoffFrequency = 22000f;
        } else { 
            highPass.cutoffFrequency += stepIt;
            lowPass.cutoffFrequency += stepIt;
        }
    }

    public void passDown() {
        float stepIt = stepFunc(highPass.cutoffFrequency);
        if (highPass.cutoffFrequency - stepIt < 10f) {
            lowPass.cutoffFrequency -= (lowPass.cutoffFrequency-10f-ecartMin);
            highPass.cutoffFrequency = 10f;
        } else {
            highPass.cutoffFrequency -= stepIt;
            lowPass.cutoffFrequency -= stepIt;
        }
    }

    public void passWidden() {
        if (highPass.cutoffFrequency - stepFunc(highPass.cutoffFrequency) < 10f){
            lowPass.cutoffFrequency += stepFunc(highPass.cutoffFrequency)*2f;
        } else if(lowPass.cutoffFrequency + stepFunc(lowPass.cutoffFrequency) > 22000f) {
            highPass.cutoffFrequency -= stepFunc(highPass.cutoffFrequency)*2f;
        } else {
            highPass.cutoffFrequency -= stepFunc(highPass.cutoffFrequency);
            lowPass.cutoffFrequency += stepFunc(lowPass.cutoffFrequency);
        }
    }

    public void passTighten() {
        if (lowPass.cutoffFrequency - stepFunc(highPass.cutoffFrequency)-ecartMin < highPass.cutoffFrequency + stepFunc(highPass.cutoffFrequency)) {
            //donothing
        } else { 
            highPass.cutoffFrequency += stepFunc(highPass.cutoffFrequency);
            lowPass.cutoffFrequency -= stepFunc(lowPass.cutoffFrequency);
        }
    }

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
    /*
    public void updateAutoChange() {
        switch (autoUpdateV) {
            case 1:
                amplitude = 0.2f;
                ppStep = 1000f;
                float pingpong = Mathf.PingPong(Time.time, amplitude);
                if (highPass.cutoffFrequency + (pingpong - amplitude / 2f) * ppStep < 10f
                    || lowPass.cutoffFrequency + (pingpong - amplitude / 2f) * ppStep > 22000f) {
                } else {
                    highPass.cutoffFrequency += (pingpong - amplitude / 2f) * ppStep;
                    lowPass.cutoffFrequency += (pingpong - amplitude / 2f) * ppStep;
                }
             break;

            case 2:
                amplitude = 2f;
                ppStep = 500;
                float pingpong2 = Mathf.PingPong(Time.time, amplitude);
                if (highPass.cutoffFrequency + (pingpong2 - amplitude / 2f) * ppStep < 10f
                    || lowPass.cutoffFrequency + (pingpong2 - amplitude / 2f) * ppStep > 22000f) {
                } else {
                    highPass.cutoffFrequency += (pingpong2 - amplitude / 2f) * ppStep;
                    lowPass.cutoffFrequency += (pingpong2 - amplitude / 2f) * ppStep;
                }
                break;
        }
     
    }*/
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

