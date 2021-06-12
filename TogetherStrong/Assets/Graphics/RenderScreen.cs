using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderScreen : MonoBehaviour {

    [SerializeField]
    private RenderTexture texture = default;

    [SerializeField]
    private RenderTexture effectTex = default;

    public Material mat = default;

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}

    private void OnPreRender() {
        cam.targetTexture = texture;
    }

    // Update is called once per frame
    void OnPostRender () {
        cam.targetTexture = null;
        if(mat != null) {
            var tmp = RenderTexture.GetTemporary(480, 270);
            tmp.filterMode = FilterMode.Point;

            Graphics.Blit(texture, tmp, mat);
            var oldTex = Camera.main.targetTexture;
            Camera.main.targetTexture = null;
            Graphics.Blit(tmp, null as RenderTexture);
            Camera.main.targetTexture = oldTex;

            RenderTexture.ReleaseTemporary(tmp);
        } else {
            Graphics.Blit(texture, null as RenderTexture);
        }
	}
}
