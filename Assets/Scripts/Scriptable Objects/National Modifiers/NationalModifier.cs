using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New National Mod", menuName = "National Mod")]
public class NationalModifier : ScriptableObject
{
    public GameObject affectedArea;
    public int lifeSpan; //in months
    public enum possibleVariables
    {
        Culture,
        Development,
        PublicOrder,
        Treasury,
        TaxIncome,
        Merchants
    }
    public possibleVariables affectedVariable;

    public float change;


    public void TakeAction()
    {
        if(affectedVariable == possibleVariables.Culture)
        {
            CultureManager.instance.Conversion(affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>(), affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().culture);
        }
        else if(affectedVariable == possibleVariables.Development)
        {
            affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().development += change;
        }
        else if (affectedVariable == possibleVariables.PublicOrder)
        {
            affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().publicOrder += change;
        }
        else if (affectedVariable == possibleVariables.Treasury)
        {
            affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject.GetComponent<Faction>().treasury += change;
        }
        else if (affectedVariable == possibleVariables.TaxIncome)
        {
            affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().taxIncome += change;
        }
        else if (affectedVariable == possibleVariables.Merchants)
        {
            for (int i = Mathf.RoundToInt(change); i > 0; i--)
            {
                affectedArea.GetComponent<Settlement>().NewMerchant();
            }
        }
        lifeSpan -= 1;
    }

    public IEnumerator RemoveMod(float delay)
    {
        yield return new WaitForSeconds(delay);
        change = 0;
        GameObject faction = affectedArea.GetComponent<Settlement>().province.GetComponent<Tile>().ownerObject;
        faction.GetComponent<Faction>().nationalMods.RemoveAt(faction.GetComponent<Faction>().nationalMods.IndexOf(this));
        Destroy(this);
    }



}
