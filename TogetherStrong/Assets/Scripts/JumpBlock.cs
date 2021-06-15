using UnityEngine;
using System.Collections;
using Entities;

public class JumpBlock : Enemy {

    public enum JumpBlockState {
        Idle,
        PreJump,
        Jump
    }

    [SerializeField]
    protected AudioClip pop;
    [SerializeField]
    protected float maxJumpSpeedX;

    [SerializeField]
    protected float jumpSpeedY;

    [SerializeField]
    protected CharacterController2D controller;

    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected float idleTime;

    [SerializeField]
    protected float idleTimeAngry;

    [SerializeField]
    protected float angerRange;

    protected float ft;

    protected JumpBlockState state;

    protected float time;

    protected int numJumps;

    protected Vector2 vel;

    [SerializeField]
    protected GameObject smokePrefab;

    [SerializeField]
    protected int health = 4;

    [SerializeField]
    protected AudioClip fall;

    [SerializeField]
    protected GameObject fallPart;

    // Use this for initialization
    void Start() {
        ft = (2 * jumpSpeedY) / Constants.GRAVITY;
        state = JumpBlockState.Idle;
        time = 0;
        numJumps = 0;
        vel = Vector2.zero;
        facing = true;
    }

    protected bool facing;

    // Update is called once per frame
    void FixedUpdate() {
        if(controller.collisionState.below && vel.y < 0) {
            vel.y = 0;
        }

        time += Time.fixedDeltaTime;

        vel.y += Constants.GRAVITY * Time.fixedDeltaTime;
        bool angry = Vector2.Distance(GameManager.Instance.player.transform.position, transform.position) < angerRange;

        switch(state) {
            case JumpBlockState.Idle:
                vel.x = 0;
                if(angry && time > idleTimeAngry) {
                    anim.SetTrigger("Jump");
                    state = JumpBlockState.PreJump;
                    time = 0;
                } else if(time > idleTime) {
                    anim.SetTrigger("Jump");
                    state = JumpBlockState.PreJump;
                    time = 0;
                }
                break;
            case JumpBlockState.PreJump:
                if(angry && time > 0.2f) {
                    state = JumpBlockState.Jump;
                    time = 0;
                    vel.y = jumpSpeedY;
                    vel.x = Mathf.Min(Mathf.Abs(GameManager.Instance.player.transform.position.x - transform.position.x) / ft, maxJumpSpeedX) *
                        -Mathf.Sign(GameManager.Instance.player.transform.position.x - transform.position.x);
                } else if(time > 0.2f) {
                    if(numJumps > 1) {
                        facing = !facing;

                    }

                    state = JumpBlockState.Jump;
                    time = 0;
                    vel.y = jumpSpeedY;
                    vel.x = maxJumpSpeedX * 0.8f * (facing ? 1 : -1);
                }
                break;
            case JumpBlockState.Jump:
                if(controller.collisionState.below) {
                    if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                        AudioManager.Instance.PlayQuiet(fall);
                    var sp2 = Instantiate(fallPart);
                    sp2.transform.position = transform.position - Vector3.up;
                    Destroy(sp2, 3f);
                    numJumps += 1;
                    state = JumpBlockState.Idle;
                    time = 0;
                }
                break;
            default:
                break;
        }

        if(vel.x > 0) {
            facing = true;
        } else if(vel.x < 0) {
            facing = false;
        }
        anim.SetBool("Facing", facing);
        controller.move(vel * Time.fixedDeltaTime);
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
