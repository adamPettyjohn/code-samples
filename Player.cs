using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//This is used for players that need to be controlled.
//Currently this is only for online, tweaks will need to be made for
//local play.
//REQUIRES Health COMPONENT

public class Player : NetworkBehaviour {
    //These top variables are used for testing, they wont be here in the end
    public GameObject testSpawn;


    //this is the "team" that the player is on, anything not on this team takes damage from
    //the player
    [SyncVar (hook = "UpdateTeam")] public string currentTeam;
    public float startHealth;
    public float startEnergy;

    //basic variables
    public float speed;
    public float faceAngle;

    //basic attack variable, these are derived from the weapon and thus
    //should not be directly modified
    private float attackWindUp;
    private float attackRadius;
    private float attackAngle;
    private float attackSpeed;
    private float attackCoolDown;

    //The weapon slot on the player
    public GameObject currentSword;

    //The ability slots on the player
    public GameObject ability1;
    public GameObject ability2;
    public GameObject ability3;

    //the acutally ability objects
    private GameObject curAbility1;
    private GameObject curAbility2;
    private GameObject curAbility3;

    //should be private, public for now for testing
    public float abilityMovementModifier;
    
    private float energy;

    //counting variables used by the player class to keep track of things,
    //do not modify directly
    private float currentAttackCoolDown;
    private float totalTravel;
    private bool isSwinging;
    private GameObject spawner;

    //this is the aimer
    public GameObject aimer;

    //These variables deal with the player jumping
    bool flashCharging;
    public float minJump;
    public float jumpCooldown;
    private float currentJumpCooldown;
   
    public GameObject jumpSmoke;

    //this bool is used to indicate the player is doing and exclusive action
    //such as attacking or using an ability
    //should be private, public for now for testing
    public bool doingAction;

    //Atempts to slot the passed in gameobject into one of the players ability slots
    public void TryToSlotIn(GameObject able)
    {
        if(curAbility1 == null)
        {
            curAbility1 = able;
        } else if(curAbility2 == null)
        {
            curAbility2 = able;
        } else if(curAbility3 == null)
        {
            curAbility3 = able;
        }
    }

    //Initialize health before anything
    void Awake()
    {
        gameObject.GetComponent<Health>().SetHealth(startHealth);
        gameObject.GetComponent<Health>().currentTeam = currentTeam;
    }

    //When the team is modified (the server modifies it on log in) we need to make
    //sure that it is properly updated on both the health component and the weapon
    void UpdateTeam(string team)
    {
        gameObject.GetComponent<Health>().currentTeam = currentTeam;
        UpdateWeapon();
    }

    //Update the weapon and attempt to spawn the player
    void Start ()
    {
        GameObject tempAim = (GameObject)Instantiate(aimer, transform.position, transform.rotation);
        tempAim.transform.parent = transform;

        abilityMovementModifier = 1f;

        UpdateWeapon();
        CmdUpdateAbilities();
        //UpdateAbilities();

        spawner = GameObject.FindGameObjectWithTag("SpawnMaster");
        spawner.GetComponent<SpawnMaster>().AttemptSpawn(gameObject);
    }

    //This sets the team and owner of the weapon and then grabs its swing
    //variables for the player
    public void UpdateWeapon()
    {
        currentSword.GetComponent<Blade>().currentTeam = currentTeam;
        currentSword.GetComponent<Blade>().owner = gameObject;

        attackWindUp = currentSword.GetComponent<Blade>().attackWindUp;
        attackRadius = currentSword.GetComponent<Blade>().attackRadius;
        attackAngle = currentSword.GetComponent<Blade>().attackAngle;
        attackSpeed = currentSword.GetComponent<Blade>().attackSpeed;
        attackCoolDown = currentSword.GetComponent<Blade>().attackCoolDown;
    }

