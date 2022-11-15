using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationUI : MonoBehaviour
{
    public Opinion faction;
    [Header("UI setup")]
    public Image flag;
    public Text factionName, opinion;
    public Slider opinionSlider;
    public Image sliderFill;
    public Gradient opinionGradient;

    private void Start()
    {
        flag.sprite = faction.factionObj.GetComponent<Faction>().flag;
        factionName.text = faction.factionObj.name.ToString();
    }

    private void FixedUpdate()
    {
        opinionSlider.value = faction.opinion;
        sliderFill.color = opinionGradient.Evaluate(faction.opinion /100);
        opinion.text = faction.opinion.ToString();
    }
}
