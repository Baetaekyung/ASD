using UnityEngine;

public static class MathUtils
{
    public static Vector3 GenerateRandomControlPoint(Vector3 start, Vector3 end, 
        float yOffset = 1.0f, float xOffset = 1.0f)
    {
        Vector3 midPoint = (start + end) / 2f;

        float randomHeight = Random.Range(-yOffset, yOffset);
        midPoint += Vector3.up * randomHeight;

        midPoint += new Vector3(Random.Range(-xOffset, xOffset), 0.0f);

        return midPoint;
    }

    public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }

    public static Vector3 GetRandomBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        Vector3 randomMiddlePoint = GenerateRandomControlPoint(p0, p1);
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * randomMiddlePoint + Mathf.Pow(t, 2) * p1;
    }
}
