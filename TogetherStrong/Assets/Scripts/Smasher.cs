using UnityEngine;
using System.Collections;

public class Smasher : MonoBehaviour {

    public enum SmasherState {
        Wait,
        Down,
        WaitBottom,
        Up
    }

    public float speedDown;
    public float speedUp;

    public float waitTop;
    public float waitUp;

    public Entities.CharacterController2D controller;

    public SmasherState state;
    public float time;

    public Vector2 vel;

    public Sprite spriteInactive;
    public Sprite spriteActive;
    public SpriteRenderer sr;

    [SerializeField]
    protected AudioClip fall;

    [SerializeField]
    protected GameObject fallPart;

    // Use this for initialization
    void Start() {
        state = SmasherState.Wait;
        time = 0;
        vel = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate() {
        time += Time.fixedDeltaTime;

        switch(state) {
            case SmasherState.Wait:
                sr.sprite = spriteInactive;
                vel.y = 0;
                if(time > waitTop) {
                    state = SmasherState.Down;
                    time = 0;
                }
                break;
            case SmasherState.Down:
                sr.sprite = spriteActive;
                vel.y = Mathf.Min((time / 0.25f) * speedDown, speedDown);

                if(controller.collisionState.below) {
                    if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                        AudioManager.Instance.Play(fall);
                    var sp = Instantiate(fallPart);
                    sp.transform.position = transform.position - Vector3.up;
                    Destroy(sp, 3f);
                    state = SmasherState.WaitBottom;
                    time = 0;
                }
                break;
            case SmasherState.WaitBottom:
                sr.sprite = spriteInactive;
                vel.y = 0;
                if(time > waitUp) {
                    state = SmasherState.Up;
                    time = 0;
                }
                break;
            case SmasherState.Up:
                sr.sprite = spriteInactive;
                vel.y = Mathf.Min((time / 0.25f) * speedUp, speedUp);

                if(controller.collisionState.above) {
                    state = SmasherState.Wait;
                    time = 0;
                }
                break;
            default:
                break;
        }

        controller.move(vel * Time.fixedDeltaTime);
    }
}