    //This tells the abilities that the player has who the owner is
   public void UpdateAbilities()
    {
        if(ability1 != null)
        {
            GameObject tempAb1 = (GameObject)Instantiate(ability1, transform.position, transform.rotation);
            tempAb1.transform.parent = transform;
            curAbility1 = tempAb1;
            curAbility1.GetComponent<Ability>().user = gameObject;
        }
        if (ability2 != null)
        {
            GameObject tempAb2 = (GameObject)Instantiate(ability2, transform.position, transform.rotation);
            tempAb2.transform.parent = transform;
            curAbility2 = tempAb2;
            curAbility2.GetComponent<Ability>().user = gameObject;
        }
        if (ability3 != null)
        {
            GameObject tempAb3 = (GameObject)Instantiate(ability3, transform.position, transform.rotation);
            tempAb3.transform.parent = transform;
            curAbility3 = tempAb3;
            curAbility3.GetComponent<Ability>().user = gameObject;
        }
    }

    // Update is called once per frame
    void Update () 
    {
	Move ();
	Attack ();
    	CastAbilities();
	DecreaseCounts ();
    	CheckHealth();
    }

    //We check to see if we died. If we did we reset our health and respawn
    void CheckHealth()
    {
        if(gameObject.GetComponent<Health>().MyHealth() <= 0f)
        {
            spawner.GetComponent<SpawnMaster>().AttemptSpawn(gameObject);
            CmdTellServerSetHealth(gameObject, startHealth);
            
        }
    }

	//anything that needs counting goes here
    void DecreaseCounts() 
    {
	if (currentAttackCoolDown > 0f) 
	{
	    currentAttackCoolDown -= Time.deltaTime;
	}
        if(currentJumpCooldown > 0f)
        {
            currentJumpCooldown -= Time.deltaTime;
        }
    }

    //player ability code
    void CastAbilities()
    {
        if (Input.GetButtonDown("X_1") && !doingAction)
        {
            if(curAbility1 != null)
            {
                curAbility1.GetComponent<Ability>().TryToCast();
            }
        }
        else if (Input.GetButtonDown("Y_1") && !doingAction)
        {
            if (curAbility2 != null)
            {
                curAbility2.GetComponent<Ability>().TryToCast();
           }
        }
        else if (Input.GetButtonDown("B_1") && !doingAction)
        {
            if (curAbility3 != null)
            {
                curAbility3.GetComponent<Ability>().TryToCast();
            }
        }

    }

    //player basic attack code
    void Attack () 
    {
	if (currentAttackCoolDown <= 0f && !isSwinging) 
	{
	    if(Input.GetButtonDown("A_1") && !doingAction) 
	    {
            	doingAction = true;

            	CmdTellServerPlaySound(gameObject, "swordSwing");

	    	currentSword.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + attackRadius, 0f);
            	currentSword.transform.rotation = Quaternion.identity;
	    	currentSword.transform.RotateAround (gameObject.transform.position, Vector3.forward, faceAngle - (attackAngle / 2f));
        
            	currentSword.GetComponent<BoxCollider2D>().enabled = true;

	    	totalTravel = 0f;
            	currentAttackCoolDown = attackCoolDown;
            	isSwinging = true;
	    }
	}

