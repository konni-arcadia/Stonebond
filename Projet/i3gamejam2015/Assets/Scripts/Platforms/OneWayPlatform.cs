using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour {

    public Collider2D myDisableCollisionTrigger;
    public Collider2D myCollisionCollider;

	// Use this for initialization
	void Start () {
	    if(myDisableCollisionTrigger == null || !myDisableCollisionTrigger.isTrigger)
        {
            Debug.LogError("Erreur, la one way platorm " + name + " n'a pas de trigger défini pour l'activation/désactivation ou le collider n'est pas un trigger");
        }

        if (myCollisionCollider == null || myCollisionCollider.isTrigger)
        {
            Debug.LogError("Erreur, la one way platorm " + name + " n'a pas de collider défini pour la collision ou le collider n'est pas un collider");
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //On rentre dans la one way platform par le bas ou le cote
    void OnTriggerEnter2D(Collider2D other)
    {
        //On devra certainement tester un tag ou un layer savoir si c'est un player qui nous a touché, un bullet ou autre
        if(other.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            Debug.Log(other.name + " entre dans le trigger de la plateforme, collision désactivées");
            Physics2D.IgnoreCollision(myCollisionCollider, other, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //On devra certainement tester un tag ou un layer savoir si c'est un player qui nous a touché, un bullet ou autre
        if (other.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            Debug.Log(other.name + " sors du trigger de la plateforme, collision réactivées");
            Physics2D.IgnoreCollision(myCollisionCollider, other, false);
        }
    }

}
