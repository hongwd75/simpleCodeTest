using UnityEngine;
using System.Collections;

public class BounceBall : MonoBehaviour
{
    public Vector2 startPoint;

    protected bool onGround = false;
    protected Rigidbody2D rigidBody = null;
    // Use this for initialization
    void Start()
    {
        onGround = false;
        rigidBody = GetComponent<Rigidbody2D>();
        startPoint = transform.position;


    }


    void UpdateInput()
    {
        if(Input.GetMouseButton(0) == true)
        {
            var pos = Input.mousePosition;
            if(pos.x < Screen.width / 2)
            {
                transform.position = new Vector2(transform.position.x - 3.0f * Time.deltaTime, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x + 3.0f * Time.deltaTime, transform.position.y);
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (onGround == true)
        {
            onGround = false;
            rigidBody.AddForce(Vector2.up * 3.2f, ForceMode2D.Impulse);
        } else
        {
            UpdateInput();
        }

        if(transform.position.y < 0)
        {
            transform.position = startPoint;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            onGround = true;
        }
    }
}
