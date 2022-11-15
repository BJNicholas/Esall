using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public Text title, description, change, dismiss;
    public Image image;
    public float lifeSpan = 5f;
    public bool important = false;

    public PossibleEvent PE;


    private void Start()
    {
        change.text = "";
        if (!important)
        {
            StartCoroutine(Dismiss());
            dismiss.text = "Dismiss";
        }
    }

    private void FixedUpdate()
    {
        if (PE != null)
        {
            PE.EventSetUp(gameObject);
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
