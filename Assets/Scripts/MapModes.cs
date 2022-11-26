using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModes : MonoBehaviour
{
    public static MapModes instance;
    public enum modes
    {
        Political,
        Terrain,
        Development,
        Cultural,
        Order
    }
    public modes currentMode;

    [Header("Gradients")]
    public Gradient developmentGradient;
    public Gradient orderGradient;

    private void Awake()
    {
        instance = this;
        RenderMap(currentMode.ToString());
    }


    public void RenderMap(string mode)
    {
        StartCoroutine(mode);
    }

    private void Update()
    {
        RenderMap(currentMode.ToString());

        if (Input.GetKeyDown(KeyCode.Alpha1)) PolButton();
        else if (Input.GetKeyDown(KeyCode.Alpha2)) DevButton();
        else if (Input.GetKeyDown(KeyCode.Alpha3)) TerButton();
        else if (Input.GetKeyDown(KeyCode.Alpha4)) CulButton();
        else if (Input.GetKeyDown(KeyCode.Alpha5)) OrdButton();
    }

    public void ResetMapText()
    {
        if (currentMode != modes.Cultural)// set the text display to be faction text if not in cultural mode
        {
            foreach (GameObject Faction in FactionManager.instance.factionObjects)
            {
                Faction.GetComponent<Faction>().factionTxtObject.SetActive(true);
            }
            foreach (GameObject culture in CultureManager.instance.cultureObjects)
            {
                culture.GetComponent<Culture>().cultureTxtObject.SetActive(false);
            }
        }
    }

    public IEnumerator Political()
    {
        yield return new WaitForEndOfFrame();

        foreach (GameObject faction in FactionManager.instance.factionObjects)
        {
            foreach (GameObject tile in faction.GetComponent<Faction>().ownedTiles)
            {
                tile.GetComponent<SpriteRenderer>().color = faction.GetComponent<Faction>().colour;
            }
        }
        ResetMapText();
    }
    public IEnumerator Terrain()
    {
        yield return new WaitForEndOfFrame();
        foreach (GameObject tile in GameManager.instance.provinces)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        }
        ResetMapText();
    }
    public IEnumerator Development()
    {
        float most= 0, least = 100;
        foreach (GameObject tile in GameManager.instance.provinces)
        {
            if (tile.GetComponent<Tile>().development > most) most = tile.GetComponent<Tile>().development;
            if (tile.GetComponent<Tile>().development < least) least = tile.GetComponent<Tile>().development;
        }

        yield return new WaitForEndOfFrame();
        foreach (GameObject tile in GameManager.instance.provinces)
        {
            float point = tile.GetComponent<Tile>().development / most;
            tile.GetComponent<SpriteRenderer>().color = developmentGradient.Evaluate(point);
            Color32 tileColour = tile.GetComponent<SpriteRenderer>().color;
            tile.GetComponent<SpriteRenderer>().color = new Color32(tileColour.r, tileColour.g, tileColour.b, 150);
        }
        ResetMapText();
    }
    public IEnumerator Cultural()
    {
        yield return new WaitForEndOfFrame();
        foreach(GameObject Faction in FactionManager.instance.factionObjects)
        {
            Faction.GetComponent<Faction>().factionTxtObject.SetActive(false);
        }

        foreach(GameObject culture in CultureManager.instance.cultureObjects)
        {
            culture.GetComponent<Culture>().GenerateNamePlacement();
            foreach (GameObject tile in culture.GetComponent<Culture>().cultureTiles)
            {
                tile.GetComponent<SpriteRenderer>().color = culture.GetComponent<Culture>().colour;
            }
        }
    }
    public IEnumerator Order()
    {
        yield return new WaitForEndOfFrame();
        foreach (GameObject tile in GameManager.instance.provinces)
        {
            float point = tile.GetComponent<Tile>().publicOrder / 100;
            tile.GetComponent<SpriteRenderer>().color = orderGradient.Evaluate(point);
            Color32 tileColour = tile.GetComponent<SpriteRenderer>().color;
            tile.GetComponent<SpriteRenderer>().color = new Color32(tileColour.r, tileColour.g, tileColour.b, 150);
        }
        ResetMapText();
    }



    //functions for button use
    public void PolButton()
    {
        currentMode = modes.Political;
    }
    public void TerButton()
    {
        currentMode = modes.Terrain;
    }
    public void DevButton()
    {
        currentMode = modes.Development;
    }
    public void CulButton()
    {
        currentMode = modes.Cultural;
    }
    public void OrdButton()
    {
        currentMode = modes.Order;
    }
}
