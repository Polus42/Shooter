using UnityEngine;
using System.Collections;
using System;
/*
public class Test5 : MonoBehaviour, IPattern
{
    public float frequency = 0.3f;
    private float counter = 0;

    private readonly SunBehavior sb;

    private float angle = 90;
    private float radius = 0.4f;
    private float speed_multiplier;
    private int count = 1;
    private float bulletspeed = 1.5f;

    private ArrayList bullets;

    private bool addForce = false;

    public Test5(SunBehavior sb)
    {
        this.sb = sb;
        bullets = new ArrayList();
    }

    public void UpdatePattern()
    {
        counter += Time.deltaTime;
        if (counter >= frequency && !addForce)
        {
            emitProjectile();
            counter = 0;
        }
    }
    void emitProjectile()
    {
        GameObject lastBullet = null;
        for (int i = 0; i < 4; i++)
        {
            Vector3 thispos = new Vector3(count * radius, count * radius, 0);
            thispos = Quaternion.Euler(0, 0, angle) * thispos;
            speed_multiplier = bulletspeed * sb.force;
            lastBullet = (GameObject)Instantiate(sb.typeProjectiles[0], sb.transform.position + thispos, Quaternion.identity);
            lastBullet.GetComponent<ProjectileBehavior>().launchedby = "sun";
            bullets.Add(lastBullet);
            angle += 90;
        }
        count++;

        if ((lastBullet.transform.position - sb.transform.position).magnitude > 5f)
        {
            addForce = true;
            foreach (GameObject go in bullets)
            {
                go.GetComponent<Rigidbody2D>().AddForce(go.transform.position * speed_multiplier);
            }
        }
    }

    public void EndPattern()
    {
        throw new NotImplementedException();
    }
}
*/