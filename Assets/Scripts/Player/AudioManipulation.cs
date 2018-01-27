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

    float ecartMin = 100f;

    public float lowPassStart;
    public float highPassStart;
    public float distortionStart;

    public AudioManipulation(AudioSource pAudioSource) {
        audioSource = pAudioSource;
        lowPassStart = 500f;
        highPassStart = 10;
        distortionStart = 0f;
    }

    public void initializeIt() {
        lowPass = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = lowPassStart;
        highPass = audioSource.gameObject.AddComponent<AudioHighPassFilter>();
        highPass.cutoffFrequency = highPassStart;
        distortion = audioSource.gameObject.AddComponent<AudioDistortionFilter>();
        distortion.distortionLevel = distortionStart;
    }

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

    public void updateAutoChange() {
        /* doing or not */
        /*Mathf.PingPong(Time.time, 3);*/
    }
}
