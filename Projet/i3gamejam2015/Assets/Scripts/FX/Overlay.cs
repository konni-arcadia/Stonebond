using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour {

    private static Overlay instance;

    private SpriteRenderer spriteRenderer;

    public float flashTime = 0.5f;
    public AnimationCurve flashCurve;

    private float pause = 0.0f;
    private float counter = 0.0f;

    private float r, g, b;

    private AnimationCurve actualCurve;

    // Use this for initialization
    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.enabled = false;

        instance = this;
    }

    void Update () {
        if (pause > 0.0f)
        {
            pause -= TimeManager.realDeltaTime;
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
                    return;
                }

                float pct = 1.0f - counter / flashTime;
                float alpha = actualCurve.Evaluate(pct);
                spriteRenderer.color = new Color(instance.r, instance.g, instance.b, alpha);
            }           
        }
    }

    public static void ShowFlash(float pause = 0.0f) {
        if (instance == null) {
            return;
        }

        Show (instance.flashTime, instance.flashCurve, 1.0f, 1.0f, 1.0f, false, pause);
    }

    //public static void ShowReverse2(float duration) {
    //    if (instance == null) {
    //        return;
    //    }

    //    Show (duration, instance.curve, 1.0f, 1.0f, 1.0f, false, 0.0f);
    //}

    private static void Show(float duration, AnimationCurve curve, float r, float g, float b, bool reverse, float pause) {
        instance.actualCurve = curve;
        instance.counter = duration;
        instance.r = r;
        instance.g = g;
        instance.b = b;
        instance.pause = pause;
        instance.spriteRenderer.enabled = true;
        instance.spriteRenderer.color = new Color (instance.r, instance.g, instance.b, reverse ? 0.0f : 1.0f);
    }
}
