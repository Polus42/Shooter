using UnityEngine;
using System.Collections;
using System;

public class LaserPattern : IPattern
{
    private OptionsHolder.LaserPatternOP options;
    private readonly SunBehavior sb;
    private GameObject[] players;
    private GameObject target;

    //private float counter = 0;
    private float counterTime = 0;
    private Transform animatedLaser;

    private bool targetLocked = false;

    //Options
    private float frequency = 5f;//waiting time between lasers
    private float duration = 5f;//per laser
    private float startTime = 0f;//override start time (default 0)
    private float rotationSpeed = 0f;
    private float toTargetSpeed = 0.7f;

    public LaserPattern(SunBehavior sb)
    {
        this.sb = sb;
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.LaserPatternOP)options;
        counterTime = startTime;
        animatedLaser = this.sb.gameObject.transform.GetChild(1);
        players = GameObject.FindGameObjectsWithTag("Player");
        target = selectTarget();
    }

    public void UpdatePattern()
    {
        /*
        Vector3 relativePos = target.transform.position - sb.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        sb.transform.rotation = rotation;
        */

        if(target != null)
        {
            Vector3 diff = target.transform.position - sb.transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            //sb.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            //sb.transform.rotation = Quaternion.Lerp(sb.transform.rotation, Quaternion.FromToRotation(sb.transform.position, target.transform.position - sb.transform.position), Time.deltaTime);
            sb.transform.rotation = Quaternion.Slerp(sb.transform.rotation, Quaternion.Euler(0f, 0f, rot_z + 90), Time.deltaTime * toTargetSpeed);
        }
        else
        {
            //EndPattern();
            players = GameObject.FindGameObjectsWithTag("Player");
            selectTarget();
        }
        

        counterTime += Time.deltaTime;
        if (counterTime >= frequency)
        {
            emitLaser();
            counterTime = 0;
        }
       
        //sb.transform.right = target.transform.position - sb.transform.position;
    }

    private GameObject selectTarget()
    {
        return players[UnityEngine.Random.Range(0, players.Length)];
    }

    void emitLaser()
    {
        animatedLaser.gameObject.SetActive(true);
        
        //animatedLaser.gameObject.SetActive(false);
    }

    public void EndPattern()
    {
        animatedLaser.gameObject.SetActive(false);
        sb.launchRotateInitial();
    }
}
