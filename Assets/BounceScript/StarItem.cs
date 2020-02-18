using UnityEngine;
using System.Collections;

public class StarItem : MonoBehaviour
{
    protected bool touchedBall = false;
    protected SpriteRenderer sprite;
    // Use this for initialization
    void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DestroyIncomming()
    {
        float duration = 0.3f;
        float startTime = Time.time;
        Vector2 oPos = transform.position;
        Vector2 nPos = new Vector2(oPos.x, oPos.y + 1);
        while (sprite.color.a > 0.0f)
        {
            float t = (Time.time - startTime) / duration;
            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1.0f, 0.0f, t));
            transform.position = new Vector2(oPos.x, Mathf.SmoothStep(oPos.y, nPos.y, t));
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9 && touchedBall == false)
        {
            touchedBall = true;
            StartCoroutine("DestroyIncomming");
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
