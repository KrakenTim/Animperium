using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector4Extension
{
    /// <summary>
    /// Returns true if the x and y part are within the ranges of x-y and z-w respectively. 
    /// </summary>
    public static bool InArea(this Vector4 extended, Vector2 position)
    {
        return extended.InArea(position.x, position.y);
    }

    /// <summary>
    /// Returns true if the x and z are within the ranges of x-y and z-w respectively. 
    /// </summary>
    public static bool InArea(this Vector4 extended, float x, float z)
    {
        return x >= extended.x && x <= extended.y && z >= extended.z && z <= extended.w;
    }

    /// <summary>
    /// Returns true if the x and z part are within the ranges of x-y and z-w respectively. 
    /// </summary>
    public static bool InArea(this Vector4 extended, Vector3 position)
    {
        return extended.InArea(position.x, position.z);
    }
}
