using System.Collections.Generic;
using Entities;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    protected AnimationCurve moveSpeed;

    [SerializeField]
    protected AnimationCurve jumpSpeed;

    [SerializeField]
    protected AnimationCurve chargeJumpSpeed;

    [SerializeField]
    protected int weightPerSlime;

    [SerializeField]
    protected int damagePerSlime;

    [SerializeField]
    protected int numSlimes;

    [SerializeField]
    protected SlimeRenderer slimeRenderer;

    [SerializeField]
    protected int slimeballsPerSlime = 10;

    [SerializeField]
    protected float drag;

    [SerializeField]
    protected float reqJumpChargeTime;

    [SerializeField]
    protected CharacterController2D controller;

    public Vector2 velocity;

    protected float jumpChargeTime;

    protected float curGravMod;

    [SerializeField]
    float gravModReturnPerSec;

    [SerializeField]
    protected SpriteRenderer eyeRenderer;

    [SerializeField]
    protected BoxCollider2D col;

    protected Collider2D[] results;

    protected Rewired.Player replayer;

    [SerializeField]
    protected float callRange;

    protected int facing;

    protected List<SlimeController> internalSlimes;

    protected bool canHighJump;
    protected bool canShoot;

    public GameObject hjsp;
    public GameObject shsp;

    public GameObject callobj;
    public float callobjTime;

    [SerializeField]
    protected AudioClip bounceSound;
    [SerializeField]
    protected AudioClip hurt;
    [SerializeField]
    protected AudioClip jumpS;
    [SerializeField]
    protected AudioClip land;
    [SerializeField]
    protected AudioClip throweeee;

    [SerializeField]
    protected GameObject crown;

    [SerializeField]
    protected ParticleSystem trail;

    public bool hasWon;

    protected void Start() {
        slimeRenderer.curNumSlimeballs = slimeballsPerSlime * numSlimes;
        velocity = Vector2.zero;
        jumpChargeTime = 0;

        curGravMod = 1;
        facing = 1;

        replayer = Rewired.ReInput.players.GetPlayer(0);

        results = new Collider2D[5];
        internalSlimes = new List<SlimeController>();
        hasWon = false;
    }

    protected float lastFramevel;

    protected void FixedUpdate() {
        if(hasWon) {
            crown.SetActive(true);
        }

        lastFramevel = velocity.y;

        if(controller.collisionState.below && Mathf.Abs(velocity.x) > 0.1f) {
            var em = trail.emission;
            em.enabled = true;
        } else {
            var em = trail.emission;
            em.enabled = false;
        }

        GameManager.Instance.player = this;

        if(canHighJump) {
            hjsp.SetActive(true);
        } else {
            hjsp.SetActive(false);
        }

        if(canShoot) {
            shsp.SetActive(true);
        } else {
            shsp.SetActive(false);
        }

        var curMoveSpeed = moveSpeed.Evaluate(numSlimes);
        var curJumpSpeed = jumpSpeed.Evaluate(numSlimes);
        var curChargeJumpSpeed = chargeJumpSpeed.Evaluate(numSlimes);
        var curWeight = weightPerSlime * numSlimes;
        var curDamage = damagePerSlime * numSlimes;

        var horiz = replayer.GetAxis("Horizontal");
        var vert = replayer.GetAxis("Vertical");
        var jump = replayer.GetButtonDown("Jump");
        var jumpHeld = replayer.GetButton("SuperJump");
        var shoot = replayer.GetButtonDown("Shoot");
        var call = replayer.GetButton("Call");

        if(controller.collisionState.below && jump && !jumpHeld && jumpChargeTime == 0) {
            velocity.y = curJumpSpeed;
            AudioManager.Instance.PlayQuiet(jumpS);
        } else if(controller.collisionState.below && velocity.y < 0) {
            velocity.y = 0;
        }

        if(velocity.y > 0 && controller.collisionState.above) {
            velocity.y -= Mathf.Min(velocity.y, 1f);
        }

        if(controller.collisionState.below && !jump && jumpHeld && canHighJump) {
            jumpChargeTime += Time.fixedDeltaTime;
            curGravMod = Mathf.Max(curGravMod, Mathf.Min((jumpChargeTime / reqJumpChargeTime), 1) * 5f);
        } else if(controller.collisionState.below && !jumpHeld && jumpChargeTime > 0.3f) {
            velocity.y = curChargeJumpSpeed * Mathf.Min(1, jumpChargeTime / reqJumpChargeTime);
            AudioManager.Instance.PlayQuiet(jumpS);
            jumpChargeTime = 0;
        } else {
            jumpChargeTime = 0;
        }

        if(!controller.collisionState.below) {
            curGravMod = -1f;
        }

        if(controller.collisionState.becameGroundedThisFrame && lastFramevel < -2f) {
            AudioManager.Instance.PlayQuiet(land);
            curGravMod = Mathf.Min(10 * (-lastFramevel / 15f), 10);
        }

        curGravMod += gravModReturnPerSec * Mathf.Sign(1 - curGravMod) * -1f;
        slimeRenderer.curGravMod = curGravMod;

        velocity.y += Constants.GRAVITY * Time.fixedDeltaTime;

        if(Mathf.Abs(horiz) > 0.1f) {
            velocity.x = horiz * curMoveSpeed;
        } else {
            if(velocity.x > 0) {
                velocity.x -= drag;
                if(velocity.x < 0) {
                    velocity.x = 0;
                }
            } else if(velocity.x < 0) {
                velocity.x += drag;
                if(velocity.x > 0) {
                    velocity.x = 0;
                }
            }
        }

        slimeRenderer.curLean = velocity.x * 0.5f;

        if(velocity.x == 0) {
            velocity.x = Time.frameCount % 2 == 0 ? 0.01f : -0.01f;
        }

        if(velocity.y < -25f) {
            velocity.y = -25f;
        }

        if(!isDead) {
            controller.move(velocity * Time.fixedDeltaTime);
            slimeRenderer.avoidSphereCoef = 1;
            slimeRenderer.xCenter = 1;
        } else {
            curGravMod = 10;
            slimeRenderer.avoidSphereCoef = 0;
            slimeRenderer.xCenter = 0;
        }

        if(Mathf.Abs(velocity.x) > 0.1f) {
            if(velocity.x > 0) {
                facing = 1;
            } else {
                facing = -1;
            }
        }

        if(facing == 1) {
            eyeRenderer.flipX = false;
        } else {
            eyeRenderer.flipX = true;
        }

        var newBounds = slimeRenderer.GetBoundingBox();

        col.offset = newBounds.center;
        col.size = newBounds.size;

        controller.recalculateDistanceBetweenRays();

        var callPressed = replayer.GetButtonDown("Call");
        if(call) {
            if(callPressed) {
                AudioManager.Instance.PlayQuiet(callSound);
            }

            callobj.SetActive(true);
            callobjTime = 1f;
            var num = Physics2D.OverlapCircle(transform.position.AsV2(), callRange, new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("OtherSlime") }, results);
            for(int i = 0; i < num; i++) {
                var sc = results[i].GetComponent<SlimeController>();
                if(sc != null) {
                    sc.Call(transform.position.x + facing * 2 + Random.Range(-1f, 1f));
                }
            }
        }

        callobjTime -= Time.fixedDeltaTime;
        if(callobjTime < 0) {
            callobj.SetActive(false);
        }

        if(shoot && canShoot && numSlimes > 1) {
            var slime = RemoveSlime();
            slime.ReInit();
            slime.transform.position = transform.position + Vector3.up * numSlimes * 0.5f;
            slime.Shoot();
            slime.SetVel(new Vector2(facing * 10, 7) + velocity / 2);
            AudioManager.Instance.Play(throweeee);
        }

        iframeTime -= Time.fixedDeltaTime;
    }

    protected float iframeTime;

    [SerializeField]
    protected AudioClip callSound;

    public void Hit() {
        if(iframeTime > 0) {
            return;
        }
        if(!isDead)
            AudioManager.Instance.PlayQuiet(hurt);

        velocity = new Vector2(Random.Range(-3, 3), Random.Range(5, 10));

        if(numSlimes == 1 && ! isDead) {
            isDead = true;
            StartCoroutine(OnDeath());
            return;
        }

        var numToRemove = numSlimes / 2;

        iframeTime = 0.5f;

        for(int i = 0; i < numToRemove; i++) {
            var removed = RemoveSlime();
            var randPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f) * numSlimes, Random.Range(.25f, 0.5f) * numSlimes, 0);
            removed.transform.position = randPos;
            removed.SetVel(removed.transform.position - transform.position);
            removed.ReInit();
        }
    }

    [SerializeField]
    protected GameObject bopBoop;

    public void Bounce(GameObject other) {
        AudioManager.Instance.Play(bounceSound);
        iframeTime = 0.5f;
        curGravMod = 20;
        velocity.y = jumpSpeed.Evaluate(numSlimes) * 0.66f;

        var en = other.GetComponent<Enemy>();
        if(en != null) {
            en.OnBounce(damagePerSlime * numSlimes);
            var sp = Instantiate(bopBoop);
            sp.transform.position = transform.position;
            Destroy(sp, 2f);
        }
    }

    public void AddSlime(SlimeController slime) {
        if(slime.givesHighJump || slime.givesShoot) {
            internalSlimes.Insert(0, slime);
        } else {
            internalSlimes.Add(slime);
        }
        slime.gameObject.SetActive(false);
        numSlimes += 1;
        slimeRenderer.curNumSlimeballs = slimeballsPerSlime * numSlimes;

        if(slime.givesHighJump) {
            canHighJump = true;
        }

        if(slime.givesShoot) {
            canShoot = true;
        }
    }

    public SlimeController RemoveSlime() {
        numSlimes -= 1;
        slimeRenderer.curNumSlimeballs = slimeballsPerSlime * numSlimes;
        var slime = internalSlimes[internalSlimes.Count - 1];
        internalSlimes.Remove(slime);
        slime.gameObject.SetActive(true);

        if(slime.givesHighJump) {
            canHighJump = false;
        }

        if(slime.givesShoot) {
            canShoot = false;
        }
        if(numSlimes < 1) {
            //TODO: dead
        }

        return slime;
    }

    public int GetWeight() {
        return numSlimes * weightPerSlime;
    }

    public bool isDead = false;

    public System.Collections.IEnumerator OnDeath() {
        yield return new WaitForSeconds(1f);
        UIController.Instance.GameOverOn(true);
        yield return new WaitForSeconds(2f);
        UIController.Instance.GameOverOn(false);
        GameManager.Instance.OnPlayerDeath();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(0);   
    }

}