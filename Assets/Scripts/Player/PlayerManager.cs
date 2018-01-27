using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public Flock flock;
    private int[] leadersArray;
    private int[] gridsArray;
    private int version = 1;

    int[] codeKeys = new int[37];

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
        createBinding();
    }
	
    void createBinding() {
        switch (version) {
            case 1:
                codeKeysCreationVersion1();
                commandCreationVersion1();
                createBindingVersion1();
            break;
        }
    }

    void codeKeysCreationVersion1() {
        int c = 0;
        /*min letter first*/
        for(int i = 97; i <= 122; i++) {
            codeKeys[c] = i;
            c++;
        }

        /*number then*/
        for (int i = 48; i <= 57; i++) {
            codeKeys[c] = i;
            c++;
        }
    }

    void commandCreationVersion1() {
        int c = 0;
        foreach (var leader in leadersArray) {
            foreach (var grid in gridsArray) {
                BirdsCommands[c].idLeaders = new int[] { leader };
                BirdsCommands[c].idGrids = new int[] { grid };
                c++;
            }
        }

        foreach (var leader in leadersArray) {
            foreach (var leader2 in leadersArray) {
                if (leader != leader2) {
                    foreach (var grid in gridsArray) {
                        BirdsCommands[c].idLeaders = new int[] { leader, leader2 };
                        BirdsCommands[c].idGrids = new int[] { grid };
                        c++;
                    }
                }
            }
        }
    }

    void createBindingVersion1() {
        int i = 0;
        foreach(BirdCommand bc in BirdsCommands) {
            binding.Add((char)codeKeys[i], bc);
            i++;
            if (i >= codeKeys.Length) {
                break;
            }
        }
        //binding.Add('a', BirdsCommands[0]);
    }

    // Update is called once per frame
    void Update () {
        foreach (char c in Input.inputString) {
            if (binding.ContainsKey(c)) {
                Debug.Log(c.ToString());
                sendBirdCommand(binding[c]);
            }
        }
        /*
            if (Input.GetKeyDown("a")) {
            Debug.Log("a");
            sendBirdCommand(binding["a"]);
        }
            //Instantiate(projectile, transform.position, transform.rotation);

        if (Input.GetKeyDown("z")) {
            Debug.Log("up");
        }

        if (Input.GetKeyDown("e")) {
            Debug.Log("down");
        }
        //Instantiate(projectile, transform.position, transform.rotation);
        */
    }

    void sendBirdCommand(int[] idLeaders, int[] idGrids) {
        flock.transmissionListener(idLeaders, idGrids);
    }


    void sendBirdCommand(BirdCommand bCommand) {
        flock.transmissionListener(bCommand.idLeaders, bCommand.idGrids);
    }
}
