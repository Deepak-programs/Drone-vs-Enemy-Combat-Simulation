using UnityEngine;

public class PlayerDetecter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameEvent.RaisePlayerDetected(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            GameEvent.RaisePlayerDetected(false);
        }
    }
}

