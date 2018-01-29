using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SnapshotManager : MonoBehaviour {

    public AudioMixer Mixer;
    private AudioMixerSnapshot transitionSnapshot;
    private AudioMixerSnapshot mainSnapshot;


    public void Start()
    {
        mainSnapshot = Mixer.FindSnapshot("MainMix");
        transitionSnapshot = Mixer.FindSnapshot("TransitionMix");
    }

    public void loadMainSnapshot()
    {
        transitionSnapshot.TransitionTo(0.5f);
        StartCoroutine(WaitAndLoadSnapshot(mainSnapshot, 2.0f, 1.0f));
    }

    private IEnumerator WaitAndLoadSnapshot(AudioMixerSnapshot shot,float transitionTime,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shot.TransitionTo(transitionTime);
    }
}
