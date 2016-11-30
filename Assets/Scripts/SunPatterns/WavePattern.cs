using UnityEngine;
using System.Collections;
using System;

public class WavePattern : IPattern
{
    private OptionsHolder.WavePatternOP options;
    private float counter = 0;
    private readonly SunBehavior sb;
    private float speed_multiplier;
    private int numberLoop = 0;
    private float currentAngle;

    //Options
    /*
    public float frequency = 0.5f;
    public int angleAdd = 360;//180 is cool
    public float angle = 0;//start angle
    public float radius = 0.4f;
    public int count = 20;
    public float bulletspeed = 0.5f;
    public float multiplicatorSpeed = 1f;//fait varier les vitesses pour type proj 0 et type proj 1, 1.1f ou 10f => interessant, peut inverser avec bulletspeed
    private bool angleWillVariateExtern = true;
    private float angleVariationExtern = 2f;
    public bool angleWillVariateIntern = true;
    public bool chaosOn = false;
    public int forceAType = -1;//-1 no forcing, otherwise index
    */

    public WavePattern(SunBehavior sb)
    {
        this.sb = sb;
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.WavePatternOP) options;
        counter = numberLoop = 0;
        currentAngle = this.options.angle;
    }

    public void UpdatePattern()
    {
        counter += Time.deltaTime;
        if (counter >= options.frequency)
        {
            emitProjectile();
            counter = 0;
        }
    }

    void emitProjectile()
    {
        //Debug.Log("number proj in screen : " + GameObject.FindGameObjectsWithTag("Projectile").Length);
        for (int i = 0; i < options.count; i++)
        {
            Vector3 thispos = new Vector3(options.radius, options.radius, 0);
            thispos = Quaternion.Euler(0, 0, currentAngle) * thispos;
            speed_multiplier = options.bulletspeed * sb.force;
            speed_multiplier = (numberLoop % 2 == 0) ? speed_multiplier : options.multiplicatorSpeed * speed_multiplier;
            //GameObject go = (GameObject)GameObject.Instantiate((numberLoop % 2 == 0) ? sb.typeProjectiles[0] : sb.typeProjectiles[1], sb.transform.position + thispos, Quaternion.identity);
            GameObject proj;
            if (options.forceAType > -1)
                proj = sb.typeProjectiles[options.forceAType];
            else
                proj = (numberLoop % 2 == 0) ? sb.typeProjectiles[0] : sb.typeProjectiles[1];
            GameObject go = GamePool.GetNextObject(proj, sb.transform.position + thispos, Quaternion.identity);
            go.GetComponent<ProjectileBehavior>().launchedby = "sun";
            go.GetComponent<Rigidbody2D>().AddForce(thispos * speed_multiplier);
            //go.GetComponent<Rigidbody2D>().velocity = thispos * speed_multiplier;

            if (options.chaosOn)
                currentAngle += (float)options.angleAdd / options.count * i;
            else if (options.angleWillVariateIntern)
                currentAngle += (float)options.angleAdd / options.count;
        }

        if (options.angleWillVariateExtern)
            currentAngle += (float)options.angleAdd / (options.count * options.angleVariationExtern);

        numberLoop++;
    }

    public void EndPattern()
    {

    }
}
