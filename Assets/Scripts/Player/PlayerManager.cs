using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour {

    public Flock flock;
    private AudioSpectrum audioSpectrum;

    private int[] leadersArray;
    private int[] gridsArray;

    private int currentAudio = -1;


    public GameObject sounds;
    private AudioSource[] aSource;
    public AudioManipulation[] audioManip = new AudioManipulation[5];

    private int nbSounds = 2 ;

    public AudioMixer masterMix;

        struct BirdCommand
    {
        public int[] idLeaders;
        public int[] idGrids;
    };

    private Dictionary<char,BirdCommand> binding = new Dictionary<char, BirdCommand>();
    private BirdCommand[] BirdsCommands = new BirdCommand[100];


    void Awake() {
        
    }

    // Use this for initialization
    void Start () {
        leadersArray = flock.getLeadersArray();
        gridsArray = flock.getGridsArray();

        audioSpectrum = GetComponent<AudioSpectrum>();
        currentAudio = -1;
        aSource = sounds.GetComponentsInChildren<AudioSource>();
        /*foreach(var source in aSource) {
            Debug.Log(source.gameObject.name);
        }*/

        /*for (int i = 0; i < aSource.Length; i++) {
            audioManip[i] = new AudioManipulation(aSource[i], 22000f, 10f, 0f);
            audioManip[i].initializeIt();
        }*/
        /*attention on gère les inputs en prenant la longueur de ce tableau*/

        audioManip[0] = new AudioManipulation(masterMix, aSource[0], 22000f, 10f, 0f, 0f, 1f);
        audioManip[0].initializeIt();

        audioManip[2] = new AudioManipulation(masterMix, aSource[1], 6000f, 2100f, 5f, 5f, 1.5f);
        audioManip[2].initializeIt();

        audioManip[3] = new AudioManipulation(masterMix, aSource[2], 500f, 10f, 0f, 0f, 1f);
        audioManip[3].initializeIt();

        audioManip[1] = new AudioManipulation(masterMix, aSource[3], 4000f, 10f, 0f, 0f, 1f);
        audioManip[1].initializeIt();

        nbSounds = 4;
    }

    void stopSound(AudioManipulation AM) {
        IEnumerator fadeSound1 = AudioFadeOut.FadeOut(AM, 1f);
        audioManip[currentAudio].inFadeOut = true;
        audioManip[currentAudio].fadeSound = fadeSound1;
        StartCoroutine(fadeSound1);
    }

    void startSound(AudioManipulation AM) {
        if (AM.inFadeOut) {
            StopCoroutine(AM.fadeSound);
            AM.inFadeOut = false;
            /*AM.startVolume */
        }
        AM.startSound();
    }

    public float[] GetSpectrumData() {
        float[] rawSpectrum = new float[audioSpectrum.numberOfSamples];
        if (currentAudio != -1) { 
            audioManip[currentAudio].audioSource.GetSpectrumData(rawSpectrum, 0, FFTWindow.BlackmanHarris);
        }
        return rawSpectrum;
    }
    /*AudioListener.GetSpectrumData (rawSpectrum, 0, FFTWindow.BlackmanHarris);*/

    // Update is called once per frame
    void Update () {
        /*TODO change for Input action to allow for remapping*/
        /*
         
        */
        for(int ci = 0; ci<nbSounds ; ci++) {
            if (Input.GetButtonDown("sound"+(ci+1))) {
                if (currentAudio == ci) {
                    stopSound(audioManip[currentAudio]);
                    currentAudio = -1;
                } else if (currentAudio != -1) {
                    stopSound(audioManip[currentAudio]);
                    currentAudio = ci;
                    startSound(audioManip[currentAudio]);
                } else {
                    currentAudio = ci;
                    startSound(audioManip[currentAudio]);
                }
            }
        }

        if (currentAudio == -1) {
            return;
        }

        if (Input.GetButton("lowpassup")) {
            audioManip[currentAudio].lowPassUp();
        }

        if (Input.GetButton("lowpassdown")) {
            audioManip[currentAudio].lowPassDown();
        }

        if (Input.GetButton("highpassup")) {
            audioManip[currentAudio].highPassUp();
        }

        if (Input.GetButton("highpassdown")) {
            audioManip[currentAudio].highPassDown();
        }

        if (Input.GetButton("passup")) {
            audioManip[currentAudio].passUp();
        }

        if (Input.GetButton("passdown")) {
            audioManip[currentAudio].passDown();
        }

        if (Input.GetButton("passwidden")) {
            audioManip[currentAudio].passWidden();
        }

        if (Input.GetButton("passtighten")) {
            audioManip[currentAudio].passTighten();
        }

        if (Input.GetButton("tremblepass1")) {
            audioManip[currentAudio].tremble1();
        }

        if (Input.GetButton("tremblepass2")) {
            audioManip[currentAudio].tremble2();
        }

        if (Input.GetButton("pitchup")) {
            audioManip[currentAudio].pitchUp();
        }

        if (Input.GetButton("pitchdown")) {
            audioManip[currentAudio].pitchDown();
        }

        if (Input.GetButton("pitchtremble1")) {
            audioManip[currentAudio].trembleP1();
        }

        if (Input.GetButton("pitchtremble2")) {
            audioManip[currentAudio].trembleP2();
        }

        audioManip[currentAudio].updateVolume();

        checkSound();

        foreach (char c in Input.inputString) {
            //Debug.Log(c.ToString());
        }
    }

    void checkSound() {
        string levels = "";
        string peaks = "";
        string means = "";
        for (int i = 0; i < audioSpectrum.Levels.Length; i++) {
            levels += audioSpectrum.Levels[i] + ", ";
            peaks += audioSpectrum.PeakLevels[i] + ", ";
            means += audioSpectrum.MeanLevels[i] + ", ";
        }
        /*Debug.Log("levels : "+levels);
        Debug.Log("peaks : " + peaks);
        Debug.Log("means : " + means);*/

    }

    /*
    void sendBirdCommand(int[] idLeaders, int[] idGrids) {
        flock.transmissionListener(idLeaders, idGrids);
    }


    void sendBirdCommand(BirdCommand bCommand) {
        flock.transmissionListener(bCommand.idLeaders, bCommand.idGrids);
    }
    */
}
