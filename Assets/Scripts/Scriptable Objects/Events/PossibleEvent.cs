using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Event", menuName = "Event")]
public class PossibleEvent : ScriptableObject
{
    public string title;
    public string description;
    public string changeText;
    public Sprite image;

    public string dismissText;

    public NationalModifier modifier;

    public void EventSetUp(GameObject eventPopUp)
    {
        eventPopUp.GetComponent<Event>().dismiss.text = dismissText;
        eventPopUp.GetComponent<Event>().change.text = modifier.affectedArea.name + " " + changeText;
    }
}
