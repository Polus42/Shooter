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
        if (coll.gameObject.GetComponent<ProjectileBehavior>()!=null)
        {
            Destroy(coll.gameObject);
            _audiosource.Play();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<ProjectileBehavior>() != null)
        {
            Destroy(other.gameObject);
            _audiosource.Play();
        }
    }
}
