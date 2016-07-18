using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour {

    private static Overlay instance;

    private SpriteRenderer spriteRenderer;

    public float flashTime = 0.5f;
    public AnimationCurve flashCurve;

    private float delay = 0.0f;
    private float duration;
    private float counter = 0.0f;

    private Color color;
    private AnimationCurve curve;

    // Use this for initialization
    public void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.enabled = false;

        instance = this;
    }

    public void Update () {
        if (delay > 0.0f)
        {
            delay -= TimeManager.realDeltaTime;
        }
        else
        {
            if (counter > 0.0f)
            {
                counter -= TimeManager.realDeltaTime;
                if (counter <= 0.0f)
                {
                    spriteRenderer.enabled = false;
                    counter = 0.0f;
                }
                else
                {
                    UpdateSpriteColor();                    
                }
            }           
        }
    }

    private void UpdateSpriteColor()
    {
        float pct = 1.0f - counter / duration;
        float alpha = curve.Evaluate(pct);
        spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }

    public static void ShowFlash(float delay = 0.0f) {
        if (instance == null) {
            return;
        }

        Show (instance.flashTime, instance.flashCurve, Color.white, delay);
    }

    public static void Show(float duration, AnimationCurve curve, Color color, float delay = 0.0f)
    {
        if(instance == null)
        {
            return;
        }

        instance.duration = duration;
        instance.counter = duration;
        instance.curve = curve;        
        instance.color = color;
        instance.delay = delay;

        instance.spriteRenderer.enabled = true;
        instance.UpdateSpriteColor();
    }
}
