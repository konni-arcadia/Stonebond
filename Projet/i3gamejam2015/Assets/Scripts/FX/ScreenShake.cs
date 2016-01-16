using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{

    //
    // PARAMETERS
    //

    public float scaler = 1.0f;
    public float maxX = float.MaxValue;
    public float maxY = float.MaxValue;
    public float maxZ = float.MaxValue;

    //
    // PRIVATE CLASS
    //

    private class Shaker
    {
        private float strenghtX;
        private float decayX;
        private float strenghtY;
        private float decayY;
        private float strenghtZ;
        private float decayZ;

        public Shaker(float strenghtX, float decayX, float strenghtY, float decayY, float strenghtZ, float decayZ)
        {
            this.strenghtX = strenghtX;
            this.decayX = decayX;

            this.strenghtY = strenghtY;
            this.decayY = decayY;

            this.strenghtZ = strenghtZ;
            this.decayZ = decayZ;
        }

        public bool update(ref Vector3 position)
        {           
            if (decayX > 0.0f && strenghtX > 0.0f)
            {
                strenghtX -= decayX * Time.deltaTime;
                if (strenghtX < 0.0f)
                {
                    strenghtX = 0.0f;
                }
            }
            
            if (decayY > 0.0f && strenghtY > 0.0f)
            {
                strenghtY -= decayY * Time.deltaTime;
                if (strenghtY < 0.0f)
                {
                    strenghtY = 0.0f;
                }
            }
            
            if (decayZ > 0.0f && strenghtZ > 0.0f)
            {
                strenghtZ -= decayZ * Time.deltaTime;
                if (strenghtZ < 0.0f)
                {
                    strenghtZ = 0.0f;
                }
            }
                        
            if (strenghtX == 0.0f && strenghtY == 0.0f && strenghtZ == 0.0f)
            {
                return false;   
            } 

            if (strenghtX > 0.0f)
            {
                position.x += Random.Range(-strenghtX, strenghtX);
            }
                
            if (strenghtY > 0.0f)
            {
                position.y += Random.Range(-strenghtY, strenghtY);
            }
                
            if (strenghtZ > 0.0f)
            {
                position.z += Random.Range(-strenghtZ, strenghtZ);
            }

            return true;
        }
    }

    //
    // MAIN CLASS
    //

    private static ScreenShake instance = null;
    private ArrayList shakers = new ArrayList();
    private float originX;
    private float originY;
    private float originZ;
    private Vector3 shakeOffset = new Vector3();
    private bool init = true;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (init)
        {
            originX = transform.localPosition.x;
            originY = transform.localPosition.y;
            originZ = transform.localPosition.z;
            init = false;
        }

        shakeOffset.Set(0.0f, 0.0f, 0.0f);

        if(enabled && !TimeManager.isPaused)
        {
            for (int i = shakers.Count - 1; i >= 0; i--)
            {
                Shaker shaker = (Shaker)shakers [i];
                if (!shaker.update(ref shakeOffset))
                    shakers.RemoveAt(i);
            }
      
            if (Mathf.Abs(shakeOffset.x) > maxX)
            {
                shakeOffset.x = Mathf.Sign(shakeOffset.x) * maxX;
            }

            if (Mathf.Abs(shakeOffset.y) > maxY)
            {
                shakeOffset.y = Mathf.Sign(shakeOffset.y) * maxY;
            }

            if (Mathf.Abs(shakeOffset.z) > maxZ)
            {
                shakeOffset.z = Mathf.Sign(shakeOffset.z) * maxZ;
            }
        }

        shakeOffset.x += originX;
        shakeOffset.y += originY;
        shakeOffset.z += originZ;
        transform.localPosition = shakeOffset;
    }

    //
    // PUBLIC METHODS
    //

    public static void ShakeX(float strenghtX, float decayX)
    {
        if (instance == null)
        {
            return;
        }
        
        Shaker shaker = new Shaker(strenghtX, decayX, 0.0f, 0.0f, 0.0f, 0.0f);
        instance.shakers.Add(shaker);
    }

    public static void ShakeY(float strenghtY, float decayY)
    {
        if (instance == null)
        {
            return;
        }
        
        Shaker shaker = new Shaker(0.0f, 0.0f, strenghtY, decayY, 0.0f, 0.0f);
        instance.shakers.Add(shaker);
    }

    public static void ShakeZ(float strenghtZ, float decayZ)
    {
        if (instance == null)
        {
            return;
        }
        
        Shaker shaker = new Shaker(0.0f, 0.0f, 0.0f, 0.0f, strenghtZ, decayZ);
        instance.shakers.Add(shaker);
    }

    public static void ShakeXY(float strenghtX, float decayX, float strenghtY, float decayY)
    {
        if (instance == null)
        {
            return;
        }

        Shaker shaker = new Shaker(strenghtX, decayX, strenghtY, decayY, 0.0f, 0.0f);
        instance.shakers.Add(shaker);
    }

    public static void SetEnabled(bool enabled)
    {
        if (instance == null)
        {
            return;
        }

        instance.enabled = enabled;
    }
}
