﻿using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {
    // Public attributes
    public float shootingFrequency;
    public int startingHealth;
    public int initialSpeed;
    public int bulletForce;
    public GameObject[] projectile;
    public string playerPrefix;
    public float boostTime;
    public float boostReloadTime;
    public int speedBonus;
    public float shieldTime;
    public float shieldReloadTime;
    public bool autoMove;
    public int _PF = 0;
    public bool viseeAuto = false;
    // Private attributes
    private int _health;
    private Transform _cursor_center;
    private Transform _cursor;
    private float _boostTime;
    private int _rotateSpeed;
    private float _shieldTime;
    // Use this for initialization
    void Start () {
        // assigning cursor
        _cursor = GameObject.Find("Cursor_" + playerPrefix).transform;
        _cursor_center =   GameObject.Find("Cursor_"+playerPrefix+"_anchor").transform;
        _health = startingHealth;
        // Init private properties
        _boostTime = 0;
        _rotateSpeed = initialSpeed;
        _shieldTime = 0;
        transform.GetChild(3).gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        _boostTime = Mathf.Max(_boostTime-Time.deltaTime,0);
        _shieldTime = Mathf.Max(_shieldTime - Time.deltaTime, 0);
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
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), direction* _rotateSpeed * Time.deltaTime);
    }
    void shoot()
    {
        GetComponent<AudioSource>().Play();
        GameObject go = (GameObject)Object.Instantiate(projectile[0], transform.position, Quaternion.identity);
        go.GetComponent<ProjectileBehavior>().launchedby = playerPrefix;
        if (playerPrefix == "P1")
        {
            go.layer = 8;
        }
        else
        {
            go.layer = 9;
        }
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(_cursor.position.x-transform.position.x, _cursor.position.y- transform.position.y).normalized*bulletForce);
    }
    void ApplyDamage(int amount)
    {
        _health -= amount;
        if (_health<=0)
        {
            Destroy(gameObject);
        }
    }
    // Actions spéciales ///////////////////////////////////////
    // Boost
    void boost()
    {
        if (_boostTime==0)
        {
            _rotateSpeed += speedBonus;
            StartCoroutine(initSpeed());
           _boostTime = boostReloadTime;
        }
    }
    IEnumerator initSpeed()
    {
        yield return new WaitForSeconds(boostTime);
        _rotateSpeed = initialSpeed;
    }
    // Shield
    void shield()
    {
        if (_shieldTime==0)
        {
            transform.GetChild(3).gameObject.SetActive(true);
            StartCoroutine(initShield());
            _shieldTime = shieldReloadTime;
        }
    }
    IEnumerator initShield()
    {
        yield return new WaitForSeconds(shieldTime);
        transform.GetChild(3).gameObject.SetActive(false);
    }
    void checkInput()
    {
        // Movement /////////////////////////////////////////
        if (autoMove)
        {
            move(1);
            if (Input.GetButtonDown(playerPrefix+"_changedirection"))
            {
                _rotateSpeed = -_rotateSpeed;
            }
        }
        else
        {
            if (Input.GetAxis(playerPrefix + "_horizontal_1") != 0)
            {
                move(Input.GetAxis(playerPrefix + "_horizontal_1"));
            }
        }
        // Fireing /////////////////////////////////////////
        if (Input.GetButtonDown(playerPrefix + "_fire") && !IsInvoking("shoot"))
            {
            InvokeRepeating("shoot", 0f, shootingFrequency);
            }
            else if (!Input.GetButton(playerPrefix + "_fire"))
            {
            CancelInvoke("shoot");
            }
            if (Input.GetAxis(playerPrefix + "_horizontal_2") != 0 || Input.GetAxis(playerPrefix + "_vertical_2") != 0)
            {
            float x = Input.GetAxis(playerPrefix + "_horizontal_2");
            float y = Input.GetAxis(playerPrefix + "_vertical_2");
            _cursor_center.transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -Mathf.Atan2(x, y) * Mathf.Rad2Deg);
        }
            else
        {
            _cursor_center.transform.rotation = transform.localRotation;
        }
        // Boost /////////////////////////////////////////
        if (Input.GetButtonDown(playerPrefix+"_boost"))
        {
            boost();
        }
        // Shield /////////////////////////////////////////
        if (Input.GetButtonDown(playerPrefix + "_shield"))
        {
            shield();
        }
        // Reload scene /////////////////////////////////////////
        if (Input.GetButtonDown("Cancel"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}