using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour {
    private int _health;
    public int startinghealth;
	// Use this for initialization
	void Start () {
        _health = startinghealth;
	}
	
	// Update is called once per frame
	void Update () {

	}
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            _health--;
            if (_health<=0)
            {
                if (coll.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP1DestroyWeakpoint");
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP2DestroyWeakpoint");
                }
                Destroy(gameObject);
            }
        }
        Destroy(coll.gameObject);
    }
}
