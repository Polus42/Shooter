using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour {
    public string launchedby;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	    
	}
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "Player")
        {
            coll.gameObject.SendMessage("ApplyDamage", 1);
            Explode();
        }
        if (coll.collider.tag == "WeakPoint")
        {
            coll.gameObject.SendMessage("ApplyDamage", 1);
            Explode();
        }
        if (coll.collider.name == "Shield")
        {
            Destroy(gameObject);
        }
    }
    void Explode()
    {
        var exp = GetComponent<ParticleSystem>();
        //exp.Play();
        Destroy(gameObject, exp.duration);
    }
}
