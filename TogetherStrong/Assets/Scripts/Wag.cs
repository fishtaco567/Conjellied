using UnityEngine;
using System.Collections;

public class Wag : MonoBehaviour {

    [SerializeField]
    protected float rotAmount;

    [SerializeField]
    protected float rotSpeed;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.eulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * rotSpeed) * rotAmount);
    }
}
