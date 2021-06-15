using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {

    public PlayerController player;

    public List<SlimeController> slimes;

    [SerializeField]
    protected List<GameObject> respawnAfterDeath;

    protected List<GameObject> respawned;

    [SerializeField]
    protected BossOverlord boss;

    // Use this for initialization
    void Awake() {
        RegenList();
    }

    protected void RegenList() {
        respawned = new List<GameObject>();
        if(respawnAfterDeath == null) {
            return;
        }
        foreach(var go in respawnAfterDeath) {
            var sp = Instantiate(go);
            sp.transform.position = go.transform.position;
            sp.SetActive(false);
            sp.transform.parent = transform;
            respawned.Add(sp);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPlayerDeath() {
        player.transform.position = Vector3.zero;
        player.isDead = false;
        var offset = -3 * slimes.Count / 2;
        foreach(var go in respawnAfterDeath) {
            Destroy(go);
        }
        respawnAfterDeath = new List<GameObject>();
        foreach(var go in respawned) {
            go.SetActive(true);
            respawnAfterDeath.Add(go);
        }
        RegenList();
        foreach(SlimeController s in slimes) {
            s.transform.position = new Vector3(offset, 0, 0);
            offset += 3;
        }

        if(boss != null) {
            boss.Reset();
        }
    }

}
