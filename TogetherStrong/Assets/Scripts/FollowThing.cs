using UnityEngine;

public class FollowThing : MonoBehaviour {

    [SerializeField]
    protected GameObject thing;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.position = thing.transform.position + new Vector3(0, 0, transform.position.z - 0.1f);
    }
}
