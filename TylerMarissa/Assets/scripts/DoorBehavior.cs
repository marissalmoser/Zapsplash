/**********************************************************************************

// File Name :         DoorBehavior.cs
// Author :            Tyler Bouchard
// Creation Date :     April 13, 2023
//
// Brief Description : mves the players when they go through the doors

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private PlayerBehavior playerScript;
    private CameraBehavior camScript;
    private Vector2 doorLocation, targetLoc1, targetLoc2;

    [SerializeField] private float targetLoc1x = 0, targetLoc1y = 0;
    [SerializeField] private float targetLoc2x = 0, targetLoc2y = 0;
    [SerializeField] private float CamTargetLocX = 0, CamTargetLocY = 0;
    private void Start()
    {
        doorLocation = new Vector2(transform.position.x, transform.position.y);
        targetLoc1 = new Vector2(targetLoc1x, targetLoc1y);
        targetLoc2 = new Vector2(targetLoc2x, targetLoc2y);
        camScript = GameObject.Find("Main Camera").GetComponent<CameraBehavior>();
    }
    public void GoThroughDoor(GameObject player, GameObject otherPlayer) 
    {
        if (player.GetComponent<PlayerBehavior>().stunned || otherPlayer.GetComponent<PlayerBehavior>().stunned)
        {
            // display "HEY dont leave them behind :("
        }
        else 
        {
            playerScript = player.GetComponent<PlayerBehavior>();
            playerScript.MovePlayer(targetLoc1);
            playerScript = otherPlayer.GetComponent<PlayerBehavior>();
            playerScript.MovePlayer(targetLoc2);
            camScript.MoveCamera(20f * CamTargetLocX, 12f * CamTargetLocY);
            FindObjectOfType<AudioManager>().Play("doorOpens");
        }
        
    }
}
