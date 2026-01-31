using UnityEngine;

public class GameTimer : MonoBehaviour
{

    public float gameDuration = 30f;
    public static bool TimeRemained;

    void Awake()
    {
        TimeRemained = true;
    }

    void Update()
    {
    }
}
