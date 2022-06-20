using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterpDelta
{
    public enum InterpTypes { Linear, CosCurve, CosSpeedUp, CosSlowDown, CosSpeedUpSlowDown, CosSlowDownSpeedUp, SmoothedLinear };

    public static float CosCurve(float rawDelta)
    {
        rawDelta = Mathf.Clamp(rawDelta, 0.0f, 1.0f);
        float rad = rawDelta * Mathf.PI;
        float cos = -Mathf.Cos(rad);
        float output = (cos + 1.0f) * 0.5f;
        return output;
    }
    
    public static float CosSpeedUp(float rawDelta)
    {
        rawDelta = Mathf.Clamp(rawDelta, 0.0f, 1.0f);
        float rad = rawDelta - 2.0f;
        rad *= Mathf.PI;
        rad /= 2.0f;
        float cos = Mathf.Cos(rad);
        float output = 1.0f + cos;
        return output;
    }

    public static float CosSlowDown(float rawDelta)
    {
        rawDelta = Mathf.Clamp(rawDelta, 0.0f, 1.0f);
        float rad = rawDelta - 1.0f;
        rad *= Mathf.PI;
        rad /= 2.0f;
        float output = Mathf.Cos(rad);
        return output;
    }
    
    public static float CosSpeedUpSlowDown(float rawDelta, bool forward)
    {
        float output;
        if (forward)
        {
            output = CosSpeedUp(rawDelta);
        }
        else
        {
            output = CosSlowDown(rawDelta);
        }
        return output;
    }
    
    public static float CosSlowDownSpeedUp(float rawDelta, bool forward)
    {
        float output;
        if (forward)
        {
            output = CosSlowDown(rawDelta);
        }
        else
        {
            output = CosSpeedUp(rawDelta);
        }
        return output;
    }

    public static float SmoothedLinear(float rawDelta, float smoothing0to1)
    {
        // This is my personal experimental lerp smoothing type!

        // It uses half of a cos-type curve at either end, connected
        // by a straight line. The bulk of the calculation is to
        // coherently connect these three parts.

        // Due to the way this works, the smoothing value has to be
        // above 0.25 - below this value, it simply breaks.
        // Conversely, at a value of 1, the curve just becomes a
        // cos-type curve, and breaks again above that value.

        float output = 0.0f;

        float n = 0.25f + 0.75f * Mathf.Clamp(smoothing0to1, 0.0f, 1.0f);
        float p1 = Mathf.Sqrt(n) / 2 + 1;
        float p2 = Mathf.Sqrt(n) / 2;

        float piDivN = Mathf.PI / n;

        if (rawDelta < n / 2.0f)
        {
            output = (Mathf.Pow(n, p1) * (1 - Mathf.Cos(piDivN * rawDelta))) / 2;
        }
        else if (rawDelta > 1.0f - n / 2.0f)
        {
            output = 1 - (Mathf.Pow(n, p1) * (1 + Mathf.Cos(piDivN * rawDelta + Mathf.PI - piDivN))) / 2;
        }
        else if (rawDelta >= n / 2.0f && rawDelta <= 1.0f - n / 2.0f)
        {
            output = (Mathf.Pow(n, p2) * Mathf.PI) / 2 * Mathf.Sin(Mathf.PI / 2) * (rawDelta - 0.5f) + 0.5f;
        }
        else if (rawDelta == 0.5f)
        {
            output = 0.5f;
        }

        return output;
    }



    private static float ToRad(float degrees)
    {
        return degrees * Mathf.PI / 180.0f;
    }

    private static float ToDeg(float radians)
    {
        return radians * 180.0f / Mathf.PI;
    }
}
