using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {
    public int startingHealth;
    private int _health;
    public int rotateSpeed;
    public int bulletForce;
    public GameObject[] projectile;
    public string playerPrefix;
    private Transform _cursor;
    // Use this for initialization
    void Start () {
        // assigning cursor
        _cursor =   GameObject.Find("Cursor_"+playerPrefix).transform;
        _health = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
        checkInput();
        drawLives();
	}
    void drawLives()
    {
        for (int i = 0;i< _health; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = Mathf.Max(0, _health) ; i < startingHealth; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    void move(float direction)
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), direction* rotateSpeed * Time.deltaTime);
    }
    void shoot()
    {
        GameObject go = (GameObject)Object.Instantiate(projectile[0], transform.position, Quaternion.identity);
        go.GetComponent<ProjectileBehavior>().launchedby = playerPrefix;
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(_cursor.position.x-transform.position.x, _cursor.position.y - transform.position.y).normalized*bulletForce);
    }
    void ApplyDamage(int amount)
    {
        _health -= amount;
    }
    void checkInput()
    {
        if (Input.GetAxis(playerPrefix + "_horizontal_1")!=0)
        {
            move(Input.GetAxis(playerPrefix + "_horizontal_1"));
        }
            if (Input.GetButton(playerPrefix + "_fire"))
            {
                shoot();
            }
            if (Input.GetAxis(playerPrefix + "_horizontal_2") != 0)
            {
                 _cursor.RotateAround(Vector3.zero, new Vector3(0, 0, 1), Input.GetAxis(playerPrefix + "_horizontal_2") *rotateSpeed * Time.deltaTime);
            }
    }
}
