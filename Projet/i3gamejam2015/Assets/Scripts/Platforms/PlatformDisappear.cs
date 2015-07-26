using UnityEngine;
using System.Collections;

public class PlatformDisappear : MonoBehaviour {

    public BoxCollider2D boxColliderToUnable;
    public BoxCollider2D boxColliderToUnable2;

    public float timeBeforeFading = 8;
    public float timeBeforeAppearing = 8;

    public bool isActiveAtStart = true;

    void Awake()
    {
        if (isActiveAtStart)
        {
            StartCoroutine(WaitAndFade(timeBeforeFading));
        }
        else
        {
            DisableChildrenSprites();
            boxColliderToUnable.enabled = false;
            boxColliderToUnable2.enabled = false;
            StartCoroutine(WaitAndAppear(timeBeforeAppearing));
        }
    }

	// Use this for initialization
	void Start () {

 
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}


    IEnumerator WaitAndFade(float waitTime)
    {
 
        yield return new WaitForSeconds(waitTime-2);

        for (int i = 0; i < 9; i++)
        {
            DisableChildrenSprites();
            yield return new WaitForSeconds(0.1f);
            EnableChildrenSprites();
            yield return new WaitForSeconds(0.1f);
        }

        DisableChildrenSprites();
        //Unable the colliders
        boxColliderToUnable.enabled = false;
        boxColliderToUnable2.enabled = false;

        //Re-appear
        StartCoroutine(WaitAndAppear(timeBeforeAppearing));

    }

    IEnumerator WaitAndAppear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Unable the colliders
        boxColliderToUnable.enabled = true;
        boxColliderToUnable2.enabled = true;
        EnableChildrenSprites();
        StartCoroutine(WaitAndFade(timeBeforeFading));

    }

    void DisableChildrenSprites()
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void EnableChildrenSprites()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}
