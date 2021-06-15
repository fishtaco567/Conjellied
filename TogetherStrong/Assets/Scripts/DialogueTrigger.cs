using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour {

    [SerializeField]
    protected Collider2D col;

    protected Collider2D[] results;

    protected bool hasTriggererd;

    [SerializeField]
    protected int id;

    [SerializeField]
    protected string lines;

    // Use this for initialization
    void Start() {
        results = new Collider2D[3];
        hasTriggererd = false;
    }

    // Update is called once per frame
    void Update() {
        if(hasTriggererd) {
            return;
        }

        var num = col.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime") }, results);

        for(int i = 0; i < num; i++) {
            var s = results[i].GetComponent<SlimeController>();
            var p = results[i].GetComponent<PlayerController>();
            if(p != null) {
                UIController.Instance.ShowTextbox(lines, id);
                hasTriggererd = true;
            }
        }
    }
}
