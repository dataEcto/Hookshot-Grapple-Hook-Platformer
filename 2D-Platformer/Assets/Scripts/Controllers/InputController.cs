using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// An abstract class that other scripts can derive input from.
/// Shinji uses this for Character and AI
/// I don't think I'll be using it for that, but I think this scriptable object opproach is interesting and a good start
/// </summary>
public abstract class InputController : ScriptableObject
{
    public abstract float RetrieveMoveInput();
    public abstract bool RetrieveJumpInput();
    public abstract bool RetrieveJumpHoldInput();
    public abstract float RetrieveUpDownInput();
}
