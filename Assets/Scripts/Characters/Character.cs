using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public new string name;
    public Vector3 birthday;
    public float age;
    public int lifeSpan;

    [Space]

    public CultureManager.cultures culture;

    [Space]

    public Sprite portrait;

    [Header("Traits")]
    public int mil;
    public int dip;
    public int mer;


    public void GenerateRandomCharacter()
    {
        GenerateBirthday();
        GenerateCulture();
        GeneratePortrait();
        GenerateName();
        GenerateTraits();
    }

    public void GenerateCharacterFromBirthPlace(GameObject birthPlace)
    {
        GenerateBirthday();

        culture = birthPlace.GetComponent<Settlement>().province.GetComponent<Tile>().culture;

        GeneratePortrait();

        GenerateName();
        name += " of " + birthPlace.GetComponent<Settlement>().settlementName;
        gameObject.name = name;

        GenerateTraits();
    }


    public void GenerateName()
    {
        foreach(GameObject cultureObject in CultureManager.instance.cultureObjects)
        {
            if(cultureObject.GetComponent<Culture>().culture == culture)
            {
                name = cultureObject.GetComponent<Culture>().names[Random.Range(0, cultureObject.GetComponent<Culture>().names.Length)];
                break;
            }
        }

        gameObject.name = name;
    }

    public void GenerateBirthday()
    {
        birthday.x = Random.Range(1, 31);
        birthday.y = Random.Range(1, 12);
        birthday.z = Random.Range(330, GameManager.instance.date.z - 18);

        age = GameManager.instance.date.z - birthday.z;
        Mathf.RoundToInt(age);

        lifeSpan = Random.Range(1, 12);
    }

    public void GenerateCulture()
    {
        int roll = Random.Range(0, CultureManager.instance.cultureObjects.ToArray().Length);
        culture = CultureManager.instance.cultureObjects[roll].GetComponent<Culture>().culture;
    }

    public void GeneratePortrait()
    {
        GameObject cultureObject = null;

        foreach(GameObject culObj in CultureManager.instance.cultureObjects)
        {
            if(culObj.GetComponent<Culture>().culture == culture)
            {
                cultureObject = culObj;
                break;
            }
        }

        portrait = cultureObject.GetComponent<Culture>().portrait;
    }

    public void GenerateTraits()
    {
        mil = Random.Range(1, 6);
        dip = Random.Range(1, 6);
        mer = Random.Range(1, 6);
    }
}
