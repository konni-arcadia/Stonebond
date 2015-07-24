using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	//Default controler id for this player
    public int controler = 1;

	//[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.

    //public AudioSource audioSource;


	public Transform[] groundChecks;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.

	//Name of the axes and buttons used
    private string horizontal = "Horizontal";
    private string vertical = "Vertical";
    private string joystick = "joystick";
    private string a = "button 0";
    private string dpadHorizontal = "DpadHorizontal";
    private string dpadVertical = "DpadVertical";

    private Constants constants;

    private PlayerControl[] playerList;

    private BoxCollider2D myBoxCollider;

    protected Animator myAnimator;

	void Awake()
	{

	}

    void Start()
    {
        myAnimator = GetComponentInChildren<Animator>();
        myBoxCollider = GetComponent<BoxCollider2D>();

        //audioSource = GetComponent<AudioSource>();

		//Here we were trying to get the controler id in the PlayerPref (a static class managed by unity where we can store data) based on the "color" (here written as power)
		//and assign it to the controler variable of this player
        /*int controlerValueFromPlayerPrefs = PlayerPrefs.GetInt(playerPower.powerType.ToString(), -1);
        if (controlerValueFromPlayerPrefs >= 0)
        {
            controler = controlerValueFromPlayerPrefs;
        }*/
 
		//We had a big animation manager (state machine) with all possible player sprites in it, so we had to define wich sprite to use
        //myAnimator.SetInteger("PlayerCharacter", (int)playerPower.powerType);

        constants = FindObjectOfType<Constants>();

        horizontal += controler;
        vertical += controler;
        joystick += " " + controler;
        
        a = joystick + " " + a;
        dpadHorizontal += controler;
        dpadVertical += controler;

        
        playerList = FindObjectsOfType<PlayerControl>();
    }

	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = false;
        for (int i = 0; i < groundChecks.Length; i++)
            grounded = grounded || Physics2D.Linecast(transform.position, groundChecks[i].position, constants.GroundLayer);

		//Gestion des flags pour le système d'animation
        //myAnimator.SetBool("Grounded", grounded);
		
		// If the jump button is pressed and the player is grounded then the player should jump.
        if (Input.GetKeyDown(a) && grounded)
        {
            //myAnimator.SetTrigger("Jumping");
			jump = true;;
            //audioSource.PlayOneShot(constants.JumpSound);
        }
        //Gestion de la croix (dpad)
        /*if (Input.GetAxis(dpadHorizontal) < 0)
            //Do something
        else if (Input.GetAxis(dpadHorizontal) > 0)
			//Do something else
        else if (Input.GetAxis(dpadVertical) < 0)
			//Do something else
        else if (Input.GetAxis(dpadVertical) > 0)
			//Do something else */
	}

	void FixedUpdate ()
	{
		// Cache the horizontal input.
        float h = Input.GetAxis(horizontal);
        if (Mathf.Abs(h) < 1)
        {
            h = 0;
        }

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody2D.velocity.x < constants.maxVelocityX)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(rigidbody2D.velocity.x) > constants.maxVelocityX)
			// ... set the player's velocity to the maxSpeed in the x axis.
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * constants.maxVelocityX, rigidbody2D.velocity.y);

		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();

		// If the player should jump...
		if(jump)
		{
            // Before adding jump force we nullify the current vertical speed
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
			// Add a vertical force to the player.
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
