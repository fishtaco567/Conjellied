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

    protected float iframe;

    public bool givesHighJump;

    public bool givesShoot;

    public bool isProjectile;

    protected Collider2D[] results;

    // Use this for initialwization
    void Start() {
        results = new Collider2D[2];
        goalX = transform.position.x;
        moving = false;
        timeNotMoving = 0;
        furthestPoint = transform.position.x;
        facing = true;
        velocity = Vector2.zero;
        gravity = 1;
        iframe = 0;
        isProjectile = false;
    }

    public void SetVel(Vector2 vel) {
        velocity = vel;
        controller.collisionState.below = false;
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
        if(iframe > 0) {
            return;
        }

        iframe = 0.3f;

        velocity = new Vector2(Random.Range(-3, 3), Random.Range(5, 10));
        goalX = transform.position.x + Random.Range(-10, 10);
        moving = true;
        facing = goalX > transform.position.x ? true : false;
    }

    public void Shoot() {
        isProjectile = true;
    }

    // Update is called once per frame
    void Update() {
        iframe -= Time.deltaTime;
        if(controller.collisionState.below && velocity.y < 0) {
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

            var hit = Physics2D.Raycast(transform.position.AsV2() + Vector2.up * 0.2f, facing ? Vector2.right : Vector2.left, preJumpLength, LayerMask.GetMask("Default"));
            if(hit.collider != null && controller.collisionState.below) {
                velocity.y += jumpSpeed;
            }
        } else {
            if(controller.collisionState.below)
                velocity.x = 0;
        }

        if(isProjectile) {
            if(controller.collisionState.below) {
                isProjectile = false;
            }

            int num = controller.boxCollider.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Enemy") }, results);
            for(int i = 0; i < num; i++) {
                var en = results[i].GetComponent<Enemy>();
                if(en != null) {
                    en.OnBounce(1);
                }
            }
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
