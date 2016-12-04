using UnityEngine;
using System.Collections;

public class AsteroidBehavior : MonoBehaviour {
    private OptionsHolder.AsteroidOP asteroidOP;
    private Renderer red;
    private GameObject target;
    private Rigidbody2D rb;
    private bool wasSeen = false;
    private float gravityFactor = 1f;
    private float fakeMass = 50f;
    private bool attracted = false;

    void Start()
    {
        GetComponents<AudioSource>()[0].Play();
        OptionsManager.Instance.getAsteroidOptions(out asteroidOP);
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(-1f, 1f) * Random.Range(asteroidOP.rotateMin, asteroidOP.rotateMax);
        target = GameObject.Find("Sun");

        //float leftOrRight = target.transform.position.x - transform.position.x;
        //leftOrRight = Mathf.Clamp(leftOrRight, -1f, 1f);
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = direction.y / 2;
        rb.AddForce(direction * asteroidOP.force);


        //rb.velocity = direction * 2;// * asteroidOP.force;
        red = GetComponent<Renderer>();
        //StartCoroutine(attract());
    }

    void Update()
    {
        if (red.isVisible)
        {
            wasSeen = true;
        }
        else if (wasSeen && !red.isVisible)
        {
            Debug.Log("asteroid out");
            Destroy(gameObject);
        }
        if (transform.position.x < 0.5f && transform.position.x > -0.5f && transform.position.y < 0.5f && transform.position.y > -0.5f)//inside sun
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("touched attraction");
        if (other.gameObject.name == "AttractionField")
        {
            attracted = true;
        }
        else if (other.gameObject.GetComponent<PlayerBehavior>() != null)
        {
            other.gameObject.SendMessage("ApplyDamage", 1);
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if(!attracted)
            return;
        
        Vector2 to = (target.transform.position - transform.position).normalized * fakeMass * gravityFactor / (target.transform.position - transform.position).sqrMagnitude;
        //Debug.Log("toto: " + to.x + " " + to.y);
        rb.AddForce(to);
        
    }
    /*
    float timeAlive = 0;

    IEnumerator attract()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            timeAlive += Time.deltaTime;
            Vector3 vec = (target.transform.position - transform.position);
            float force = -Mathf.Log10(timeAlive) * asteroidOP.force;
            //float force = (vec.magnitude > 0) ? asteroidOP.force / vec.magnitude : asteroidOP.force;
            Debug.Log("force applied to asteroid: " + force);
            rb.AddForce(vec.normalized * force);//de plus en plus attiré
            //rb.velocity = vec.normalized * force;
        }
    }
    */
}
