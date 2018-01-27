using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public Flock flock;
    private int[] leadersArray;
    private int[] gridsArray;

    private int currentAudio = -1;

    public AudioSource[] aSource = new AudioSource[5];
    public AudioManipulation[] audioManip = new AudioManipulation[5];
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

        audioManip[0] = new AudioManipulation(aSource[0]);
        audioManip[0].initializeIt();

        currentAudio = 0;
        //aSource[0].GetComponent<AudioLowPassFilter>().enabled = true;
    }

    // Update is called once per frame
    void Update () {
        /*TODO change for Input action to allow for remapping*/
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
            audioManip[currentAudio].distortionUp();
        }

        if (Input.GetKey("g")) {
            audioManip[currentAudio].distortionDown();
        }

        audioManip[currentAudio].updateAutoChange();

        foreach (char c in Input.inputString) {
            Debug.Log(c.ToString());
        }
    }

    void sendBirdCommand(int[] idLeaders, int[] idGrids) {
        flock.transmissionListener(idLeaders, idGrids);
    }


    void sendBirdCommand(BirdCommand bCommand) {
        flock.transmissionListener(bCommand.idLeaders, bCommand.idGrids);
    }
}
