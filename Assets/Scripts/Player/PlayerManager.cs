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

    public AudioSource[] aSource = new AudioSource[5];
    public AudioManipulation[] audioManip = new AudioManipulation[5];

    public AudioMixer masterMix;
    //int[] codeKeys = new int[37];

        struct BirdCommand
    {
        public int[] idLeaders;
        public int[] idGrids;
    };

    private Dictionary<char,BirdCommand> binding = new Dictionary<char, BirdCommand>();
    private BirdCommand[] BirdsCommands = new BirdCommand[100];

    // Use this for initialization
    void Start () {
        leadersArray = flock.getLeadersArray();
        gridsArray = flock.getGridsArray();

        currentAudio = -1;

        /*for (int i = 0; i < aSource.Length; i++) {
            audioManip[i] = new AudioManipulation(aSource[i], 22000f, 10f, 0f);
            audioManip[i].initializeIt();
        }*/

        audioManip[0] = new AudioManipulation(masterMix, aSource[0], 22000f, 10f, 0f, 0f,1f);
        audioManip[0].initializeIt();

        audioManip[1] = new AudioManipulation(masterMix, aSource[1], 6000f, 1200f, 5f, 5f, 1.5f);
        audioManip[1].initializeIt();

        //aSource[0].GetComponent<AudioLowPassFilter>().enabled = true;
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

    // Update is called once per frame
    void Update () {
        /*TODO change for Input action to allow for remapping*/

        if (Input.GetKeyDown("c")) {
            if (currentAudio != -1) {
                stopSound(audioManip[currentAudio]);
            }
            currentAudio = 0 ;
            startSound(audioManip[currentAudio]);
        }

        if (Input.GetKeyDown("v")) {
            if(currentAudio != -1) {
                stopSound(audioManip[currentAudio]);
            }
            currentAudio = 1;
            startSound(audioManip[currentAudio]);
        }

        if(currentAudio == -1) {
            return;
        }

        if (Input.GetKey("a")) {
            audioManip[currentAudio].lowPassUp();
        }

        if (Input.GetKey("q")) {
            audioManip[currentAudio].lowPassDown();
        }

        if (Input.GetKey("z")) {
            audioManip[currentAudio].highPassUp();
        }

        if (Input.GetKey("s")) {
            audioManip[currentAudio].highPassDown();
        }

        if (Input.GetKey("e")) {
            audioManip[currentAudio].passUp();
        }

        if (Input.GetKey("d")) {
            audioManip[currentAudio].passDown();
        }

        if (Input.GetKey("r")) {
            audioManip[currentAudio].passWidden();
        }

        if (Input.GetKey("f")) {
            audioManip[currentAudio].passTighten();
        }

        if (Input.GetKey("t")) {
            //audioManip[currentAudio].distortionUp();
            audioManip[currentAudio].tremble1();
        }

        if (Input.GetKey("g")) {
            //audioManip[currentAudio].distortionDown();
            audioManip[currentAudio].tremble2();
        }

        if (Input.GetKey("y")) {
            //audioManip[currentAudio].distortionUp();
            audioManip[currentAudio].pitchUp();
        }

        if (Input.GetKey("h")) {
            //audioManip[currentAudio].distortionDown();
            audioManip[currentAudio].pitchDown();
        }

        //audioManip[currentAudio].updateAutoChange();

        foreach (char c in Input.inputString) {
            //Debug.Log(c.ToString());
        }
    }

    void sendBirdCommand(int[] idLeaders, int[] idGrids) {
        flock.transmissionListener(idLeaders, idGrids);
    }


    void sendBirdCommand(BirdCommand bCommand) {
        flock.transmissionListener(bCommand.idLeaders, bCommand.idGrids);
    }
}
