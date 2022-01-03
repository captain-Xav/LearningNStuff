using UnityEngine;

public static class Utils
{
    public static Vector3 XZPlane(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);
}
