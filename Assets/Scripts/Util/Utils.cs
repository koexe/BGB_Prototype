using UnityEngine;

public static class Utils
{
    public static int FNVHash(string _str)
    {
        const uint fnvPrime = 16777619u;
        uint hash = 2166136261u;

        for (int i = 0; i < _str.Length; i++)
        {
            hash ^= _str[i];
            hash *= fnvPrime;
        }

        // Unity Random.InitState는 int만 받으니까 변환
        return (int)(hash & 0x7FFFFFFF);
    }
}
