using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpNoise;

public class SlimeRenderer : MonoBehaviour {

    [SerializeField]
    protected GameObject slimeballBase;

    [SerializeField]
    protected int maxNumSlimeballs;

    [SerializeField]
    protected AnimationCurve slimeballToSizeCurve;

    [SerializeField]
    protected float slimeballRadius;

    [SerializeField]
    protected float togetherCoef;

    [SerializeField]
    protected float apartCoef;

    [SerializeField]
    protected float perlinCoef;

    [SerializeField]
    protected float avoidSphereCoef;

    [SerializeField]
    protected float xCenter;

    [SerializeField]
    protected float sitOnTheFloor;

    [SerializeField]
    protected float gravityCoef;
    public float curGravMod;

    [SerializeField]
    protected float lean;

    [SerializeField]
    protected float maxTo;

    [SerializeField]
    protected float maxApt;

    public float curLean;

    protected List<GameObject> slimeballs;
    protected List<GameObject> eyes;

    protected List<Vector2> slimeballPositions;
    protected List<Vector2> slimeballVelocities;

    public int curNumSlimeballs;

    protected PerlinNoise perlin;

    protected Bounds boundingBox;

    [SerializeField]
    protected GameObject eyeBase;

    protected void Start() {
        eyes = new List<GameObject>();
        boundingBox.min = new Vector3(-.1f, 0);
        boundingBox.max = new Vector3(.1f, .1f);

        perlin = new PerlinNoise((uint)Time.frameCount);

        slimeballPositions = new List<Vector2>();
        slimeballVelocities = new List<Vector2>();
        slimeballs = new List<GameObject>();

        slimeballs.Add(slimeballBase);
        slimeballBase.SetActive(false);

        var neededSlimeballs = Mathf.Max(curNumSlimeballs, maxNumSlimeballs);

        for(int i = 0; i < maxNumSlimeballs - 1; i++) {
            var slimeball = Instantiate(slimeballBase);
            slimeball.transform.position = transform.position;
            slimeball.transform.parent = transform;
            slimeball.SetActive(false);
            slimeballs.Add(slimeball);

            if(i != 0 && i % 10 == 0) {
                var eye = Instantiate(eyeBase);
                eye.GetComponent<FollowThing>().thing = slimeball;
                eye.SetActive(false);
                eyes.Add(eye);
            }
        }

        var curRadius = slimeballToSizeCurve.Evaluate(curNumSlimeballs);

        for(int i = 0; i < curNumSlimeballs; i++) {
            slimeballs[i].SetActive(true);
            var pos = MathHelper.RandomPointInCircle(Vector2.zero, curRadius);
            slimeballPositions.Add(pos);
            slimeballVelocities.Add(Vector2.zero);
            slimeballs[i].transform.localPosition = pos + new Vector2(0, curRadius);
            if(i != 0 && i % 10 == 0) {
                var ind = (i / 10) - 1;
                eyes[ind].SetActive(true);
            }
        }
    }

    public void ReInit() {
        var curRadius = slimeballToSizeCurve.Evaluate(curNumSlimeballs);
        for(int i = 0; i < curNumSlimeballs; i++) {
            slimeballs[i].SetActive(true);
            var pos = MathHelper.RandomPointInCircle(Vector2.zero, curRadius);
            slimeballPositions.Add(pos);
            slimeballVelocities.Add(Vector2.zero);
            slimeballs[i].transform.localPosition = pos + new Vector2(0, curRadius);
        }
    }

