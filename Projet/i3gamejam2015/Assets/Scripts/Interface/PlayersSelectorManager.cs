using UnityEngine;
using System.Collections;

public class PlayersSelectorManager : MonoBehaviour {

    private PlayerSelectorManager[] listPlayerSelector;
    public int minPlayerNeeded = 3;

	// Use this for initialization
	void Start () {

        listPlayerSelector = GetComponentsInChildren<PlayerSelectorManager>();
	
	}
	
	// Update is called once per frame
	void Update () {

        int nbReady = 0;
        bool canStart = true;
        for (int i = 0; i < listPlayerSelector.Length; i++)
        {
            if (listPlayerSelector[i].HasPressedStart)
            {
                if (listPlayerSelector[i].HasChoosen)
                {
                    nbReady++;
                }
                else
                {
                    canStart = false;
                }
            }

        }

        if (canStart && (nbReady >= minPlayerNeeded))
        {
            Application.LoadLevelAdditiveAsync("SelectLvl");
            Destroy(gameObject);
        }
	
	}
}
