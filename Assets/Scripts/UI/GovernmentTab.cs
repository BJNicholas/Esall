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
    public Image rulerPortrait;
    [Space]
    public Text milSkillTXT;
    public Text dipSkillTXT;
    public Text merSkillTXT;

    [Header("Graph")]
    public UILineRenderer line;
    public UIGridRenderer grid;


    [Header("National Mods")]
    public List<GameObject> nationalModsList;
    public GameObject nationalModUI_prefab;
    public GameObject scrollViewContent;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public void UpdateINFO() // called in the InfoBar script
    {
        //totals
        orderTXT.text = GameManager.instance.playerFactionObject.GetComponent<Faction>().publicOrder.ToString("0");
        //  Image
        if (GameManager.instance.playerFactionObject.GetComponent<Faction>().publicOrder >= 80)
        {
            orderIMG.color = Color.green;
        }
        else orderIMG.color = Color.red;

        GameObject ruler = GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler;

        rulerNameTXT.text = ruler.GetComponent<Character>().name;
        rulerAgeTXT.text = ruler.GetComponent<Character>().age.ToString("0");
        rulerCultureTXT.text = ruler.GetComponent<Character>().culture.ToString();
        rulerPortrait.sprite = ruler.GetComponent<Character>().portrait;    

        milSkillTXT.text = ruler.GetComponent<Character>().mil.ToString();
        dipSkillTXT.text = ruler.GetComponent<Character>().dip.ToString();
        merSkillTXT.text = ruler.GetComponent<Character>().mer.ToString();
    }

    public void UpdateNationalModsList()
    {
        //destory old list (just in case)
        for (int i = 0; i < nationalModsList.ToArray().Length; i++)
        {
            Destroy(nationalModsList[i]);
        }

        //populate new mods list
        foreach(NationalModifier mod in GameManager.instance.playerFactionObject.GetComponent<Faction>().nationalMods)
        {
            GameObject ModUI = Instantiate(nationalModUI_prefab, scrollViewContent.transform);
            ModUI.GetComponent<ModUI>().mod = mod;
            nationalModsList.Add(ModUI);
        }
    }

    public void UpdateGraph(float currentPO)
    {
        grid.SetAllDirty();
        line.SetAllDirty();

        if (line.points.ToArray().Length < grid.gridSize.x + 1)
        {
            Vector2 newLine = new Vector2(line.points.ToArray().Length, currentPO);
            line.points.Add(newLine);
        }
        else
        {
            line.points.Remove(line.points[0]);
            //loat largest = 0;
            for (int i = 0; i < line.points.ToArray().Length; i++)
            {
                line.points[i] = new Vector2(line.points[i].x - 1, line.points[i].y);
                //if (line.points[i].y > largest) largest = line.points[i].y;
            }
            Vector2 newLine = new Vector2(line.points.ToArray().Length, currentPO);
            line.points.Add(newLine);
            //grid.gridSize.y = Mathf.RoundToInt(largest);
        }

        //colouring
        if (currentPO >= 50)
        {
            line.color = Color.green;
        }
        else line.color = Color.red;
        if (currentPO > grid.gridSize.y)
        {
            grid.gridSize.y = Mathf.RoundToInt(currentPO);
        }
        grid.SetAllDirty();
        line.SetAllDirty();
    }


}
