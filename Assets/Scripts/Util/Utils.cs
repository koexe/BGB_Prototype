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
public static class LogUtil
{
    public static void Log(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(_message);
#endif
    }

    public static void LogWarning(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning(_message);
#endif
    }

    public static void LogError(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError(_message);
#endif
    }
}