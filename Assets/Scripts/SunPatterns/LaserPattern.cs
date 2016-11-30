using UnityEngine;
using System.Collections;
using System;

public class LaserPattern : IPattern
{
    private OptionsHolder.CyclicPatternOP options;
    private readonly SunBehavior sb;
    private GameObject[] players;
    private GameObject target;
    private float counter = 0;

    //Options
    private float frequency = 0.3f;
    private float mult = 1;
    private float radius = 0.4f;
    private int count = 10;
    private float bulletspeed = 1.5f;
    private float angleVariation = 2f;

    public LaserPattern(SunBehavior sb)
    {
        this.sb = sb;
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.CyclicPatternOP)options;
        counter = 0;
        players = GameObject.FindGameObjectsWithTag("Player");
        target = selectTarget();
    }

    public void UpdatePattern()
    {
        /*
        counter += Time.deltaTime;
        if (counter >= frequency)
        {
            emitProjectile();
            counter = 0;
        }
        */

        Vector3 relativePos = target.transform.position - sb.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        sb.transform.rotation = rotation;
        //sb.transform.right = target.transform.position - sb.transform.position;
    }

    private GameObject selectTarget()
    {
        return players[UnityEngine.Random.Range(0, players.Length +1)];
    }

    void emitProjectile()
    {

    }

    public void EndPattern()
    {

    }
}
