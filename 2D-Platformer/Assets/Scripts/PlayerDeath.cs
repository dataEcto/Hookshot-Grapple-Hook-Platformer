using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If the player comes in contact with anything marked Death Zone, it will die
/// Dying does the following:
/// - Sets the Player's SPAWN
/// - Disables the Players Components
/// - Plays a Death Animation
/// - Sends an Event to the UI manager to play a Fade Out
/// - Respawns the Player
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    // We get our scripts to disable later
    // This really isn't the cleanest way to do this honestly
    // So I could switch to the new input system to disable input.
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
