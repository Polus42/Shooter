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
    //private float frequency = 5f;//waiting time between lasers
    //private float startTime = 0f;//override start time (default 0)
    //private float rotationSpeed = 0.7f;
    //Useless
    //private float duration = 5f;//per laser


    CameraShake cameraShake;

    public LaserPattern(SunBehavior sb)
    {
        this.sb = sb;
    }

    public void setOptions(OptionsHolder.IOptionPattern options)
    {
        this.options = (OptionsHolder.LaserPatternOP)options;
        counterTime = this.options.startTime;
        animatedLaser = this.sb.gameObject.transform.GetChild(1);
        targetLocked = false;
        counterTime = 0;
        players = GameObject.FindGameObjectsWithTag("Player");
        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        target = selectTarget();

        cameraShake.Shaking(Math.Abs(this.options.frequency - this.options.startTime), 1);
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
            sb.transform.rotation = Quaternion.Slerp(sb.transform.rotation, Quaternion.Euler(0f, 0f, rot_z + 90), Time.deltaTime * this.options.rotationSpeed);
        }
        else
        {
            //EndPattern();
            players = GameObject.FindGameObjectsWithTag("Player");
            selectTarget();
        }
        
        if(!targetLocked)
        {
            counterTime += Time.deltaTime;
            if (counterTime >= this.options.frequency)
            {
                emitLaser();
                counterTime = 0;
                targetLocked = true;
            }
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
