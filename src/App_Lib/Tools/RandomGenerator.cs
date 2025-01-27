using System;
using System.Security.Cryptography;

namespace src.App_Lib.Tools;

public static class RandomGenerator
{
    /// <summary>
    /// Returns a random in between min and max boundaries.
    /// </summary>
    /// <param name="min">Min value is included</param>
    /// <param name="max">Max value is excluded</param>
    /// <returns>Secure-Random integer</returns>
    public static int Next(int min, int max)
    {
        if (min >= max) throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");

        long diff = (long)max - min;

        long upperBound = uint.MaxValue / diff * diff;

        uint ui;

        do
        {
            ui = GetRandomUInt();
        } while (ui >= upperBound);

        return (int)(min + (ui % diff));
    }

    private static uint GetRandomUInt()
    {
        var randomBytes = GenerateRandomBytes(sizeof(uint));
        return BitConverter.ToUInt32(randomBytes, 0);
    }

    private static byte[] GenerateRandomBytes(int bytesNumber)
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] buffer = new byte[bytesNumber];
        rng.GetBytes(buffer);
        return buffer;
    }
}
