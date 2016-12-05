using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour {
    public string launchedby;
    private Renderer red;
    // Use this for initialization
    void Start () {
        red = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!red.isVisible)
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
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "Sun")
            Destroy(gameObject, Random.Range(0.2f,0.3f));
    }
}
