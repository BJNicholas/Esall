using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifier")]
public class Modifier : ScriptableObject
{
    public bool activated = false;
    public new string name;
    public string description;

    public float opinionChange;

    public bool permanent;
    [Tooltip("Only relevant if the modifier is not permanent")]public int lifeSpan;
}
