using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    [SerializeField]
    protected PlayerController primaryFollow;

    [SerializeField]
    protected float easeSpeed;

    [SerializeField]
    protected Collider2D lookArea;

    [SerializeField]
    protected float pullAmount;

    protected Collider2D[] results;

    [SerializeField]
    protected GameObject mainMenuFocus;

    public bool inMainMenu;

    // Use this for initialization
    void Start() {
        results = new Collider2D[5];
        inMainMenu = true;
    }

    // Update is called once per frame
    void Update() {
        var num = lookArea.OverlapCollider(new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("OtherSlime", "Enemy") }, results);
        var targetPos = primaryFollow.transform.position;
        var avgPos = Vector3.zero;
        for(int i = 0; i < num; i++) {
            avgPos += (results[i].transform.position - targetPos) * Mathf.Max(0, (1 - (Vector2.Distance(transform.position, results[i].transform.position) / 15)));
        }
        if(num != 0) {
            avgPos = avgPos / num;
            avgPos *= Mathf.Sqrt(num) * pullAmount;
        }
        targetPos += new Vector3(primaryFollow.velocity.x, primaryFollow.velocity.y, 0) * 0.15f;
        targetPos += avgPos;

        if(inMainMenu) {
            targetPos = mainMenuFocus.transform.position;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPos.x, targetPos.y, transform.position.z), easeSpeed * Time.unscaledDeltaTime);
    }
}
