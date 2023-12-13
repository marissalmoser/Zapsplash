/**********************************************************************************

// File Name :         MainMenuBehavior.cs
// Author :            Marissa Moser
// Creation Date :     April 13, 2023
//
// Brief Description : Code for the main menu. Adds functions for the two buttons,
       play and quit, and sets the play button to be selected first.

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuBehavior : MonoBehaviour
{
    public GameObject PlayButton;
    public GameObject ControlButton;
    public GameObject X;
    public GameObject ControlScreen;

    /// <summary>
    /// Begins the game
    /// </summary>
    public void StartLevel()
    {
        SceneManager.LoadScene(1);
        //Debug.Log("Start Level");
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Enables the controls image
    /// </summary>
    public void ControlsOn()
    {
        ControlScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(X);
    }

    /// <summary>
    /// Enables the controls image
    /// </summary>
    public void ControlsOff()
    {
        ControlScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(ControlButton);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    /// <summary>
    /// Start function is used to set the buttoon that will be selected first.
    /// </summary>
    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(PlayButton);
    }
}
