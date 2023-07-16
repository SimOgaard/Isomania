using System;
using UnityEngine;

public static class MathExtensions
{
    public static float ClampRotation(this float rotation)
    {
        return (rotation + 360) % 360;
    }

    public static int GetCeiledEvenNumber(float number)
    {
        int ceiledNumber = (int)Math.Ceiling(number);
        if (ceiledNumber % 2 != 0) // Check if the number is odd
        {
            ceiledNumber++;        // Increment by 1 to make it even
        }

        return ceiledNumber;
    }
    public static int GetCeiledOddNumber(float number)
    {
        int ceiledNumber = (int)Math.Ceiling(number);
        if (ceiledNumber % 2 == 0) // Check if the number is even
        {
            ceiledNumber++;        // Increment by 1 to make it odd
        }

        return ceiledNumber;
    }
}
