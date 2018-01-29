using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour {
    public SnapshotManager snapshotManager;
    public void LoadMainScene()
    {
        snapshotManager.loadMainSnapshot();
        SceneManager.LoadScene("NaK3", LoadSceneMode.Single);
        
    }
}
