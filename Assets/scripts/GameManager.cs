using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pipe_01, pipe_02, blu_bird, yel_bird, ora_bird, bg_01, bg_02, ground, tut, flappyBird, play_rank, playerin, topBlock, pipe_all, area;
    public Image medal, newB, flash, losePanel, gameOver;
    public Image[] p_r;
    public Text _score, los_score, los_hiscore;
    public int pre_spawn_pipe, hiscore;
    public float max_pipe_y, min_pipe_y, pipe_dis, birdMoveSmooth;
    private float screenx;
    public static bool controlable;
    public bool firstPlay, start;
    public static int score;
    public static AudioSource audioSource;
    public List<GameObject> pipe_ = new List<GameObject>();
    public AudioClip bonus, whoosp;
    private player playersr;
    private Rigidbody2D rgd;
    public Sprite _sil_medal, _gol_medal, sil_medal, gol_medal;
    private Vector2 screenTo;

    void Awake()
    {
        screenTo = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        min_pipe_y = -screenTo.y + 1.4f;
        max_pipe_y = screenTo.y - 0.8f;
    }
    void Start()
    {
        firstPlay = true;
        Debug.Log(screenTo);
        Debug.Log(min_pipe_y + "  " + max_pipe_y);
        area.GetComponent<BoxCollider2D>().size = new Vector2(screenTo.x * 2, screenTo.y * 2);
        topBlock.GetComponent<BoxCollider2D>().size = new Vector2(screenTo.x * 2, 0.25f);
        topBlock.transform.position = new Vector2(0, screenTo.y + 0.5f);
        screenx = screenTo.x;
        newB.gameObject.SetActive(false);
        if (!File.Exists(Application.persistentDataPath + "//userdata//score.txt"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "//userdata");
            StreamWriter fi = new StreamWriter(Application.persistentDataPath + "//userdata//score.txt");
            fi.WriteLine("Score: 0");
            fi.Write("Hi_Score: 0");
            fi.Close();
        }
        else
        {
            StreamReader R = new StreamReader(Application.persistentDataPath + "//userdata//score.txt");
            string all_text = R.ReadToEnd();
            R.Close();
            if (all_text == "" || all_text == null)
            {
                R.Close();
                StreamWriter W = new StreamWriter(Application.persistentDataPath + "//userdata//score.txt");
                W.WriteLine("Score: 0");
                W.Write("Hi_Score: 0");
                W.Close();
            }
            else
            {
                string[] all_lines = all_text.Split('\n');
                int i = 0;
                while (!all_lines[i].Contains("Hi_Score: "))
                {
                    i++;

                }
                int.TryParse(all_lines[i].Remove(0, 10), out hiscore);
            }
        }
        audioSource = GetComponent<AudioSource>();
        HomeScreen();
        p_r = play_rank.GetComponentsInChildren<Image>();
    }

    void FixedUpdate()
    {
        if (!start && !firstPlay)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                TouchPhase phase = touch.phase;
                if (phase == TouchPhase.Began)
                {
                    StartPlaying();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                StartPlaying();
            }
        }
    }

    public void Lose()
    {
        newB.gameObject.SetActive(false);
        player.lose = true;
        if (score == 0)
        {
            medal.gameObject.SetActive(false);
        }
        else
        {
            medal.gameObject.SetActive(true);
            if (score < 5)
            {
                medal.sprite = _sil_medal;
            }
            else if (score < 10)
            {

                medal.sprite = _gol_medal;
            }
            else if (score < 20)
            {
                medal.sprite = sil_medal;
            }
            else if (score >= 20)
            {
                medal.sprite = gol_medal;
            }
        }
        if (score > hiscore)
        {
            hiscore = score;
            newB.gameObject.SetActive(true);
        }
        los_hiscore.text = hiscore.ToString();
        los_score.text = 0.ToString();
        StreamWriter W = new StreamWriter(Application.persistentDataPath + "//userdata//score.txt");
        W.WriteLine("Score: " + score.ToString());
        W.Write("Hi_Score: " + hiscore.ToString());
        W.Close();
        StartCoroutine(Los_panel());
    }

    IEnumerator Los_panel()
    {
        yield return new WaitForSeconds(0.8f);
        Color c = gameOver.color;
        c.a = 0.1f;
        gameOver.color = c;
        gameOver.gameObject.SetActive(true);
        gameOver.CrossFadeAlpha(255, 5, false);
        audioSource.PlayOneShot(whoosp);
        yield return new WaitForSeconds(0.6f);
        losePanel.gameObject.SetActive(true);
        foreach (Image t in p_r)
        {
            t.color = c;
        }
        Vector3 p = play_rank.transform.position;
        p.y -= 1;
        play_rank.transform.position = p;
        play_rank.SetActive(true);
        foreach (Image t in p_r)
        {
            t.CrossFadeAlpha(255, 10, false);
        }
        Mov();
        audioSource.PlayOneShot(whoosp);
        StartCoroutine(Disp(0.25f));
    }

    IEnumerator Disp(float ime)
    {
        yield return new WaitForSeconds(0.5f);
        int s = 0;
        while (s < score)
        {
            yield return new WaitForSeconds(ime / score);
            s += 1;
            los_score.text = s.ToString();
        }
    }

    void Mov()
    {
        if (play_rank.transform.position != Vector3.zero)
        {
            Vector2 pos = play_rank.transform.position;
            pos.y += 1;
            play_rank.transform.position = pos;
        }
    }

    public void Bonus()
    {
        audioSource.PlayOneShot(bonus);
        score++;
        _score.text = score.ToString();
    }

    public void Reset()
    {
        if (play_rank.activeInHierarchy)
        {
            play_rank.SetActive(false);
        }
        //A few Stuff...
        audioSource.PlayOneShot(whoosp);
        losePanel.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        _score.text = 0.ToString();
        score = 0;
        start = false;
        //back_ground
        if (GameObject.Find("Background"))
        {
            Destroy(GameObject.Find("Background"));
        }
        CreateBackground();
        //ground
        if (!GameObject.FindWithTag("ground"))
        {
            CreateGround();
        }
        //pipe
        if (GameObject.FindWithTag("Pipe"))
        {
            Reset_pipe();
        }
        else
        {
            CreatePipe();
        }
        //Player
        if (!firstPlay)
        {
            if (GameObject.FindWithTag("Player"))
            {
                Destroy(GameObject.FindWithTag("Player"));
            }
            CreateBird(new Vector2(-0.65f, 0));
        }
        else
        {
            firstPlay = false;
            flappyBird.SetActive(false);
            GameObject t = GameObject.FindWithTag("Player");
            Vector3 pos = new Vector3(-0.65f, 0);
            t.transform.position = pos;

        }
        //Something else
        groundscr.canMove = true;
        pipe.canMove = false;
        rgd = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        rgd.velocity = Vector2.zero;
        rgd.gravityScale = 0;
        tut.SetActive(true);
        _score.gameObject.SetActive(true);
        playersr.controlable = true;
    }

    void CreateBird(Vector2 pos)
    {
        var rand_bird = Random.Range(-45, 45);
        GameObject bird = blu_bird;
        if (rand_bird < -15)
        {
            bird = blu_bird;
        }
        else if (rand_bird > 15)
        {
            bird = yel_bird;
        }
        else
        {
            bird = ora_bird;
        }
        GameObject player = (GameObject)Instantiate(bird, pos, Quaternion.identity);
        player.name = "bird";
        playerin = player;
        playersr = playerin.GetComponentInChildren<player>();
    }

    void CreateGround()
    {
        GameObject go = new GameObject();
        go.name = "ground_all";
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = new Vector2(i * 3.36f, -2);
            GameObject g = (GameObject)Instantiate(ground, pos, Quaternion.identity);
            g.name = "ground_" + (i + 1);
            g.transform.parent = go.transform;
        }
    }

    void CreatePipe()
    {
        var rand_pipe = Random.Range(-50, 100);
        GameObject go = rand_pipe < 98 ? pipe_01 : pipe_02;
        if (!GameObject.Find("pipe_all"))
        {
            pipe_all = new GameObject();
            pipe_all.name = "pipe_all";
        }
        for (int i = 1; i <= pre_spawn_pipe; i++)
        {
            Vector2 pos = new Vector2((screenx) / 2 + i * (pipe_dis), Random.Range(min_pipe_y, max_pipe_y));
            GameObject pipe_x = (GameObject)Instantiate(go, pos, Quaternion.identity);
            pipe_x.name = "pipe_" + i;
            pipe_x.transform.parent = pipe_all.transform;
            pipe_.Add(pipe_x);
        }
    }
    void Reset_pipe()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Pipe");
        Vector3 pos = new Vector2((screenx) / 2 + pipe_dis, Random.Range(min_pipe_y, max_pipe_y));
        foreach (GameObject t in p)
        {
            t.SetActive(false);
            t.transform.position = pos;
            pos.x += pipe_dis;
            pos.y = Random.Range(min_pipe_y, max_pipe_y);
            t.SetActive(true);
        }
    }

    void CreateBackground()
    {
        var rand_bg = Random.Range(-100, 100);
        GameObject bg = rand_bg > 0 ? bg_01 : bg_02;
        GameObject BG = (GameObject)Instantiate(bg, Vector2.zero, Quaternion.identity);
        BG.name = "Background";
    }

    void StartPlaying()
    {
        playersr.anim.SetBool("isStart", true);
        playersr.controlable = true;
        play_rank.SetActive(false);
        rgd = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        start = true;
        rgd.gravityScale = 1.2f;
        tut.SetActive(false);
        pipe.canMove = true;
    }

    void HomeScreen()
    {
        flappyBird.SetActive(true);
        _score.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        tut.SetActive(false);
        play_rank.SetActive(true);
        CreateBackground();
        CreateGround();
        CreateBird(new Vector2(0, 0));
        playersr.controlable = false;
    }
}
