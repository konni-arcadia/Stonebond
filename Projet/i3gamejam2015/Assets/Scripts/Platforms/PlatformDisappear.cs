﻿using UnityEngine;
using System.Collections;

public class PlatformDisappear : MonoBehaviour {

    public BoxCollider2D boxColliderToUnable;
    public BoxCollider2D boxColliderToUnable2;

    public float waitBeforeFading = 8;
    public float waitBeforeAppearing = 8;

    public bool isActiveAtStart = true;


	// Use this for initialization
	void Start () {

        if (isActiveAtStart)
        {
            StartCoroutine(WaitAndFade(waitBeforeFading));
        }
        else
        {
            StartCoroutine(WaitAndFade(0));
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            StartCoroutine(WaitAndFade(waitBeforeFading));
        }
    }

    IEnumerator WaitAndFade(float waitTime)
    {
 
        yield return new WaitForSeconds(waitTime);

        //Unable the colliders
        boxColliderToUnable.enabled = false;
        boxColliderToUnable2.enabled = false;
      //  boxTrigger.enabled = false;
        DisableChildrenSprites();

        //Re-appear
        StartCoroutine(WaitAndAppear(waitBeforeAppearing));

    }

    IEnumerator WaitAndAppear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Unable the colliders
        boxColliderToUnable.enabled = true;
        boxColliderToUnable2.enabled = true;
       // boxTrigger.enabled = true;
        EnableChildrenSprites();
        StartCoroutine(WaitAndFade(waitBeforeFading));

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
