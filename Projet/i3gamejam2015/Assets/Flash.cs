using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour {

    private static Flash instance;

    private SpriteRenderer spriteRenderer;

    public float flashTime = 0.5f;
    public AnimationCurve curve;

    private float pause = 0.0f;
    private float counter = 0.0f;
//  private float duration = 0.0f;
    private bool reverse = false;

    private float r, g, b;

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
            }

            float pct = 1.0f - counter / flashTime;
            if(reverse)
            {
                pct = 1.0f - pct;
            }

            float alpha = curve.Evaluate(pct);
            spriteRenderer.color = new Color(instance.r, instance.g, instance.b, alpha);
        }
    }

    public static void Show()
    {
        if (instance == null) {
            return;
        }

        Show(0.0f);
    }

    public static void Show(float pause) {
        if (instance == null) {
            return;
        }

        Show (instance.flashTime, 1.0f, 1.0f, 1.0f, false, pause);
    }

    public static void ShowReverse(float duration) {
        if (instance == null) {
            return;
        }

        Show (duration, 1.0f, 1.0f, 1.0f, false, 0.0f);
    }

    private static void Show(float duration, float r, float g, float b, bool reverse, float pause) {
        instance.counter = duration;
//      instance.duration = duration;
        instance.r = r;
        instance.g = g;
        instance.b = b;
        instance.reverse = reverse;
        instance.pause = pause;
        instance.spriteRenderer.enabled = true;
        instance.spriteRenderer.color = new Color (instance.r, instance.g, instance.b, reverse ? 0.0f : 1.0f);
    }
}
