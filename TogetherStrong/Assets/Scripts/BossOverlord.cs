using UnityEngine;
using System.Collections;

public class BossOverlord : MonoBehaviour {

    public enum BossState {
        Shooting,
        ShieldsDown
    }

    public BossState state;

    [SerializeField]
    protected float timeBetweenShots;

    [SerializeField]
    protected Collider2D button1;
    [SerializeField]
    protected Collider2D button2;

    [SerializeField]
    protected SpriteRenderer b1r;
    [SerializeField]
    protected SpriteRenderer b2r;

    [SerializeField]
    protected Sprite buttonUnpressed;
    [SerializeField]
    protected Sprite buttonPressed;

    [SerializeField]
    protected GameObject spawn;

    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected GameObject[] shootAnchors;

    [SerializeField]
    protected GameObject combinator;
    [SerializeField]
    protected float moveCombinator;

    [SerializeField]
    protected Collider2D combCollider;

    protected Vector3 combinatorBasePos;

    protected float timeShot;

    protected bool b1p;
    protected bool b2p;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject hurtbox;

    [SerializeField]
    protected Collider2D tenderSpot;

    protected float shieldDownTime;

    [SerializeField]
    protected float shieldDownMaxTime;

    public int health;
    protected float iFrameTime;

    [SerializeField]
    protected GameObject smoke;

    [SerializeField]
    protected GameObject preDeathSmoke;

    [SerializeField]
    protected GameObject onWhenDead;

    [SerializeField]
    protected GameObject destroyWhenDead;

    [SerializeField]
    protected AudioClip shootSouind;

    [SerializeField]
    protected AudioClip endExp;

    // Use this for initialization
    void Start() {
        combinatorBasePos = combinator.transform.position;
        state = BossState.Shooting;
        timeShot = 0;
        b1p = false;
        b2p = false;
        results = new Collider2D[4];
        shieldDownTime = 0;
        iFrameTime = 0;
    }

    // Update is called once per frame
    void Update() {
        iFrameTime -= Time.deltaTime;

        switch(state) {
            case BossState.Shooting:
                timeShot += Time.deltaTime;

                var num = button1.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);
                for(int i = 0; i < num; i++) {
                    var s = results[i].GetComponent<SlimeController>();
                    var p = results[i].GetComponent<PlayerController>();
                    if(s != null) {
                        if(s.velocity.y < 0) {
                            b1p = true;
                        }
                    }
                    if(p != null) {
                        if(p.velocity.y < 0) {
                            b1p = true;
                        }
                    }
                }

                num = button2.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);
                for(int i = 0; i < num; i++) {
                    var s = results[i].GetComponent<SlimeController>();
                    var p = results[i].GetComponent<PlayerController>();
                    if(s != null) {
                        if(s.velocity.y < 0) {
                            b2p = true;
                        }
                    }
                    if(p != null) {
                        if(p.velocity.y < 0) {
                            b2p = true;
                        }
                    }
                }

                if(b1p) {
                    b1r.sprite = buttonPressed;
                } else {
                    b1r.sprite = buttonUnpressed;
                }

                if(b2p) {
                    b2r.sprite = buttonPressed;
                } else {
                    b2r.sprite = buttonUnpressed;
                }

                if(b1p && b2p) {
                    state = BossState.ShieldsDown;
                    anim.SetBool("Shield", false);
                    hurtbox.SetActive(false);
                    shieldDownTime = 0;

                    for(int i = 0; i < 3; i++) {
                        var sp = Instantiate(smoke);
                        sp.transform.position = transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
                    }
                }

                combinator.transform.position = combinator.transform.position - Vector3.up * 2f * Time.deltaTime;
                if(combinator.transform.position.y < combinatorBasePos.y - moveCombinator) {
                    combinator.transform.position = new Vector3(combinator.transform.position.x, combinatorBasePos.y - moveCombinator, combinator.transform.position.z);
                }
                combCollider.enabled = false;
                
                if(timeShot > timeBetweenShots) {
                    Shoot();
                }
                break;
            case BossState.ShieldsDown:
                shieldDownTime += Time.deltaTime;

                num = tenderSpot.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime", "OtherSlime") }, results);
                for(int i = 0; i < num; i++) {
                    var s = results[i].GetComponent<SlimeController>();
                    if(s != null) {
                        if(s.isProjectile && iFrameTime < 0) {
                            health -= 1;
                            iFrameTime = 0.5f;
                            for(int j = 0; j < 10; j++) {
                                var sp2 = Instantiate(smoke);
                                sp2.transform.position = transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
                            }
                        }

                    }
                }

                combinator.transform.position = combinator.transform.position + Vector3.up * 2f * Time.deltaTime;
                if(combinator.transform.position.y > combinatorBasePos.y) {
                    combinator.transform.position = new Vector3(combinator.transform.position.x, combinatorBasePos.y, combinator.transform.position.z);
                }
                combCollider.enabled = true;

                if(shieldDownTime > shieldDownMaxTime) {
                    state = BossState.Shooting;
                    anim.SetBool("Shield", true);
                    hurtbox.SetActive(true);
                    b1p = false;
                    b2p = false;

                    for(int i = 0; i < 3; i++) {
                        var sp = Instantiate(smoke);
                        sp.transform.position = transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
                    }
                }
                break;
            default:
                break;
        }

        if(health <= 0 && dead == false) {
            dead = true;
            var sp = Instantiate(preDeathSmoke);
            sp.transform.position = transform.position;
            if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                AudioManager.Instance.Play(endExp);
            Destroy(sp.gameObject, 10f);
            Destroy(destroyWhenDead, 1f);
            StartCoroutine(AfterDeath());
        }
    }

    bool dead = false;

    public IEnumerator AfterDeath() {
        yield return new WaitForSeconds(4.5f);

        onWhenDead.SetActive(true);
    }

    public void Shoot() {
        timeShot = 0;
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
            AudioManager.Instance.Play(shootSouind);
        var sp = Instantiate(spawn);
        var pr = sp.GetComponent<Proj>();
        sp.transform.position = shootAnchors[Random.Range(0, shootAnchors.Length)].transform.position;
        pr.vel = (GameManager.Instance.player.transform.position - sp.transform.position).normalized * 12f;
    }
}
