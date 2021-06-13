using UnityEngine;
using System.Collections;
using Entities;

public class RollyBoi : Enemy {

    public enum RollyBoiState {
        Idle,
        Patrol,
        PreShoot,
        TakeAim,
        Shoot,
        PostShoot
    }

    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected CharacterController2D controller;

    [SerializeField]
    protected float angerRange;

    [SerializeField]
    protected float idleTime;

    [SerializeField]
    protected float moveTime;

    [SerializeField]
    protected float shootTime;

    [SerializeField]
    protected float preShootTime;

    [SerializeField]
    protected float shootRange;

    [SerializeField]
    protected float postShootTime;

    protected Collider2D[] results;

    [SerializeField]
    protected RollyBoiState state;

    protected float time;

    protected Vector2 velocity;

    protected int facing;

    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected LineRenderer line1;
    [SerializeField]
    protected LineRenderer line2;

    protected Vector2 shootline;

    [SerializeField]
    protected GameObject arm;

    [SerializeField]
    protected int health = 2;

    [SerializeField]
    protected GameObject smokePrefab;

    [SerializeField]
    protected AudioClip shoot;

    // Use this for initialization
    void Start() {
        results = new Collider2D[2];
        state = RollyBoiState.Idle;
        time = 0;
        velocity = Vector2.zero;
        facing = 1;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(controller.collisionState.below) {
            velocity.y = 0;
        }

        velocity.y += Constants.GRAVITY * Time.fixedDeltaTime;

        PlayerController player = null;
        var num = Physics2D.OverlapCircle(transform.position, shootRange, new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime") }, results);
        for(int i = 0; i < num; i++) {
            var p = results[i].GetComponent<PlayerController>();
            if(p != null) {
                player = p;
                break;
            }
        }

        time += Time.fixedDeltaTime;

        switch(state) {
            case RollyBoiState.Idle:
                velocity.x = 0;
                arm.transform.eulerAngles = new Vector3(0, 0, 0);
                if(time > idleTime) {
                    if(player != null) {
                        state = RollyBoiState.PreShoot;
                        line1.gameObject.SetActive(true);
                        line2.gameObject.SetActive(true);
                        Vector3 v24 = GameManager.Instance.player.transform.position - transform.position;
                        v24 = v24.normalized * 2;
                        line1.SetPosition(1, v24);
                        line2.SetPosition(1, v24);
                        facing = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
                        time = 0;
                    } else {
                        state = RollyBoiState.Patrol;
                        time = 0;
                        num = Physics2D.OverlapCircle(transform.position, angerRange, new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime") }, results);
                        for(int i = 0; i < num; i++) {
                            var p = results[i].GetComponent<PlayerController>();
                            if(p != null) {
                                player = p;
                                break;
                            }
                        }

                        if(player != null) {
                            facing = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
                        } else {
                            facing = -facing;
                        }
                    }
                }
                break;
            case RollyBoiState.Patrol:
                velocity.x = facing * moveSpeed;
                arm.transform.eulerAngles = new Vector3(0, 0, 0);
                if(time > moveTime) {
                    if(player != null) {
                        state = RollyBoiState.PreShoot;
                        line1.gameObject.SetActive(true);
                        line2.gameObject.SetActive(true);
                        Vector3 v25 = GameManager.Instance.player.transform.position - transform.position;
                        v25 = v25.normalized * 2;
                        line1.SetPosition(1, v25);
                        line2.SetPosition(1, v25);
                        facing = (int)Mathf.Sign(player.transform.position.x - transform.position.x);
                        time = 0;
                    } else {
                        state = RollyBoiState.Idle;
                        time = 0;
                    }
                }
                break;
            case RollyBoiState.PreShoot:
                velocity.x = 0;
                Vector3 v2 = GameManager.Instance.player.transform.position - transform.position;
                v2 = v2.normalized * 2;
                line1.SetPosition(1, v2);
                line2.SetPosition(1, v2);
                line2.startWidth = 0;
                line2.endWidth = 0;
                arm.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, v2.normalized));

                if(time > preShootTime) {
                    shootline = v2.normalized;
                    state = RollyBoiState.TakeAim;
                    time = 0;
                }
                break;
            case RollyBoiState.TakeAim:
                velocity.x = 0;
                var v26 = shootline.normalized * (2 + (shootRange - 4) * (time / 0.5f));
                line1.SetPosition(1, v26);
                line2.SetPosition(1, v26);
                arm.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, shootline.normalized));

                if(time > 0.5f) {
                    anim.SetTrigger("Shoot");
                    Hit();
                    state = RollyBoiState.Shoot;
                    time = 0;
                }
                break;
            case RollyBoiState.Shoot:
                velocity.x = 0;
                var v22 = shootline.normalized * (2 + (shootRange - 2) * (time / shootTime));
                line1.SetPosition(1, v22);
                line2.SetPosition(1, v22);
                line2.startWidth = 1;
                line2.endWidth = 1;
                line1.startWidth = 0.3f;
                line1.endWidth = 0.3f;
                arm.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, shootline.normalized));
                if(time > shootTime) {
                    state = RollyBoiState.PostShoot;
                    time = 0;
                }
                break;
            case RollyBoiState.PostShoot:
                velocity.x = 0;
                var angle = -Vector2.SignedAngle(Vector2.down, shootline.normalized);
                var da = (-4f * (1 - time / postShootTime)) * time * Mathf.Sign(angle);
                shootline = new Vector2(Mathf.Cos(da) * shootline.x - Mathf.Sin(da) * shootline.y, Mathf.Sin(da) * shootline.x + Mathf.Cos(da) * shootline.y);
                var v23 = shootline.normalized * (shootRange - (shootRange - 2) * (time / postShootTime));
                line1.SetPosition(1, v23);
                line2.SetPosition(1, v23);
                line1.startWidth = 0.1f;
                line1.endWidth = 0.1f;
                line2.startWidth = (1 - (1f) * (time / postShootTime));
                line2.endWidth = (1 - (1f) * (time / postShootTime));
                arm.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, shootline.normalized));
                if(time > postShootTime) {
                    anim.ResetTrigger("Shoot");
                    state = RollyBoiState.Idle;
                    time = 0;
                    line1.gameObject.SetActive(false);
                    line2.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

        if(facing == 1) {
            anim.SetBool("Facing", true);
        } else {
            anim.SetBool("Facing", false);
        }

        if(Mathf.Abs(velocity.x) > 0.1f) {
            anim.SetBool("Moving", true);
        } else {
            anim.SetBool("Moving", false);
        }

        controller.move(velocity * Time.fixedDeltaTime);
    }

    [SerializeField]
    protected AudioClip pop;

    protected void Hit() {
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
            AudioManager.Instance.Play(shoot);
        var hit = Physics2D.Raycast(transform.position, shootline, shootRange, LayerMask.GetMask("Slime"));
        if(hit.collider != null) {
            var p = hit.collider.GetComponent<PlayerController>();
            if(p != null) {
                p.Hit();
            }
        }

        var hit2 = Physics2D.Raycast(transform.position, shootline, shootRange, LayerMask.GetMask("OtherSlime"));
        if(hit2.collider != null) {
            var s = hit2.collider.GetComponent<SlimeController>();
            if(s != null) {
                s.Hit();
            }
        }
    }

    public override void OnBounce(int amount) {
        health -= amount;
        if(health <= 0) {
            var sp = Instantiate(smokePrefab);
            sp.transform.position = transform.position;
            Destroy(sp, 1f);
            Destroy(this.gameObject);
            if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                AudioManager.Instance.Play(pop);
        }
        //TODO: DIE
    }
}
