using UnityEngine;
using System.Collections;

public class TileAction : MonoBehaviour
{
    public float JumpPower = 3.2f;
    protected Vector2 RePosition;
    protected Vector2 moveDirection;
    protected float jumpSpeed = -0.06f;
    protected float gravity = 0.6f;
    protected bool touched = false;
    public void OnGroundBall()
    {
        touched = true;
        moveDirection.y = jumpSpeed;
    }
    // Update is called once per frame
    private void Start()
    {
        RePosition = transform.position;
    }
    void Update()
    {
        if (touched == true)
        {
            moveDirection.y += gravity * Time.deltaTime;
            transform.Translate(moveDirection);

            if(transform.position.y > RePosition.y)
            {
                transform.position = RePosition;
                touched = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            foreach (ContactPoint2D hitPos in collision.contacts)
            {
                if (hitPos.normal.y < 0) // check if its collided on top 
                {
                    var Ball = collision.gameObject.GetComponent<BounceBall>();
                    if (Ball != null)
                    {
                        Debug.Log("OnTriggerEnter2D");
                        OnGroundBall();
                        Ball.JumpPower = JumpPower;
                        Ball.onGround = true;
                    }
                    break;
                }
            }
        }

    }
}
