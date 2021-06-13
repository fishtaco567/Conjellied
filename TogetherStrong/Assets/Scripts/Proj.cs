using UnityEngine;
using System.Collections;

public class Proj : MonoBehaviour {

    [SerializeField]
    protected Collider2D col;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject spawnAfterDestroy;

    public Vector2 vel;

    [SerializeField]
    protected Entities.CharacterController2D controller;

    [SerializeField]
    protected AudioClip playOnDestroy;

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
                p.Hit();
            }

            if(s != null) {
                s.Hit();
            }
        }

        controller.move(vel * Time.deltaTime);
        if(controller.collisionState.hasCollision()) {
            Dest();
        }
    }

    private void Dest() {
        Destroy(this.gameObject);
        var sp = Instantiate(spawnAfterDestroy);
        sp.transform.position = transform.position;
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
            AudioManager.Instance.Play(playOnDestroy);
        Destroy(sp, 1f);
    }
}
