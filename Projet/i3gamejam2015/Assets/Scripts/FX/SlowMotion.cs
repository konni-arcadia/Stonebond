using UnityEngine;
using System.Collections;

public class SlowMotion : MonoBehaviour
{
    public delegate void BeginCallback();
    public delegate void EndCallback();

    //
    // PRIVATE CLASS
    //
    
    private class PauseEvent
    {
        float counter;
        BeginCallback beginCallback;
        EndCallback endCallback;

        public PauseEvent(float duration, BeginCallback beginCallback, EndCallback endCallback)
        {
            counter = duration;
            this.beginCallback = beginCallback;
            this.endCallback = endCallback;
        }

        public void Begin()
        {
            if (beginCallback != null)
            {
                beginCallback();
            }

            Time.timeScale = 0.0f;
        }

        public bool Update(float delta)
        {
            counter -= delta;
            if (counter <= 0.0f)
            {
                Time.timeScale = 1.0f;
                if(endCallback != null)
                {
                    endCallback();
                }
                return false;
            }

            return true;
        }
    }

    private static SlowMotion instance = null;
   
    private float lastFrameTime = 0.0f;

    private ArrayList pauses = new ArrayList();
    private PauseEvent currentPause = null; 
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        lastFrameTime = Time.realtimeSinceStartup;
    }
    
    void Update()
    {
        float now = Time.realtimeSinceStartup;
        float delta = now - lastFrameTime;
        lastFrameTime = now;

        if (currentPause != null)
        {
            if(!currentPause.Update(delta))
            {
                currentPause = null;
            }
        }

        if (currentPause == null && pauses.Count > 0)
        {
            currentPause = (PauseEvent) pauses[0];
            pauses.RemoveAt(0);

            currentPause.Begin();
        }
    }

    public static void Pause(float duration, BeginCallback beginCallback, EndCallback endCallback)
    {
        if (instance == null)
        {
            return;
        }

        instance.pauses.Add(new PauseEvent(duration, beginCallback, endCallback));
    }
}
