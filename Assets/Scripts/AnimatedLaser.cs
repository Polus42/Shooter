using UnityEngine;
using System.Collections;

public class AnimatedLaser : MonoBehaviour {

    private Animator animator;

    public float timeBeforeBig = 2f;
    public LayerMask playersMasks;

    // Define an "infinite" size, not too big but enough to go off screen
    private float maxLaserSize = 7f;
    // Raycast at the right as our sprite has been design for that
    private Vector2 laserDirection;

    void Start()
    {
    //    
    }

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    Coroutine toBigCoroutine;

    void OnEnable()
    {
        GetComponents<AudioSource>()[0].Play();
        animator.Rebind();
        if (toBigCoroutine != null)
            StopCoroutine(toBigCoroutine);
        toBigCoroutine = StartCoroutine(toBig());
    }

    IEnumerator toBig()
    {
        yield return new WaitForSeconds(timeBeforeBig);
        GetComponents<AudioSource>()[0].Stop();
        GetComponents<AudioSource>()[1].Play();
        animator.SetBool("isBig", true);
    }

    //Collisions trigger uniquement avec layer p1 et p2
    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("laser enter: " + other.name);
        animator.SetBool("touchPlayer", true);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("laser stay: " + other.name);
        other.SendMessage("ApplyDamage", 1);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("laser exit: " + other.name);
        animator.SetBool("touchPlayer", false);
    }
    */
    

    void Update()
    {
        laserDirection = this.transform.right;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, laserDirection, maxLaserSize, playersMasks);
        Debug.DrawRay(transform.position, laserDirection* maxLaserSize, Color.green);

        if (hit.collider != null 
            && (hit.collider.gameObject.GetComponent<PlayerBehavior>() != null))
        {
            Debug.Log("laser touched !: " + hit.transform.gameObject.name);
            // We touched something!
            animator.SetBool("touchPlayer", true);
            hit.transform.gameObject.SendMessage("ApplyDamage", 1);

            // -- Get the laser length
            //currentLaserSize = Vector2.Distance(hit.point, this.transform.position);
        }
        else
        {
            animator.SetBool("touchPlayer", false);
        }
        
        /*
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0)
        {
            animator.SetBool("isBig", true);
        }
        else if (vertical < 0)
        {
            animator.SetBool("touchPlayer", true);
        }
        else if (horizontal > 0)
        {
            animator.SetBool("isBig", false);
        }
        else if (horizontal < 0)
        {
            animator.SetBool("touchPlayer", false);
        }
        */
    }
}
