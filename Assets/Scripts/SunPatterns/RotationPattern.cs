using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RotationPattern : IPattern
{
    private OptionsHolder.RotationPatternOP options;
    private readonly SunBehavior sb;
    private float counter = 0;
    private float counterDirection = 0;
    private bool haveChangedDirection = false;
    private List<GameObject> bullets;
    private bool addForce = false;
    private float currentAngle;
    private int count = 1;

    //Options
    /*
    public float frequency = 0.3f;//frequency create helixes
    private float angle = 90;//Start angle
    private float radius = 0.4f;//influence separation between the projectiles
    private float bulletSpeed = 1.5f;
    private float rotatingSpeed = 20f;
    private float changeDirection = 3f;//change direction after this time
    private bool repeatChangeDirection = true;
    private float maxRadius = 5f;//Size of helix
    private int numberHelixes = 4;
    */

    public RotationPattern(SunBehavior sb)
    {
        this.sb = sb;
        bullets = new List<GameObject>();
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.RotationPatternOP) options;
        counter = 0;
        counterDirection = 0;
        count = 1;
        haveChangedDirection = false;
        addForce = false;
        currentAngle = this.options.angle;
    }

    public void UpdatePattern()
    {
        if(!addForce)
        {
            counter += Time.deltaTime;
            if (counter >= options.frequency)
            {
                //Debug.Log("emit rotation projectile");
                emitProjectile();
                counter = 0;
            }
        }
        else
        {
            updateBullets();
        }
    }

    void updateBullets()
    {
        if(!haveChangedDirection || options.repeatChangeDirection)
        {
            counterDirection += Time.deltaTime;
            if (counterDirection >= options.changeDirection)
            {
                options.rotatingSpeed = -options.rotatingSpeed;
                counterDirection = 0;
                haveChangedDirection = true;
            }
        }

        /*
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            GameObject go = bullets[i];
            if (go == null || !go.activeSelf)
            {
                bullets.RemoveAt(i);
                continue;
            }
            go.transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), options.rotatingSpeed * Time.deltaTime);
        }
        */
       
        foreach (GameObject go in bullets)
        {
            if (go == null || !go.activeSelf)
            {
                //bullets.Remove(go);
                continue;
            }
            go.transform.RotateAround(Vector3.zero, new Vector3(0,0,1), options.rotatingSpeed * Time.deltaTime);
        }
    }

    public void EndPattern()
    {
        foreach (GameObject go in bullets)
        {
            if (go == null || !go.activeSelf)
            {
                //bullets.Remove(go);
                continue;
            }
            go.GetComponent<Rigidbody2D>().isKinematic = false;
            go.GetComponent<Rigidbody2D>().AddForce((go.transform.position - sb.transform.position).normalized * options.bulletSpeed * sb.force);
            //go.GetComponent<Rigidbody2D>().AddForce(go.transform.forward * options.bulletSpeed * sb.force);

            //Cool    
            /*Vector3 test = (go.transform.position - sb.transform.position);
            test.x *= -1;
            test *= sb.force;
            go.GetComponent<Rigidbody2D>().AddForce(test);*/
        }
        bullets.Clear();
    }

    void emitProjectile()
    {
        GameObject lastBullet = null;
        for (int i = 0; i < options.numberHelixes; i++)
        {
            Vector3 thispos = new Vector3(count * options.radius, count * options.radius, 0);
            thispos = Quaternion.Euler(0, 0, currentAngle) * thispos;
            //lastBullet = GamePool.GetNextObject(sb.typeProjectiles[0], sb.transform.position + thispos, Quaternion.identity);
            //Debug.Log("is active? " + lastBullet.gameObject.activeSelf);
            lastBullet = (GameObject) GameObject.Instantiate(sb.typeProjectiles[1], sb.transform.position + thispos, Quaternion.identity);
            lastBullet.GetComponent<ProjectileBehavior>().launchedby = "sun";
            bullets.Add(lastBullet);
            currentAngle += (float) 360 / options.numberHelixes;
        }
        count++;

        if((lastBullet.transform.position - sb.transform.position).magnitude > options.maxRadius)
        {
            addForce = true;
            foreach(GameObject go in bullets)
            {
                if(go == null || !go.activeSelf)
                {
                    //bullets.Remove(go);
                    continue;
                }
                go.GetComponent<Rigidbody2D>().isKinematic = true;
                //go.GetComponent<Rigidbody2D>().AddTorque(360, ForceMode2D.Impulse); //go.transform.position * speed_multiplier
            }
        }
    }
   
}
