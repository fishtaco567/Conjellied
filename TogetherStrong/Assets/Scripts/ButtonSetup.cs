using UnityEngine;
using System.Collections;

public class ButtonSetup : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 1f;
    }

    // Update is called once per frame
    void Update() {

    }
}
