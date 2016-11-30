using UnityEngine;
using System.Collections;

public class OpposedZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player1")
        {
            GameObject.Find("Senpai").SendMessage("OnP1EnterOpposed");
        }
        if (coll.gameObject.name == "Player2")
        {
            GameObject.Find("Senpai").SendMessage("OnP2EnterOpposed");
        }
    }
}
