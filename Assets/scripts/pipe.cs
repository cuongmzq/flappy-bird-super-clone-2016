using UnityEngine;

public class pipe : MonoBehaviour
{
    public float speed, pipe_dis;
    private float max_y, min_y;
    private GameManager gameMgr;
    private float x;
    public static bool canMove;

    void Start()
    {
        canMove = false;
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        min_y = gameMgr.min_pipe_y;
        max_y = gameMgr.max_pipe_y;
        x = (gameMgr.pipe_dis * gameMgr.pre_spawn_pipe);
    }

    void Update()
    {
        if (canMove)
        {
            transform.Translate(Vector3.left * speed);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.name == "area")
        {
            Vector2 pos = new Vector2(x + transform.position.x, Random.Range(min_y, max_y));
            transform.position = pos;
        }
    }
}
