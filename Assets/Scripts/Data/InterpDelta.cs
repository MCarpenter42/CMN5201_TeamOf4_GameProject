using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterpDelta
{
    public static float CosCurve(float deltaIn)
    {
        float rad = deltaIn * Mathf.PI;
        float cos = -Mathf.Cos(rad);
        return (cos + 1.0f) * 0.5f;
    }

    public static float SmoothedLinear(float x, float smoothing0to1)
    {
        // This is my personal experimental lerp smoothing type!

        // It uses half of a cos-type curve at either end, connected
        // by a straight line. The bulk of the calculation is to
        // coherently connect these three parts.

        // Due to the way this works, the smoothing value has to be
        // above 0.25 - below this value, it simply breaks.
        // Conversely, at a value of 1, the curve just becomes a
        // cos-type curve, and breaks again above that value.

        float y = 0.0f;

        float n = 0.25f + 0.75f * Mathf.Clamp(smoothing0to1, 0.0f, 1.0f);
        float p1 = Mathf.Sqrt(n) / 2 + 1;
        float p2 = Mathf.Sqrt(n) / 2;

        float piDivN = Mathf.PI / n;

        if (x < n / 2.0f)
        {
            y = (Mathf.Pow(n, p1) * (1 - Mathf.Cos(piDivN * x))) / 2;
        }
        else if (x > 1.0f - n / 2.0f)
        {
            y = 1 - (Mathf.Pow(n, p1) * (1 + Mathf.Cos(piDivN * x + Mathf.PI - piDivN))) / 2;
        }
        else if (x >= n / 2.0f && x <= 1.0f - n / 2.0f)
        {
            y = (Mathf.Pow(n, p2) * Mathf.PI) / 2 * Mathf.Sin(Mathf.PI / 2) * (x - 0.5f) + 0.5f;
        }
        else if (x == 0.5f)
        {
            y = 0.5f;
        }

        return y;
    }
}
