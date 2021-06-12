using UnityEngine;

public class FollowThing : MonoBehaviour {

    public GameObject thing;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(thing.transform.position.x, thing.transform.position.y, 0) + new Vector3(0, 0, transform.position.z);
    }
}