	if (isSwinging) 
	{
	    currentSword.transform.RotateAround (gameObject.transform.position, Vector3.forward, attackSpeed * Time.deltaTime);
	    totalTravel += attackSpeed * Time.deltaTime;
			
	    //the swing has finished
	    if(totalTravel > attackAngle) 
	    {
                //currentSword.GetComponent<SpriteRenderer>().enabled = false;
                currentSword.GetComponent<BoxCollider2D>().enabled = false;
                isSwinging = false;
                currentSword.GetComponent<Blade>().ClearHitList();

                doingAction = false;
            }
	}
    }

    //code that deals with moveing the player
    void Move() 
    {
    	//first get the inputs
	float h = Input.GetAxis ("L_XAxis_1");
	float v = -Input.GetAxis ("L_YAxis_1");

        Vector2 move;

	//now we check to see if its in our sensitivty (this prevents stick drift)
	if (Mathf.Abs (h) < 0.3f && Mathf.Abs (v) < 0.3f) 
	{
		h = 0f;
		v = 0f;
        } 

        move = new Vector2(h, v);

        //now we normilize the movement vector (so you cant move faster diagonally)
        //note that this prevents "slow move" its all or nothing. May change this later
        if (move.magnitude > 1f) {
            move.Normalize();
        }
		
	//do the rotation, only if we moved (again so theres no drift)
	if (Mathf.Abs(h) > 0f || Mathf.Abs(v) > 0f) 
	{
	    float angle = Mathf.Atan2 (move.x, move.y) * Mathf.Rad2Deg;
	    Vector3 pAngles = new Vector3 (0f, 0f, -angle);

	    if(!isSwinging) 
	    {
		gameObject.transform.rotation = Quaternion.Euler (pAngles);
		faceAngle = -angle;
            }
	}

        float holdStill = Input.GetAxis("TriggersL_1");
        float jumpAmount = Input.GetAxis("TriggersR_1");
        
        //Atempt to jump
        if(flashCharging)
        {
            if (jumpAmount > 0.1f)
            {
            
            } else
            {
                flashCharging = false;
            }
            
        } else
        {
            if(jumpAmount > 0.1f && currentJumpCooldown <= 0f && !doingAction)
            {
                currentJumpCooldown = jumpCooldown;
                Debug.Log("begin charge");
                CmdCreateObject(jumpSmoke, transform.position, transform.rotation);
                transform.position = transform.position + transform.up * (minJump);
                CmdCreateObject(jumpSmoke, transform.position, transform.rotation);

                flashCharging = true;
            }
        }


        //now we move the object
        if (!isSwinging && holdStill < 0.3f)
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = move * speed * abilityMovementModifier;
	} else
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
    }

 

    //This command applies the passed in damage to the passed in gameobject across the network
    [Command]
    public void CmdTellServerApplyDamage(GameObject victim, float damage)
    {
        victim.GetComponent<Health>().ApplyDamage(damage);
    }
  
    //Sets the passed in health of the passed in object across the network
    [Command]
    public void CmdTellServerSetHealth(GameObject victim, float health)
    {
        victim.GetComponent<Health>().SetHealth(health);
    }
    
    //Plays a specific sound across the network originating from the passed in gameobject;
    [Command]
    public void CmdTellServerPlaySound(GameObject noiseMaker, string sound)
    {
        noiseMaker.GetComponent<SoundEffects>().PlayEffect(sound);
    }
    
    //Creates the passed in object at the passed in location and rotation across the network
    [Command]
    public void CmdCreateObject(GameObject toBeCreated, Vector3 createPos, Quaternion createRot)
    {
        GameObject theCreate = (GameObject)Instantiate(toBeCreated, createPos, createRot);
        NetworkServer.Spawn(theCreate);
    }
    
    //Updates the abilities of this player object and ensures that the server understands
    //what abilities the player has
    [Command]
    public void CmdUpdateAbilities()
    {
        if (ability1 != null)
        {
            GameObject tempAb1 = (GameObject)Instantiate(ability1, transform.position, transform.rotation);
            NetworkServer.SpawnWithClientAuthority(tempAb1, gameObject);
            tempAb1.GetComponent<Ability>().user = gameObject;
        }
        if (ability2 != null)
        {
            GameObject tempAb2 = (GameObject)Instantiate(ability2, transform.position, transform.rotation);
            NetworkServer.SpawnWithClientAuthority(tempAb2, gameObject);
            tempAb2.GetComponent<Ability>().user = gameObject;
        }
        if (ability3 != null)
        {
            GameObject tempAb3 = (GameObject)Instantiate(ability3, transform.position, transform.rotation);
            NetworkServer.SpawnWithClientAuthority(tempAb3, gameObject);
            tempAb3.GetComponent<Ability>().user = gameObject;
        }
    }

}
