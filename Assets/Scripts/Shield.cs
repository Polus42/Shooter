using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    private AudioSource _audiosource;
	// Use this for initialization
	void Start () {
        _audiosource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    void OnCollisionEnter2D(Collision2D coll)
    {
        _audiosource.Play();
    }
}
