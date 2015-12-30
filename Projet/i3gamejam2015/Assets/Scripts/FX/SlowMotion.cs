using UnityEngine;
using System.Collections;

public class SlowMotion : MonoBehaviour
{
    
    private static SlowMotion instance = null;
    public float slowMotionTime = 0.15f;
    private float slowMotionCounter = 0.0f;
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
    }
    
    void Update()
    {
        if (slowMotionCounter > 0.0f)
        {
            slowMotionCounter -= Time.deltaTime;
            if (slowMotionCounter <= 0.0f)
            {
                slowMotionCounter = 0.0f;
                Time.timeScale = 1.0f;
            }
            else
            {
                float pct = 1.0f - slowMotionCounter / slowMotionTime;
                Time.timeScale = 0.5f + pct * 0.5f;
            }
        }
    }
    
    public static void StartSlowMotion()
    {
        instance.slowMotionCounter = instance.slowMotionTime;
    }
}
