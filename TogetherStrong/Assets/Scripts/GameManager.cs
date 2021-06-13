using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    public PlayerController player;

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update() {

    }
}
