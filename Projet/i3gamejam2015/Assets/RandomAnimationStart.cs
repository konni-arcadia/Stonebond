using UnityEngine;
using System.Collections;

public class RandomAnimationStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(RandomizeApparition(0.1f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator RandomizeApparition(float time)
    {
        GetComponent<Animator>().speed = Random.Range(0.0f, 2000.0f);
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().speed = 1;
    }
}
