using UnityEngine;

public class groundscr : MonoBehaviour
{
    public float speed;
    private float boxcol;
    public static bool canMove;

    void Start()
    {
        canMove = true;
        boxcol = GetComponent<BoxCollider2D>().size.x * 1f;
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
            transform.position = new Vector2(transform.position.x + boxcol, transform.position.y);
        }
    }
}
