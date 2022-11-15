using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyTab : MonoBehaviour
{
    public static DiplomacyTab instance;

    public GameObject relationUI_Prefab;
    public List<GameObject> allRelationUI_Objects;

    [Header("The three types")]
    public Transform normalRelationsList;
    public Transform enemyRelationsList;
    public Transform allyRelationsList;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public void CreateNewRelationUI(Opinion relation, Transform relationType)
    {
        GameObject newRelationUI = Instantiate(relationUI_Prefab, relationType);
        newRelationUI.GetComponent<RelationUI>().faction = relation;
        allRelationUI_Objects.Add(newRelationUI);   
    }

    public void DestroyDeadFactionFromUI(GameObject faction)
    {
        int index = 0;
        foreach(GameObject relationUIObj in allRelationUI_Objects)
        {
            if(relationUIObj.GetComponent<RelationUI>().faction.factionObj == faction)
            {
                index = allRelationUI_Objects.IndexOf(relationUIObj);
            }
        }
        GameObject temp = allRelationUI_Objects[index];
        allRelationUI_Objects.RemoveAt(index);
        Destroy(temp);

    }

    public void SetRelationsToWar(GameObject faction)
    {
        foreach(GameObject relationUIObj in allRelationUI_Objects)
        {
            if(relationUIObj.GetComponent<RelationUI>().faction.factionObj == faction)
            {
                SetNewRelationType(relationUIObj, enemyRelationsList); 
            }
        }
    }

    public void SetRelationsToNormal(GameObject faction)
    {
        foreach (GameObject relationUIObj in allRelationUI_Objects)
        {
            if (relationUIObj.GetComponent<RelationUI>().faction.factionObj == faction)
            {
                SetNewRelationType(relationUIObj, normalRelationsList);
            }
        }
    }
    public void SetRelationsToAlly(GameObject faction)
    {
        foreach (GameObject relationUIObj in allRelationUI_Objects)
        {
            if (relationUIObj.GetComponent<RelationUI>().faction.factionObj == faction)
            {
                SetNewRelationType(relationUIObj, allyRelationsList);
            }
        }
    }

    public void SetNewRelationType(GameObject relationObj,Transform newType)
    {
        relationObj.transform.parent = newType;
    }

}
