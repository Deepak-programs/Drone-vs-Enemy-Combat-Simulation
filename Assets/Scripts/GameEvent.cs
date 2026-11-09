using System;

public class GameEvent
{
    public static event Action<bool> PlayerDetected;

    public static void RaisePlayerDetected(bool dectected)
    {
        PlayerDetected?.Invoke(dectected);
    }
}

