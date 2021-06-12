using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

    [SerializeField]
    protected bool pressed;

    [SerializeField]
    protected GameObject door;

    protected Vector3 doorBasePos;

    [SerializeField]
    protected float doorMoveDistance;

    [SerializeField]
    protected Collider2D col;

    [SerializeField]
    protected float doorMoveSpeed;

    [SerializeField]
    protected float reqAmount;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject up;
    [SerializeField]
    protected GameObject down;


    // Use this for initialization
    void Start() {
        doorBasePos = door.transform.position;
        results = new Collider2D[3];
    }

    // Update is called once per frame
    void Update() {
        var num = col.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);

        for(int i = 0; i < num; i++) {
            var s = results[i].GetComponent<SlimeController>();
            var p = results[i].GetComponent<PlayerController>();
            if(p != null) {
                TryPress(p.GetWeight());
            }

            if(s != null) {
                TryPress(1);
            }
        }

        if(pressed) {
            up.SetActive(false);
            down.SetActive(true);
            door.transform.position = door.transform.position + Vector3.up * doorMoveSpeed * Time.deltaTime;
            if(door.transform.position.y > doorBasePos.y + doorMoveDistance) {
                door.transform.position = new Vector3(door.transform.position.x, doorBasePos.y + doorMoveDistance, door.transform.position.z);
            }
        } else {
            up.SetActive(true);
            down.SetActive(false);
            door.transform.position = door.transform.position - Vector3.up * doorMoveSpeed * Time.deltaTime;
            if(door.transform.position.y < doorBasePos.y) {
                door.transform.position = new Vector3(door.transform.position.x, doorBasePos.y, door.transform.position.z);
            }
        }
    }

    public void TryPress(int amount) {
        if(amount >= reqAmount) {
            pressed = true;
        }
    }

}
