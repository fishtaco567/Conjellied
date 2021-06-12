using UnityEngine;
using System.Collections;

public class TouchDamage : MonoBehaviour {

    [SerializeField]
    protected Collider2D col;

    protected Collider2D[] results;

    [SerializeField]
    protected bool noHitAbove;

    // Use this for initialization
    void Start() {
        results = new Collider2D[3];
    }

    // Update is called once per frame
    void Update() {
        var num = col.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);

        for(int i = 0; i < num; i++) {
            var s = results[i].GetComponent<SlimeController>();
            var p = results[i].GetComponent<PlayerController>();
            if(p != null) {
                if(noHitAbove && p.transform.position.y + 0.2f > col.bounds.max.y && p.velocity.y < 0) {
                    p.Bounce(this.gameObject);
                    continue;
                }

                p.Hit();
            }
        }
    }
}
