using UnityEngine;
using System.Collections;
using System;

public class CyclicPattern : IPattern
{
    private OptionsHolder.CyclicPatternOP options;
    private readonly SunBehavior sb;
    private float counter = 0;
    private float speed_multiplier;
    private float currentAngle;

    //Options
    /*
    private float angle = 1; //Start angle
    private float frequency = 0.3f;
    private float mult = 1;
    private float radius = 0.4f;
    private int count = 10;
    private float bulletspeed = 1.5f;
    private float angleVariation = 2f;
    */

    public CyclicPattern(SunBehavior sb)
    {
        this.sb = sb;
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.CyclicPatternOP) options;
        counter = 0;

        speed_multiplier = options.bulletspeed * sb.force;
        currentAngle = this.options.angle;
    }

    public void UpdatePattern()
    {
        counter += Time.deltaTime;
        if(counter >= options.frequency)
        {
            emitProjectile();
            counter = 0;
        }
    }

    void emitProjectile()
    {
        //Debug.Log("number proj in screen : " + GameObject.FindGameObjectsWithTag("Projectile").Length);
        //Debug.Log("pos of sun : " + sb.transform.position.x + " " + sb.transform.position.y + " " + sb.transform.position.z);
        for (int i = 0; i < options.count; i++)
        {
            Vector3 thispos = new Vector3(options.radius * (float) Math.Sin(currentAngle), options.radius * (float) Math.Cos(currentAngle) * options.mult, 0);
            //GameObject go = GamePool.GetNextObject(sb.typeProjectiles[0], sb.transform.position + thispos, Quaternion.identity);
            GameObject go = (GameObject)GameObject.Instantiate(sb.typeProjectiles[0], sb.transform.position + thispos, Quaternion.identity);
            go.GetComponent<ProjectileBehavior>().launchedby = "sun";
            go.GetComponent<Rigidbody2D>().AddForce(thispos * speed_multiplier);
            currentAngle += (2 * (float) Math.PI) / options.count;
        }
        currentAngle += options.angleVariation;
        //mult = 3 * (float) Math.Sin(counter);
    }

    public void EndPattern()
    {
        
    }
}
