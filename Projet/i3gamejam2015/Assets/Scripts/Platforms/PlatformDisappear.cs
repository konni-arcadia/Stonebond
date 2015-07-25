using UnityEngine;
using System.Collections;

public class PlatformDisappear : MonoBehaviour {

    public EdgeCollider2D myEdgeCollider;

    public BoxCollider2D boxTrigger;

    public float waitBeforeFading = 2;
    public float waitBeforeAppearing = 2;


	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            Debug.Log(gameObject.name + " va disparaitre");
            StartCoroutine(WaitAndFade(waitBeforeFading));
        }
    }

    IEnumerator WaitAndFade(float waitTime)
    {
        //flashing the Sprites before disappearing
       /* yield return new WaitForSeconds(waitTime / 5);
        DisableChildrenSprites();
        yield return new WaitForSeconds(waitTime / 5);
        EnableChildrenSprites();
        yield return new WaitForSeconds(waitTime / 5);
        DisableChildrenSprites();
        yield return new WaitForSeconds(waitTime / 5);
        EnableChildrenSprites();*/
        yield return new WaitForSeconds(waitTime);

        //Unable the colliders
        myEdgeCollider.enabled = false;
        boxTrigger.enabled = false;
        DisableChildrenSprites();

        //Re-appear
        StartCoroutine(WaitAndAppear(waitBeforeAppearing));

    }

    IEnumerator WaitAndAppear(float waitTime)
    {
        Debug.Log("J'apprais dans " + waitTime);
        yield return new WaitForSeconds(waitTime);
        Debug.Log("J'apprais");
        //Unable the colliders
        myEdgeCollider.enabled = true;
        boxTrigger.enabled = true;
        EnableChildrenSprites();

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
