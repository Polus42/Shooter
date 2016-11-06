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
            Debug.Log("Collission with : " + coll.collider.name + " launched by " + launchedby);
            coll.gameObject.SendMessage("ApplyDamage", 10);
            Explode();
        }
    }
    void Explode()
    {
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.duration);
    }
}
