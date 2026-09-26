using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}