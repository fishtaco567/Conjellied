using UnityEngine;
using System.Collections;
using Entities;

public class SlimeController : MonoBehaviour {

    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected float jumpSpeed;

    [SerializeField]
    protected float preJumpLength;

    [SerializeField]
    protected float giveUpTime;

    [SerializeField]
    protected float goalX;

    [SerializeField]
    protected bool moving;

    [SerializeField]
    protected bool facing;

    protected float timeNotMoving;

    protected float furthestPoint;

    [SerializeField]
    protected CharacterController2D controller;

    [SerializeField]
    protected SlimeRenderer slime;

    public Vector2 velocity;

    protected float gravity;

    [SerializeField]
    protected float gravReturn;

    // Use this for initialization
    void Start() {
        goalX = transform.position.x;
        moving = false;
        timeNotMoving = 0;
        furthestPoint = transform.position.x;
        facing = true;
        velocity = Vector2.zero;
        gravity = 1;
    }

    public void SetVel(Vector2 vel) {
        velocity = vel;
    }

    public void ReInit() {
        slime.ReInit();
        moving = false;
        goalX = transform.position.x;
        timeNotMoving = 0;
        facing = true;
        gravity = 1;
    }

    public void Hit() {
        velocity = new Vector2(Random.Range(-3, 3), Random.Range(5, 10));
    }

    // Update is called once per frame
    void Update() {
        if(controller.collisionState.below) {
            velocity.y = 0;
        } else {
            gravity = -1f;
        }

        velocity.y += Constants.GRAVITY * Time.deltaTime;
        if(moving) {

            if(transform.position.x > goalX && facing) {
                moving = false;
            } else if(transform.position.x < goalX && !facing) {
                moving = false;
            }

            if(transform.position.x < goalX && facing) {
                velocity.x = moveSpeed;

                if(transform.position.x > furthestPoint) {
                    furthestPoint = Mathf.Max(transform.position.x, furthestPoint);
                } else {
                    timeNotMoving += Time.deltaTime;
                }
            } else if(transform.position.x > goalX && !facing) {
                velocity.x = -moveSpeed;

                if(transform.position.x < furthestPoint) {
                    furthestPoint = Mathf.Min(transform.position.x, furthestPoint);
                } else {
                    timeNotMoving += Time.deltaTime;
                }
            }

            if(timeNotMoving > giveUpTime) {
                moving = false;
                timeNotMoving = 0;
            }

            var hit = Physics2D.Raycast(transform.position.AsV2() + Vector2.up * 0.2f, Vector2.right, preJumpLength, LayerMask.GetMask("Default"));
            if(hit.collider != null && controller.collisionState.below) {
                velocity.y += jumpSpeed;
            }
        } else {
            if(controller.collisionState.below)
                velocity.x = 0;
        }


        if(controller.collisionState.becameGroundedThisFrame && velocity.y < 10f) {
            gravity = 10;
        }
        gravity += gravReturn * Mathf.Sign(1 - gravity) * -1f;

        slime.curLean = velocity.x * 0.5f;
        slime.curGravMod = gravity;

        controller.move(velocity * Time.deltaTime);
    }

    public void Call(float x) {
        goalX = x;
        moving = true;
        facing = x > transform.position.x ? true : false;
    }

}
