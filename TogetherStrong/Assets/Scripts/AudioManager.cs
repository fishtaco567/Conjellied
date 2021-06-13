using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager> {

    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioSource qSource;

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Play(AudioClip clip) {
        source.PlayOneShot(clip);
    }

    public void PlayQuiet(AudioClip clip) {
        qSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update() {

    }
}
