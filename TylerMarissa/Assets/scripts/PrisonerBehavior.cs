using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerBehavior : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager gameManagerScript;
    public void Start()
    {
        gameManager = GameObject.Find("GameController");
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }
    public void FreePrisoner() {
        gameObject.SetActive(false);
        print("im free!!");
        gameManagerScript.prisonersFreed++;
    }
}
