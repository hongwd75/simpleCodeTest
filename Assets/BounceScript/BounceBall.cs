using UnityEngine;
using System.Collections;

public class BounceBall : MonoBehaviour
{
    public Vector2 startPoint;
    public float height;
    public bool onGround = false;
    public float JumpPower = 3.2f;
    protected Rigidbody2D rigidBody = null;

    // Use this for initialization
    void Start()
    {
        onGround = false;
        rigidBody = GetComponent<Rigidbody2D>();
        startPoint = transform.position;
        height = transform.localScale.y / GetComponent<SpriteRenderer>().sprite.bounds.size.y+0.2f;
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


    private void FixedUpdate()
    {
        if (onGround == true && JumpPower != 0.0f)
        {
            onGround = false;
            rigidBody.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
        }
        else
        {
            UpdateInput();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 0)
        {
            transform.position = startPoint;
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.layer == 8)
    //    {
    //      //  if(Physics2D.Raycast(gameObject.transform.position, Vector2.down, height, 1 << 8) == true)
    //        { 
    //            onGround = true;
    //            var tileAction = collision.gameObject.GetComponent<TileAction>();
    //            if (tileAction != null)
    //            {
    //                Debug.Log("tileAction");
    //                tileAction.OnGroundBall();
    //            }
    //        }
    //    }
    //}
}
