using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private int nbSounds = 2;

    public AudioMixer masterMix;

    struct BirdCommand {
        public int[] idLeaders;
        public int[] idGrids;
    };

    private Dictionary<char, BirdCommand> binding = new Dictionary<char, BirdCommand>();
    private BirdCommand[] BirdsCommands = new BirdCommand[100];


    void Awake() {

    }

    // Use this for initialization
    void Start() {
        leadersArray = flock.getLeadersArray();
        gridsArray = flock.getGridsArray();

        InvokeRepeating("spawnSoundLength", 5f, 1f);

        audioSpectrum = GetComponent<AudioSpectrum>();
        initializeCheckSound();
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

        audioManip[1] = new AudioManipulation(masterMix, aSource[3], 22000f, 10f, 0f, 0f, 1f);
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
    void Update() {
        /*TODO change for Input action to allow for remapping*/
        /*
         
        */
        if (Input.GetButton("Jump")) {
            if (currentAudio != -1) {
                stopSound(audioManip[currentAudio]);
                currentAudio = -1;
                spawnGrid(NavFieldPrimitives.dispersal,"jump");
            }
        }

        for (int ci = 0; ci < nbSounds; ci++) {
            if (Input.GetButtonDown("sound" + (ci + 1))) {
                if (currentAudio == ci) {
                    //stopSound(audioManip[currentAudio]);
                    //currentAudio = -1;
                } else if (currentAudio != -1) {
                    stopSound(audioManip[currentAudio]);
                    currentAudio = ci;
                    startSound(audioManip[currentAudio]);
                } else {
                    currentAudio = ci;
                    startSound(audioManip[currentAudio]);
                    spawnGrid(NavFieldPrimitives.tube, "sound" + (ci + 1));
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
            spawnGrid(NavFieldPrimitives.dispersal, "passwidden");
        }

        if (Input.GetButton("passtighten")) {
            audioManip[currentAudio].passTighten();
            spawnGrid(NavFieldPrimitives.gathering, "passtighten");
        }

        if (Input.GetButton("tremblepass1")) {
            audioManip[currentAudio].tremble1();
            spawnGrid(NavFieldPrimitives.horizontal_compressor, "tremblepass1");
        }

        if (Input.GetButton("tremblepass2")) {
            audioManip[currentAudio].tremble2();
            spawnGrid(NavFieldPrimitives.vertical_compressor, "tremblepass2");
        }

        if (Input.GetButton("pitchup")) {
            audioManip[currentAudio].pitchUp();
            spawnGrid(NavFieldPrimitives.ascension, "pitchup");
        }

        if (Input.GetButton("pitchdown")) {
            audioManip[currentAudio].pitchDown();
            spawnGrid(NavFieldPrimitives.descent, "pitchdown");
        }

        if (Input.GetButton("pitchtremble1")) {
            audioManip[currentAudio].trembleP1();
            spawnGrid(NavFieldPrimitives.dispersal, "pitchtremble1");
            spawnGrid(NavFieldPrimitives.tube, "pitchtremble1");
        }

        if (Input.GetButton("pitchtremble2")) {
            audioManip[currentAudio].trembleP2();
            spawnGrid(NavFieldPrimitives.ascension, "pitchtremble1");
            spawnGrid(NavFieldPrimitives.tube, "pitchtremble1");
        }

        audioManip[currentAudio].updateVolume();

        //checkSound();

        foreach (char c in Input.inputString) {
            //spawnSoundGrid(c);
            //Debug.Log(c.ToString());
        }
    }

    void spawnSoundLength() {
        if (currentAudio == -1) return;

        float currentValueH;
        float currentValueL;
        masterMix.GetFloat("highpassCutoff", out currentValueH);
        masterMix.GetFloat("lowpassCutoff", out currentValueL);

        /*if (currentValueH + currentValueL < 5000f) {
            spawnGrid(3, "currentValueH + currentValueL < 5000f");
        }
        if (currentValueL - currentValueH < 1200f) {
            spawnGrid(2, "currentValueL - currentValueH < 1200f");
        }
        if (currentValueH + currentValueL > 6000f) {
            spawnGrid(4, "currentValueH + currentValueL > 6000f");
        }*/ 
    }

    float lastInputGrid = 0f;

    void spawnSoundGrid(int inputType,string from) {
        if (currentAudio == -1) return;

        if (lastInputGrid <= Time.time) { 
            if (inputType == 1) {
                spawnGrid(NavFieldPrimitives.ascension, from);
            } else if (inputType == 2) {
                spawnGrid(NavFieldPrimitives.gathering, from);
            } else {
                spawnGrid(NavFieldPrimitives.descent, from);
            }
            lastInputGrid = Time.time + 1.0f;
        }
    }

    float lastSpawnGrid = 0f;
    NavFieldPrimitives lastSpawnVersion = NavFieldPrimitives.gathering;

    void spawnGrid(NavFieldPrimitives version,string from) {
        if (currentAudio == -1) return;
        if (lastSpawnGrid > Time.time && version == lastSpawnVersion) return;

        lastSpawnVersion = version;
        switch (version) {
            case NavFieldPrimitives.dispersal:
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.dispersal, 1.0f);
                lastSpawnGrid = Time.time + 1.0f;
                Debug.Log("dispersal" + " from " + from);
                break;
            case NavFieldPrimitives.gathering:
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.gathering, 1.0f);
                Debug.Log("gathering" + " from " + from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
            case NavFieldPrimitives.descent: /*descendre*/
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.descent, 1.0f);
                Debug.Log("descendre" + " from " + from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
            case NavFieldPrimitives.ascension: /*monter*/
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.ascension, 1.0f);
                Debug.Log("monter "+" from "+ from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
            case NavFieldPrimitives.tube: /*tube*/
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.tube, 1.0f);
                Debug.Log("tube " + " from " + from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
            case NavFieldPrimitives.horizontal_compressor: /*horizontal_compressor*/
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.horizontal_compressor, 1.0f);
                Debug.Log("horizontal_compressor " + " from " + from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
            case NavFieldPrimitives.vertical_compressor: /*vertical_compressor*/
                flock.GetComponent<NavfieldManager>().addNavfield(flock, flock.getLeader().transform.rotation, NavFieldPrimitives.vertical_compressor, 1.0f);
                Debug.Log("vertical_compressor " + " from " + from);
                lastSpawnGrid = Time.time + 1.0f;
                break;
        }

    }

    private float rmsValue;
    private float dbValue;
    private float pitchValue;
    private float refValue = 0.1f;
    private float threshold = 0.01f;


    private List<Peak> peaks = new List<Peak>();
    int samplerate;

    public Text txtdisplay; // drag a Text object here to display values

    void initializeCheckSound() {
        samplerate = AudioSettings.outputSampleRate;
    }

    private float previousPitch;
    private float cumulatePitchVelocity;

    void checkSound() {
        string levels = "";
        string peaks = "";
        string means = "";
        for (int i = 0; i < audioSpectrum.Levels.Length; i++) {
            levels += audioSpectrum.Levels[i] + ", ";
            peaks += audioSpectrum.PeakLevels[i] + ", ";
            means += audioSpectrum.MeanLevels[i] + ", ";
        }
        float[] spectrum = new float[1024];
        float[] samples = new float[1024];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        AudioListener.GetOutputData(samples, 0);
        AnalyzeSound(samples, spectrum);
        if (txtdisplay != null) {
            txtdisplay.text = "RMS: " + rmsValue.ToString("F2") +
                " (" + dbValue.ToString("F1") + " dB)\n" +
                "Pitch: " + pitchValue.ToString("F0") + " Hz";
        }
        /*Debug.Log("RMS: " + rmsValue.ToString("F2") +
                " (" + dbValue.ToString("F1") + " dB)\n" +
                "Pitch: " + pitchValue.ToString("F0") + " Hz");*/

        float velocityPitch = (pitchValue - previousPitch) / Time.deltaTime;
        cumulatePitchVelocity = Mathf.Max(cumulatePitchVelocity, cumulatePitchVelocity-(cumulatePitchVelocity / 200f)*Time.deltaTime);
        cumulatePitchVelocity += velocityPitch*Time.deltaTime;
        previousPitch = pitchValue ;
        //Debug.Log(cumulatePitchVelocity);

        /*Debug.Log("levels : "+levels);
        Debug.Log("peaks : " + peaks);
        Debug.Log("means : " + means);*/
    }

    void AnalyzeSound(float[] samples, float[] spectrum) {
        int qSamples = samples.Length;

        /*sound data*/
        int i = 0;
        float sum = 0f;
        for (i = 0; i < qSamples; i++) {
            sum += samples[i] * samples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min

        /*sound spectrum*/
        float maxV = 0f;
        for (i = 0; i < qSamples; i++) { // find max
            if (spectrum[i] > maxV && spectrum[i] > threshold) {
                peaks.Add(new Peak(spectrum[i], i));
                if (peaks.Count > 5) { // get the 5 peaks in the sample with the highest amplitudes
                    peaks.Sort(new AmpComparer()); // sort peak amplitudes from highest to lowest
                                                    //peaks.Remove (peaks [5]); // remove peak with the lowest amplitude
                }
            }
        }
        float freqN = 0f;
        if (peaks.Count > 0) {
            //peaks.Sort (new IndexComparer ()); // sort indices in ascending order
            maxV = peaks[0].amplitude;
            int maxN = peaks[0].index;
            freqN = maxN; // pass the index to a float variable
            if (maxN > 0 && maxN < qSamples - 1) { // interpolate index using neighbours
                var dL = spectrum[maxN - 1] / spectrum[maxN];
                var dR = spectrum[maxN + 1] / spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
        }
        pitchValue = freqN * (samplerate / 2f) / qSamples; // convert index to frequency
        peaks.Clear();
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

class Peak
{
    public float amplitude;
    public int index;

    public Peak() {
        amplitude = 0f;
        index = -1;
    }

    public Peak(float _frequency, int _index) {
        amplitude = _frequency;
        index = _index;
    }
}

class AmpComparer : IComparer<Peak>
{
    public int Compare(Peak a, Peak b) {
        return 0 - a.amplitude.CompareTo(b.amplitude);
    }
}

class IndexComparer : IComparer<Peak>
{
    public int Compare(Peak a, Peak b) {
        return a.index.CompareTo(b.index);
    }
}
