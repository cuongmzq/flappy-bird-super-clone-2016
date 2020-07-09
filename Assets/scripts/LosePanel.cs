using UnityEngine;
using UnityEngine.UI;
public class LosePanel : MonoBehaviour
{
    public GameObject GameOver, play_rank;
    public Image im;
    public Image[] p_r;
    public bool reach;
    private float pdis;

    void Start()
    {
        im = this.GetComponent<Image>();
        p_r = play_rank.GetComponentsInChildren<Image>();
        pdis = transform.position.y - play_rank.transform.position.y;
    }

    void Update()
    {
        if (transform.position != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.fixedDeltaTime * 20);
            if (play_rank.transform.position.y + pdis != transform.position.y)
            {
                play_rank.transform.position = new Vector2(0, transform.position.y - pdis);
            }
            transform.position = new Vector3(0, Mathf.Clamp(transform.position.y, -2, 0));
        }
    }

    void OnEnable()
    {
        im = this.GetComponent<Image>();
        im.CrossFadeAlpha(255, 1, false);
    }

    void OnDisable()
    {
        transform.position = new Vector3(0, -1);
        Color c = im.color;
        c.a = 0.01f;
        im.color = c;
    }
}
