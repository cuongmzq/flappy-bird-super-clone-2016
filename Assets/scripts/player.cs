using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class player : MonoBehaviour
{
    public static Rigidbody2D rgd;
    public Animator anim;
    public Animation anima;
    private GameManager gameMgr;
    private AudioSource audioSource;
    public AudioClip flap, hit, die;
    public float speed;
    public static bool scored, lose;
    public bool controlable;
    public Image flash;

    void Start()
    {
        scored = false;
        lose = false;
        controlable = false;
        rgd = GetComponent<Rigidbody2D>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GameManager.audioSource;
        flash = gameMgr.flash;
        anim = this.GetComponentInParent<Animator>();
    }

    void FixedUpdate()
    {
        if (controlable)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                TouchPhase phase = touch.phase;
                if (phase == TouchPhase.Began)
                {
                    Tap();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Tap();
            }
        }
        if (rgd.velocity.y < 0)
        {
            anim.SetBool("tapped", false);
        }
    }

    void Tap()
    {    
        Vector2 des = new Vector2(0, speed);
        rgd.velocity = des;
        audioSource.PlayOneShot(flap);
        anim.SetBool("tapped",true);
        anim.SetTrigger("Tap");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground")
        {
            anim.enabled = false;
            controlable = false;
            if (!lose)
            {
                audioSource.PlayOneShot(hit);
                StartCoroutine(Flashing(0.3f));
            }
            transform.rotation=Quaternion.Euler(0,0,-90);
            lose = true;
            rgd.velocity = Vector2.zero;
            StopObstacle();
            gameMgr.Lose();
        }

    }

    IEnumerator Flashing(float time)
    {
        flash.gameObject.SetActive(true);
        flash.CrossFadeAlpha(0, 0.25f, false);
        yield return new WaitForSeconds(time);
        flash.gameObject.SetActive(false);
        Color c = flash.color;
        c.a = 1;
        flash.color = c;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Pipe")
        {
            scored = false;
        }

        if (col.name == "pipe" && controlable)
        {
            controlable = false;
            lose = true;
            StopObstacle();
            StartCoroutine(Flashing(0.3f));
            audioSource.PlayOneShot(hit);
            audioSource.PlayOneShot(die);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Pipe")
        {
            if (transform.position.x > col.transform.position.x && !scored)
            {
                gameMgr.Bonus();
                scored = true;
            }
        }
    }

    void StopObstacle()
    {
        pipe.canMove = false;
        groundscr.canMove = false;
    }
}