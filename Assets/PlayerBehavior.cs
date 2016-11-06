using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {
    public int startingHealth;
    private int _health;
    public int rotateSpeed;
    public int bulletForce;
    public GameObject[] projectile;
    public string playerPrefix;
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
        go.GetComponent<ProjectileBehavior>().launchedby = playerPrefix;
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(-transform.position.x*bulletForce,-transform.position.y * bulletForce));
    }
    void ApplyDamage(int amount)
    {
        _health -= amount;
    }
    void checkInput()
    {
            if (Input.GetButton(playerPrefix+"_left"))
            {
                goLeft();
            }
            if (Input.GetButton(playerPrefix + "_right"))
            {
                goRight();
            }
            if (Input.GetButton(playerPrefix + "_fire"))
            {
                shoot();
            }
    }
}
