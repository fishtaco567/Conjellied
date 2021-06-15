using UnityEngine;
using System.Collections;

public class UIController : Singleton<UIController> {

    [SerializeField]
    protected GameObject gameOver;

    [SerializeField]
    protected GameObject paused;

    [SerializeField]
    protected GameObject mainMenu;

    [SerializeField]
    protected GameObject controls;
    [SerializeField]
    protected CameraController cam;

    [SerializeField]
    protected GameObject greentb;
    [SerializeField]
    protected TMPro.TMP_Text greenText;

    [SerializeField]
    protected GameObject redtb;
    [SerializeField]
    protected TMPro.TMP_Text redText;

    [SerializeField]
    protected GameObject qtb;
    [SerializeField]
    protected TMPro.TMP_Text qText;

    protected bool inMainMenu;

    protected bool isDialogue;

    // Use this for initialization
    void Start() {
        Time.timeScale = 0;
        inMainMenu = true;
        isDialogue = false;
    }

    // Update is called once per frame
    void Update() {
        var replayer = Rewired.ReInput.players.GetPlayer(0);
        if(replayer.GetButtonDown("Menu") && mainMenu.activeSelf == false && !isDialogue && !inMainMenu) {
            PausedOn(!paused.activeSelf);
        }

        if((replayer.GetButtonDown("Menu") || replayer.GetButtonDown("Shoot")) && isDialogue) {
            greentb.SetActive(false);
            redtb.SetActive(false);
            qtb.SetActive(false);
            isDialogue = false;
            Time.timeScale = 1;
        }
    }

    protected GameObject fromControls;

    public void GameOverOn(bool on) {
        gameOver.SetActive(on);
    }

    public void PausedOn(bool on) {
        slides.SetActive(on);
        Time.timeScale = on ? 0 : 1;
        paused.SetActive(on);
    }

    public void MainMenuOn(bool on) {
        slides.SetActive(on);
        Time.timeScale = on ? 0 : 1;
        mainMenu.SetActive(on);
    }

    public void ControlsOn(bool on) {
        controls.SetActive(on);
    }

    public void PlayPressed() {
        inMainMenu = false;
        MainMenuOn(false);
        cam.inMainMenu = false;
    }

    public void ControlsPressed() {
        mainMenu.SetActive(false);
        fromControls = mainMenu;
        ControlsOn(true);
    }

    public void ControlsBackPressed() {
        fromControls.SetActive(true);
        ControlsOn(false);
    }

    public void ResumePressed() {
        PausedOn(false);
    }

    public void ExitPressed() {
        paused.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        inMainMenu = true;
    }

    public void ControlsPressedIg() {
        ControlsOn(true);
        fromControls = paused;
        paused.SetActive(false);
    }

    public void ShowTextbox(string text, int type) {
        Time.timeScale = 0;
        isDialogue = true;
        switch(type) {
            case 0:
                greentb.SetActive(true);
                greenText.text = text;
                break;
            case 1:
                redtb.SetActive(true);
                redText.text = text;
                break;
            case 2:
                qtb.SetActive(true);
                qText.text = text;
                break;
        }
    }

    [SerializeField]
    protected UnityEngine.UI.Slider volume;
    [SerializeField]
    protected UnityEngine.UI.Slider sfx;

    [SerializeField]
    protected AudioSource music;

    [SerializeField]
    protected AudioSource sfx1;
    [SerializeField]
    protected AudioSource sfx2;

    [SerializeField]
    protected GameObject slides;

    public void MusicS(float s) {
        music.volume = volume.value;
    }

    public void SfxS(float s) {
        sfx1.volume = sfx.value;
        sfx2.volume = sfx.value * .25f;
    }

}
