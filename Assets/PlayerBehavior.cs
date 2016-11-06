using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {
    public int startingHealth;
    private int _health;
    public int rotateSpeed;
    public int bulletForce;
    public GameObject[] projectile;
    // Use this for initialization
    void Start () {
        _health = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
        checkInput();
	}
    void goLeft()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), rotateSpeed * Time.deltaTime);
    }
    void goRight()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), -rotateSpeed * Time.deltaTime);
    }
    void shoot()
    {
        GameObject go = (GameObject)Object.Instantiate(projectile[0], transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(-transform.position.x*bulletForce,-transform.position.y * bulletForce));
    }
    void ApplyDamage(int amount)
    {
        _health -= amount;
    }
    void checkInput()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            // there's at least one joysticks

        }
        else
        {
            if (Input.GetKey("left"))
            {
                goLeft();
            }
            if (Input.GetKey("right"))
            {
                goRight();
            }
            if (Input.GetKey("space"))
            {
                shoot();
            }
        }
    }
}
