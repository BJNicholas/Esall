using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public Text title, description, dismiss;
    public float lifeSpan = 5f;
    public bool important = false;


    private void Start()
    {
        if (!important)
        {
            StartCoroutine(Dismiss());
            dismiss.text = "Dismiss";
        }
    }

    public void Peace()
    {
        DiploHub.instance.ProposePeace();
        description.text += " Thank you, I'm sure it will never come to this again...";

        DiploHub.instance.faction.GetComponent<AI_Faction>().CancelCurrentTask();

        Invoke("Dismiss", 1.5f);
    }

    public void Decline()
    {
        description.text += " You will regret this...";
        DiploHub.instance.faction.GetComponent<AI_Faction>().CancelCurrentTask();

        Invoke("Dismiss", 1.5f);
    }



    public IEnumerator Dismiss()
    {
        yield return new WaitForSecondsRealtime(lifeSpan);
        if(gameObject.activeInHierarchy) Destroy(gameObject);
    }

    public void CloseWindowButton()
    {
        if (gameObject.activeInHierarchy) Destroy(gameObject);
    }
}
