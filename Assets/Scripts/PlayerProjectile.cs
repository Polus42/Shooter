using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public string launchedby;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Explode()
    {
        var exp = GetComponent<ParticleSystem>();
        //exp.Play();
        Destroy(gameObject, exp.duration);
    }
}
