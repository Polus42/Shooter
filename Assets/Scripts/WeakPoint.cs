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
	    if (_health<=0)
        {
            Destroy(gameObject);
        }
	}
    void ApplyDamage(int howmany)
    {
        _health -= howmany;
    }
}
