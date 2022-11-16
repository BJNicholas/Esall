using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public PossibleEvent RulerDeathEvent;

    public GameObject characterPrefab;

    public List<GameObject> characters;
    public List<GameObject> deadCharacters;


    private void Awake()
    {
        instance = this;
    }

    public GameObject CreateNewRandomCharacter(GameObject character)
    {
        character = Instantiate(characterPrefab, transform);
        character.GetComponent<Character>().GenerateRandomCharacter();
        characters.Add(character);
        return character;
    }

    public GameObject CreateNewCharacter(GameObject character, GameObject birthPlace)
    {
        character = Instantiate(characterPrefab, transform);
        character.GetComponent<Character>().GenerateCharacterFromBirthPlace(birthPlace);
        characters.Add(character);

        return character;
    }

    public void CharacterDeath(GameObject character)
    {
        if (character == GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler)
        {
            if (GameManager.instance.playerFactionObject.activeInHierarchy) // check to make sure we dont spawn an event after GAME OVER
            {
                PossibleEvent PE = Instantiate(RulerDeathEvent);
                GameObject newEvent = Instantiate(GameManager.instance.eventPrefab, GameObject.Find("UI").transform);
                EventFirer.instance.FillEventDetails(newEvent, PE);


                NationalModifier uniqueMod = Instantiate(PE.modifier);
                PE.modifier = uniqueMod;

                uniqueMod.affectedArea = GameManager.instance.playerFactionObject.GetComponent<Faction>().capitalCity;

                GameManager.instance.playerFactionObject.GetComponent<Faction>().nationalMods.Add(uniqueMod);

                GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler = null;
                GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler = CreateNewCharacter(GameManager.instance.playerFactionObject.GetComponent<Faction>().ruler, GameManager.instance.playerFactionObject.GetComponent<Faction>().capitalCity);
            }
        }
    }

    public void AjustLives() //called each month
    {
        foreach(GameObject character in characters)
        {
            character.GetComponent<Character>().lifeSpan -= 1;

            if (character.GetComponent<Character>().lifeSpan == 0)
            {
                deadCharacters.Add(character);
            }
        }
        foreach (GameObject character in deadCharacters)
        {
            if (characters.Contains(character))
            {
                characters.RemoveAt(characters.IndexOf(character));
                print(character.name + " HAS DIED AND IS REMOVED FROM THE LIVING");
                CharacterDeath(character);  
            }
        }
        for (int i = 0; i < deadCharacters.ToArray().Length; i++)
        {
            Destroy(deadCharacters[i]);
        }
        deadCharacters.Clear();
        characters.TrimExcess();
    }
}
