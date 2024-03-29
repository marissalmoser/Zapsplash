/**********************************************************************************

// File Name :         PlayerBehavior.cs
// Author :            Marissa Moser
// Creation Date :     April 13, 2023
//
// Brief Description : Script for the player. Sets up all the actions ans interaction
        such as interact and attack.

**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    private float movementSpeed = 7f;
    private Vector2 movement;
    private Vector2 rotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSpeed1;
    [SerializeField] private Rigidbody2D rb2D;

    private GameObject otherPlayer;

    private InputActionAsset inputAsset;
    private InputActionMap inputMap;
    private InputAction move, interact, attack, pause, aim;

    private DoorBehavior doorScript;
    private ButtonBehavior buttonScript;
    private CellDoorBehavior cellDoorScript;

    [SerializeField] private bool inDoorRange, touchingButton, inCellDoorRange;
    public bool stunned;

    [SerializeField] private GameObject playerAmmo;
    
    [SerializeField] private GameObject ammoSpawn;

    public bool IsElePlayer;
    private bool inPlayerRange = false;

    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private Image pauseMenu;

    /// <summary>
    /// Sets bools to begin as false.
    /// </summary>
    private void Start()
    {
        inDoorRange = false;
        touchingButton = false;
        inCellDoorRange = false;
        stunned = false;
        if (IsElePlayer)
        {
            otherPlayer = GameObject.Find("WaterPlayer(Clone)");
        }
        else {
            otherPlayer = GameObject.Find("ElectricPlayer(Clone)");
        }
        //print(otherPlayer);
    }

    /// <summary>
    /// Assigns input assets and actions to references
    /// </summary>
    void Awake()
    {
        //assigns input assets and actions to references
        inputAsset = this.GetComponent<PlayerInput>().actions;
        inputMap = inputAsset.FindActionMap("PlayerActions");
        move = inputMap.FindAction("Move");
        interact = inputMap.FindAction("Interact");
        attack = inputMap.FindAction("Attack");
        pause = inputMap.FindAction("ResetLevel");
        aim = inputMap.FindAction("Aim");

        //sets the movement velocity for the players
        move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        move.canceled += ctx => movement = Vector2.zero; //(0,0)

        //allows you to interact with objects you are in rage of
        interact.performed += ctx => Interact();

        //players attack function
        attack.performed += ctx => Attack();

        //reset level
        pause.performed += ctx => OpenPauseMenu();

        //rotates player
        aim.performed += ctx => rotation = ctx.ReadValue<Vector2>();
        aim.canceled += ctx => rotation = Vector2.zero;

    }

    /// <summary>
    /// Function called from DoorBehavior to move the players to their new position.
    /// </summary>
    public void MovePlayer(Vector2 targetLoc)
    {
        transform.position = targetLoc;
    }

    /// <summary>
    /// Fixt update funcition actually moves and rotates the player 
    /// </summary>
    void FixedUpdate()
    {
        if(!stunned)
        {
            //movement
            Vector2 movementVelocity = new Vector2(movement.x, movement.y) * movementSpeed * Time.deltaTime;
            transform.Translate(movementVelocity, Space.World);


            //rotation of the player during movement. Only if movement is not 0 and rotation is 0
            if (movement != Vector2.zero && rotation == Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movement);
                Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                rb2D.MoveRotation(rotation);
            }
            //when aim stick is moved ever
            if (rotation != Vector2.zero)
            {
                Quaternion targetRotation1 = Quaternion.LookRotation(transform.forward, rotation);
                Quaternion rotation1 = Quaternion.RotateTowards(transform.rotation, targetRotation1, rotationSpeed1 * Time.deltaTime);
                rb2D.MoveRotation(rotation1);
            }
        }
    }

    /// <summary>
    ///Update function checks if player is stunned and plays particals if they are 
    /// </summary>
    private void Update()
    {
        if(stunned)
        {
            var em = ps.emission;
            em.enabled = true;
        }
        else
        {
            var em = ps.emission;
            em.enabled = false;
        }
    }

    /// <summary>
    /// When the player presses A to interact, does a variety of actions
    ///     depending on the bools that are true.
    /// </summary>
    public void Interact()
    {
        if (inDoorRange)
        {
            doorScript.GoThroughDoor(gameObject, otherPlayer);
        }
        if (touchingButton)
        {
            buttonScript.PressButton();
        }
        if (inCellDoorRange)
        {
            cellDoorScript.OpenDoor();
        }
        if(inPlayerRange)
        {
            //stunned = false;
        }

        if(IsElePlayer)
        {
            anim.SetBool("EleInteract", true);
            Invoke("EndEleInteractBool", 0.3f);
        }
        if (!IsElePlayer)
        {
            anim.SetBool("WaterInteract", true);
            Invoke("EndWaterInteractBool", 0.3f);
        }
  
    }

    /// <summary>
    /// Functions used to end the interacting animation
    /// </summary>
    private void EndEleInteractBool()
    {
        anim.SetBool("EleInteract", false);
    }
    private void EndWaterInteractBool()
    {
        anim.SetBool("WaterInteract", false);
    }

    /// <summary>
    /// Spawns ammo when the player uses the trigger
    /// </summary>
    private void Attack()
    {
        if(!stunned)
        {
            Instantiate(playerAmmo, ammoSpawn.transform.position, transform.rotation);
            PlayAttackSound();
        }
    }

    /// <summary>
    /// Opens the pause menu when player presses the start button
    /// </summary>
    private void OpenPauseMenu()
    {
        if(!pauseMenu.enabled)
        {
            pauseMenu.enabled = !pauseMenu.enabled;
            //stop players and enemies 
        }

    }

    /// <summary>
    /// Sets bools for the Interact action. 
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Door")
        {
            inDoorRange = true;
            doorScript = collision.gameObject.GetComponent<DoorBehavior>();
        }
        if (collision.gameObject.tag == "Button")
        {
            touchingButton = true;
            buttonScript = collision.gameObject.GetComponent<ButtonBehavior>();
        }
        if (collision.gameObject.tag == "CellDoor")
        {
            inCellDoorRange = true;
            cellDoorScript = collision.gameObject.GetComponent<CellDoorBehavior>();
        }
    }

    /// <summary>
    /// Sets bools for the Interact action. 
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Door")
        {
            inDoorRange = false;
        }
        if (collision.gameObject.tag == "Button")
        {
            touchingButton = false;
        }
        if (collision.gameObject.tag == "CellDoor")
        {
            inCellDoorRange = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (!stunned)
            {
                FindObjectOfType<AudioManager>().Play("stunned");
            }
            stunned = true;
        }
        if (collision.gameObject.tag == "Player")
        {
            if (stunned)
            {
                FindObjectOfType<AudioManager>().Play("unstunned");
            }
            stunned = false;
        }
    }

    /// <summary>
    /// Determines which lpayer this script is on, and then plays a random sound
    ///     effect for that player when they attack.
    /// </summary>
    void PlayAttackSound()
    {
        if (IsElePlayer)
        {
            PlayZapSound();
        }
        if (!IsElePlayer)
        {
            PlayWaterSound();
        }
    }
    private void PlayZapSound()
    {
        int i = UnityEngine.Random.Range(1, 3);

        switch (i)
        {
            case 1:
                FindObjectOfType<AudioManager>().Play("zap1");
                break;
            case 2:
                FindObjectOfType<AudioManager>().Play("zap2");
                break;
            case 3:
                FindObjectOfType<AudioManager>().Play("zap3");
                break;
        }
    }
    private void PlayWaterSound()
    {
        int i = UnityEngine.Random.Range(1, 3);

        switch (i)
        {
            case 1:
                FindObjectOfType<AudioManager>().Play("water1");
                break;
            case 2:
                FindObjectOfType<AudioManager>().Play("water2");
                break;
            case 3:
                FindObjectOfType<AudioManager>().Play("water3");
                break;
        }
    }

    /// <summary>
    /// turns action map on
    /// </summary>
    void OnEnable()
    {
        inputMap.Enable();
    }
    /// <summary>
    /// turns action map off
    /// </summary>
    void OnDisable()
    {
        inputMap.Disable();
    }
}