    protected void Update() {
        boundingBox.min = new Vector3(-.1f, 0);
        boundingBox.max = new Vector3(.1f, .1f);
        var curRadius = slimeballToSizeCurve.Evaluate(curNumSlimeballs);
        var origin = new Vector2(0, curRadius);

        if(slimeballPositions.Count != curNumSlimeballs) {
            if(slimeballPositions.Count > curNumSlimeballs) {
                var toRemove = slimeballPositions.Count - curNumSlimeballs;
                for(int i = 0; i < toRemove; i++) {
                    var index = slimeballPositions.Count - 1;
                    slimeballPositions.RemoveAt(slimeballPositions.Count - 1);
                    slimeballVelocities.RemoveAt(slimeballPositions.Count - 1);
                    slimeballs[index].SetActive(false);
                    if(index != 0 && index % 10 == 0) {
                        var ind = (index / 10) - 1;
                        eyes[ind].SetActive(false);
                    }
                }
            } else if(slimeballPositions.Count < curNumSlimeballs) {
                var toAdd = curNumSlimeballs - slimeballPositions.Count;
                for(int i = 0; i < toAdd; i++) {
                    var index = slimeballPositions.Count;
                    var pos = MathHelper.RandomPointInCircle(transform.position.AsV2() + Vector2.up * curRadius, curRadius);
                    slimeballPositions.Add(pos);
                    slimeballVelocities.Add(Vector2.zero);
                    slimeballs[index].transform.localPosition = pos + origin;
                    slimeballs[index].SetActive(true);
                    if(index != 0 && index % 10 == 0) {
                        var ind = (index / 10) - 1;
                        eyes[ind].SetActive(true);
                    }
                }
            }
        }

        for(int i = 0; i < curNumSlimeballs; i++) {
            slimeballVelocities[i] = Vector2.zero;

            Vector2 to = Vector2.zero;
            Vector2 apt = Vector2.zero;
            for(int j = 0; j < curNumSlimeballs; j++) {
                if(i == j) {
                    continue;
                }

                //togetherness
                var vectorTo = (slimeballPositions[i] - slimeballPositions[j]);
                var vectorToMag = vectorTo.magnitude;
                var vectorToNorm = vectorTo.normalized;
                var toamt = Mathf.Min(vectorToMag, curRadius);
                to -= vectorToNorm * togetherCoef * toamt;

                //apartness
                if(i == 0) {
                    continue;
                }
                var aptamt = Mathf.Max(slimeballRadius - vectorToMag, 0);
                apt += vectorToNorm * apartCoef * aptamt;
            }

            if(to.magnitude > maxTo) {
                to = to.normalized * maxTo;
            }

            if(apt.magnitude > maxApt) {
                apt = apt.normalized * maxApt;
            }
            slimeballVelocities[i] += to + apt;

            //avoid circle
            var distFromOrigin = slimeballPositions[i].magnitude;
            var amountIn = Mathf.Max(0, (distFromOrigin + slimeballRadius) - curRadius) * avoidSphereCoef;
            slimeballVelocities[i] += -slimeballPositions[i].normalized * amountIn;


            //gravity
            slimeballVelocities[i] -= new Vector2(0, -gravityCoef) * curGravMod;

            //perlin
            slimeballVelocities[i] += new Vector2(perlin.GetNoise2D(Time.time + i * 3.259291f, .294920f), perlin.GetNoise2D(Time.time + i * 3.259291f, 4.42903219342f)) * perlinCoef;

            //xcenter
            slimeballVelocities[i] += new Vector2(-slimeballPositions[i].x, 0) * xCenter;

            //sitonthefloor
            slimeballVelocities[i] += new Vector2(0, -Mathf.Min(slimeballPositions[i].y - slimeballRadius + curRadius, 0)) * sitOnTheFloor;

            //lean
            slimeballVelocities[i] += new Vector2(slimeballPositions[i].y, 0) * lean * curLean;
        }

        slimeballVelocities[0] = new Vector2(slimeballVelocities[0].x + lean * curLean * 3, slimeballVelocities[0].y);

        var minX = 0.0f;
        var maxX = 0.0f;
        var maxY = 0.0f;
        for(int i = 0; i < curNumSlimeballs; i++) {
            var newPos = slimeballPositions[i] + slimeballVelocities[i] * Time.deltaTime;
            slimeballPositions[i] = newPos;
            var realPos = newPos + origin;
            slimeballs[i].transform.localPosition = newPos + origin;

            minX = Mathf.Min(minX, realPos.x - .25f);
            maxX = Mathf.Max(maxX, realPos.x + 0.25f);
            maxY = Mathf.Max(maxY, realPos.y + 0.25f);
        }

        boundingBox.min = new Vector2(minX, 0);
        boundingBox.max = new Vector2(maxX, maxY);
    }

    public Bounds GetBoundingBox() {
        return boundingBox;
    }

    protected void OnDrawGizmosSelected() {
        var curRadius = slimeballToSizeCurve.Evaluate(curNumSlimeballs);
        var origin = new Vector2(0, curRadius);
        Gizmos.DrawWireSphere(origin, curRadius);
    }

}
