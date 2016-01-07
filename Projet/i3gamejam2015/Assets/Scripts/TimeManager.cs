using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public float timeScale = 1.0f;

    public delegate void BeginCallback();
    public delegate void EndCallback();
    
    private class PauseEvent
    {
        private TimeManager manager;
        private float counter;
        private BeginCallback beginCallback;
        private EndCallback endCallback;

        public PauseEvent(TimeManager manager, float duration, BeginCallback beginCallback, EndCallback endCallback)
        {
            this.manager = manager;
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
                Time.timeScale = manager.timeScale;
                if(endCallback != null)
                {
                    endCallback();
                }
                return false;
            }

            return true;
        }
    }

    private static TimeManager instance = null;
   
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
        Time.timeScale = timeScale;
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

        instance.pauses.Add(new PauseEvent(instance, duration, beginCallback, endCallback));
    }
}
