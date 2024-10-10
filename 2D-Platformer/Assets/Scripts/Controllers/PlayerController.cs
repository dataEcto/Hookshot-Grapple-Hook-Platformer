using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public override bool RetrieveJumpHoldInput()
    {
        //Return true if we are holding the jump button
        return Input.GetButton("Jump");
    }

    public override bool RetrieveJumpInput()
    {
        //Return True if Jump is pressed
        return Input.GetButtonDown("Jump");
    }

    public override float RetrieveMoveInput()
    {
        //Return the Horizontal Axis
        return Input.GetAxis("Horizontal");
    }
    public override float RetrieveUpDownInput()
    {
        // Vertical Axis
        return Input.GetAxis("Vertical");
    }

}
