using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GovernmentTab : MonoBehaviour
{
    public static GovernmentTab instance;
    [Header("MACRO")]
    public Text orderTXT;
    public Image orderIMG;

    [Header("Ruler")]
    public Text rulerNameTXT;
    public Text rulerAgeTXT;
    public Text rulerCultureTXT;
    [Space]
    public Text milSkillTXT;
    public Text dipSkillTXT;
    public Text merSkillTXT;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public void UpdateINFO() // called in the InfoBar script
    {
        //totals
        orderTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().publicOrder.ToString("00.0");
        //  Image
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().publicOrder >= 50)
        {
            orderIMG.color = Color.green;
        }
        else orderIMG.color = Color.red;

        rulerNameTXT.text = "BLAKE NICHOLAS";
        rulerAgeTXT.text = "19";
        rulerCultureTXT.text = "Australian";

        milSkillTXT.text = "5";
        dipSkillTXT.text = "5";
        merSkillTXT.text = "5";
    }


}
