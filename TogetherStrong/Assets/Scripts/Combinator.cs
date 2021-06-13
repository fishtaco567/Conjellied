using UnityEngine;
using System.Collections.Generic;

public class Combinator : MonoBehaviour {

    [SerializeField]
    protected Collider2D col;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject smok;

    [SerializeField]
    protected AudioClip combine;

    // Use this for initialization
    void Start() {
        results = new Collider2D[5];
    }

    // Update is called once per frame
    void Update() {
        var num = col.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);

        PlayerController player = null;
        List<SlimeController> slimes = new List<SlimeController>();
        for(int i = 0; i < num; i++) {
            var s = results[i].GetComponent<SlimeController>();
            if(player == null) {
                var p = results[i].GetComponent<PlayerController>();
                Debug.Log(p);
                if(p != null) {
                    player = p;
                }
            }
            
            if(s != null) {
                slimes.Add(s);
                var sp = Instantiate(smok);
                sp.transform.position = transform.position;
                Destroy(sp.gameObject, 3f);
                if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                    AudioManager.Instance.Play(combine);
            }
        }

        if(player != null) {
            foreach(SlimeController s in slimes) {
                player.AddSlime(s);
            }
        }
    }
}
