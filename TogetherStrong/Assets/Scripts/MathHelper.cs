using UnityEngine;

public static class MathHelper {

    public static float RandomRange(Vector2 v2) {
        return Random.Range(v2.x, v2.y);
    }

    public static int RandomRange(Vector2Int v2i) {
        return Random.Range(v2i.x, v2i.y);
    }

    public static Vector2 RandomPointInCircle(Vector2 center, float radius) {
        var theta = Random.Range(0, Mathf.PI * 2);
        var r = Random.Range(0, radius);

        return RadAngleToVector2(theta, r);
    }

    public static Vector2 AngleToVector2(float angle, float length) {
        float angleInDeg = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInDeg), Mathf.Sin(angleInDeg)) * length;
    }

    public static Vector2 RadAngleToVector2(float angle, float length) {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
    }

    public static float Lerp(float a, float b, float x) {
        return a + (b - a) * x;
    }
    public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 x) {
        return a + (b - a) * x;
    }

    public static Vector2 AsV2(this Vector3 v3) {
        return new Vector2(v3.x, v3.y);
    }

    public static bool Contains(Bounds a, Bounds b) {
        return a.min.x < b.min.x && a.min.y < b.min.y && a.max.x > b.max.x && a.max.y > b.max.y;
    }

    public static float CalculateSpeedToHit(float yAccel, float yVel, float yOffset, float distance) {
        var timeToFall = (-yVel - Mathf.Sqrt(yVel * yVel - 2 * yOffset * yAccel)) / yAccel;
        return distance / timeToFall;
    }

}