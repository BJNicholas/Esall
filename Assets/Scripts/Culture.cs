using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Culture : MonoBehaviour
{
    public CultureManager.cultures culture;

    public Color32 colour;

    public string[] names;

    public Sprite portrait; // add more later

    public List<GameObject> cultureTiles;

    [Header("Name Placement Setup")]
    public GameObject cultureTxtObject;
    public Text cultureTxt;


    private void Awake()
    {
        foreach(GameObject tile in cultureTiles)
        {
            tile.GetComponent<Tile>().culture = culture;
        }
    }

    public void GenerateNamePlacement()
    {
        cultureTxtObject.SetActive(true);

        cultureTxt.text = culture.ToString();
        //gettting the furthest provinces
        if (cultureTiles.ToArray().Length > 1)
        {
            float maxDistance = 0;
            Vector3 pointA = Vector3.zero, pointB = Vector3.zero;
            Vector2 midPoint = Vector2.zero;
            foreach (GameObject tile in cultureTiles)
            {
                foreach (GameObject otherTile in cultureTiles)
                {
                    float distance = Vector2.Distance(tile.transform.position, otherTile.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        midPoint = (tile.transform.position + otherTile.transform.position) / 2;
                        if (tile.transform.position.x > otherTile.transform.position.x)
                        {
                            pointA = tile.transform.position;
                            pointB = otherTile.transform.position;
                        }
                        else
                        {
                            pointB = tile.transform.position;
                            pointA = otherTile.transform.position;
                        }
                    }
                }
            }
            //angle
            Vector3 vectorToTarget = pointA - pointB;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            cultureTxtObject.gameObject.transform.rotation = q;
            //placement and size
            cultureTxtObject.gameObject.transform.position = midPoint;
            cultureTxtObject.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxDistance);
        }
        else if (cultureTiles.ToArray().Length == 1) //only one tile
        {
            //placement
            cultureTxtObject.gameObject.transform.position = cultureTiles[0].transform.position;
            cultureTxtObject.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
        }
        else if (cultureTiles.ToArray().Length == 0) //no tiles left
        {
            gameObject.SetActive(false);
        }

    }


}
