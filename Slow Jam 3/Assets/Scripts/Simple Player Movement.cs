using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float maxSpeed = 5;
    public Transform groundCheck;
    public float jumpForce = 1000;

    protected Animator myAnimator;
    protected Rigidbody2D myRigidBody;
    protected float moveForce = 365;
    protected bool facingRight = true;
    protected bool grounded = false;
    protected bool jump = false;

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //layer mask bitwise ops: https://answers.unity.com/questions/8715/how-do-i-use-layermasks.html
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        myAnimator.SetFloat("Speed", Mathf.Abs(horizontalAxis));
        //Have we reach maxSpeed? If not, add force.
        if (horizontalAxis * myRigidBody.linearVelocity.x < maxSpeed)
        {
            myRigidBody.AddForce(Vector2.right * horizontalAxis * moveForce);
        }
        //have we exceeded the maxSpeed? Clamp it (set it to maxSpeed).
        if (Mathf.Abs(myRigidBody.linearVelocity.x) > maxSpeed)
        {
            myRigidBody.linearVelocity = new Vector2(Mathf.Sign(myRigidBody.linearVelocity.x) * maxSpeed, myRigidBody.linearVelocity.y);
        }

        if (jump)
        {
            myAnimator.SetTrigger("Jump");
            myRigidBody.AddForce(new Vector2(0, jumpForce));
            jump = false;
        }

        if (horizontalAxis > 0 && !facingRight)
        {
            Flip();
        }

        else if (horizontalAxis < 0 && facingRight)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}