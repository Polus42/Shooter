using UnityEngine;
using System.Collections;

public class CloseZone : MonoBehaviour {
    private Transform _senpai; 
	// Use this for initialization
	void Start () {
        _senpai = GameObject.Find("Senpai").transform;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = _senpai.position;
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player1")
        GameObject.Find("Senpai").SendMessage("OnP1EnterCloseZone");

        if (other.gameObject.name == "Player2")
        GameObject.Find("Senpai").SendMessage("OnP2EnterCloseZone");
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player1")
            GameObject.Find("Senpai").SendMessage("OnP1ExitCloseZone");

        if (other.gameObject.name == "Player2")
            GameObject.Find("Senpai").SendMessage("OnP2ExitCloseZone");
    }
}
