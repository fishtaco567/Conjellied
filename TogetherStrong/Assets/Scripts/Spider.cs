using UnityEngine;
using System.Collections;
using Entities;

public class Spider : Enemy {

    public enum SpiderState {
        Idle,
        Patrol,
        Angry
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
    protected float angerFlipTime;

    protected Collider2D[] results;

    protected SpiderState state;

    protected float time;

    protected Vector2 velocity;

    protected int facing;

    [SerializeField]
    protected Animator anim;

    // Use this for initialization
    void Start() {
        results = new Collider2D[2];
        state = SpiderState.Idle;
        time = 0;
        velocity = Vector2.zero;
        facing = 1;
    }

    // Update is called once per frame
    void Update() {
        PlayerController player = null;
        var num = Physics2D.OverlapCircle(transform.position, angerRange, new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Slime") }, results);
        for(int i = 0; i < num; i++) {
            var p = results[i].GetComponent<PlayerController>();
            if(p != null) {
                player = p;
                break;
            }
        }

        if(player != null && state != SpiderState.Angry) {
            state = SpiderState.Angry;
        }

        time += Time.deltaTime;

        switch(state) {
            case SpiderState.Idle:
                velocity.x = 0;
                if(time > idleTime) {
                    state = SpiderState.Patrol;
                    time = 0;
                    facing = -facing;
                }
                break;
            case SpiderState.Patrol:
                velocity.x = facing * moveSpeed;
                if(time > moveTime) {
                    state = SpiderState.Patrol;
                    time = 0;
                }
                break;
            case SpiderState.Angry:
                velocity.x = facing * moveSpeed * 1.2f;
                if(player == null) {
                    state = SpiderState.Idle;
                    time = 0;
                    break;
                }
                if(time > angerFlipTime) {
                    time = 0;
                    facing = (int) Mathf.Sign(player.transform.position.x - transform.position.x);
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

        controller.move(velocity * Time.deltaTime);
    }

    public override void OnBounce(int amount) {
        //TODO: DIE
    }
}
