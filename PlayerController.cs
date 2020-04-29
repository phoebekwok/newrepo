using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;     //Control the speed that the player is moving around in the world
    public Rigidbody2D rb;    //Store a reference to the Rigidbody2D component required to use 2D Physics 

    public float jumpSpeed;     //Control the speed when the player jumps

    //Ground detection variables
    public Transform groundCheck;   // A point in space to check where the ground is
    public float groundCheckRadius;
    public LayerMask realGround;
    public bool isGrounded;

    private Animator myAnim;     //Store a reference to the Animator component

    public Vector3 respawnPosition; //Store a respawn position the player will go to whenever she dies.

    public LevelManager theLevelManager;    //Make reference to LevelManager 

    public float knockbackForce;
    public float knockbackLength;      // Amount of time the player being knocked back
    private float knockbackCounter;    // Count down of time the player being knocked back

    public AudioSource jumpSound;
    public AudioSource hurtSound;

    public GameObject bulletToRight;
    public GameObject bulletToLeft; //Game obejct will be instantiated when hit the fire button
    private Vector2 bulletPos;  //Coordinates where the bullet should be instantiated
    public float fireRate;
    private float nextFire;
    private bool facingRight = true;

    public bool canMove = true;    //When game is paused, player cannnot move

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();   //Get and store a reference to the Rigidbody2D component so that we can access it.
        myAnim = GetComponent<Animator>();  //Get and store a reference to the Animator component so that we can access it.

        respawnPosition = transform.position; //When game starts, respawn position equals to the current players position.

        theLevelManager = FindObjectOfType<LevelManager>(); 
    }
	
	// Update is called once per frame
	void Update () {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, realGround);

        if (knockbackCounter <= 0 && canMove)   //If there is no knock back
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);    // Move to the right
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);   // Make sure when move right, face right
                facingRight = true;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);   // Move to the left 
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);   // Make sure when move left, face left
                facingRight = false;
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);    //If input is staying at 0, player should be standing still.
            }

            // character must be touching ground AND key W must be pressed in order to jump
            // red words are newly added condition
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);       // For jumping
                jumpSound.Play();
            }

            if (Input.GetKeyDown(KeyCode.L) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();
            }
        }

        if (knockbackCounter > 0)
        {
            knockbackCounter -= Time.deltaTime;    //Time Count down

            if (transform.localScale.x > 0)    
            {
                rb.velocity = new Vector3(-knockbackForce, knockbackForce, 0.0f);   //The force to push the player back 
            }
            else
            {
                rb.velocity = new Vector3(knockbackForce, knockbackForce, 0.0f);   //The force to push the player back
            }        
        }

        myAnim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        myAnim.SetBool("Ground", isGrounded);
    }

    public void Knockback()
    {
        knockbackCounter = knockbackLength;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "KillPlane")
        {
            //gameObject.SetActive(false);    //Set the player to inactive

            //transform.position = respawnPosition;   //Set the position of player to respawnPosition when it dies
            theLevelManager.healthCount -= 100;
            theLevelManager.UpdateHeartMeter();
            theLevelManager.Respawn(); 
        }

        if (other.tag == "Checkpoint")
        {
            respawnPosition = other.transform.position;    //Set the player respawn position to checkpoint position
        }
    }

    void Fire()
    {
        bulletPos = transform.position; // Set the position of the bullet to be player position
        if (facingRight)
        {
            bulletPos += new Vector2(+1f, -0.43f);
            Instantiate(bulletToRight, bulletPos, Quaternion.identity);
        }
        else
        {
            bulletPos += new Vector2(-1f, -0.43f);
            Instantiate(bulletToLeft, bulletPos, Quaternion.identity);
        }
    }
}
