using UnityEngine;
using System.Collections;

public class UniversalGravity : MonoBehaviour {
    // Use this for initialization
    void Start()
    {
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("Player");

        //the gravity between each couple of object is calculated
        foreach (GameObject ObjectA in Objects)
        {
            ObjectA.GetComponent<Rigidbody2D>().AddForce(new Vector3(30, 0, 0));
        }
    }
    void ApplyGravity(Rigidbody2D A, Rigidbody2D B)
    {
        //This is how to get the distance vector between two objects.
        Vector3 dist = B.transform.position - A.transform.position;
        float r = dist.magnitude;
        dist /= r;

        //This is the Newton's equation
        //G = 6.67 * 10^-11 N.m².kg^-2
        double G = 6.674f * (10 ^ 11);
        float force = ((float)G * A.mass * B.mass) / (r * r);

        //Then, just apply the forces
        A.AddForce(dist * force);
        B.AddForce(-dist * force);
    }
    void FixedUpdate()
    {
        //Get every object 
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("Player");

        //the gravity between each couple of object is calculated
        foreach (GameObject ObjectA in Objects)
        {
            foreach (GameObject ObjectB in Objects)
            {
                //Objects must not self interact 
                if (ObjectA == ObjectB)
                    continue;

                ApplyGravity(ObjectA.GetComponent<Rigidbody2D>(), ObjectB.GetComponent<Rigidbody2D>());
            }

        }
    }
}
