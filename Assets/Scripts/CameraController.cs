using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject target;
    public float cameraSpeed;
    public float scrollSpeed;

    [Space]
    [Header("Timing")]
    public float resetDelay = 3; //in seconds
    float timer; //public for testing
    [Space]

    public Vector2 scrollBounds;

    private void Start()
    {
        timer = resetDelay;
    }

    private void Update()
    {
        CameraMovement();
        if(target != null)
        {
            //go back to player army
            if(timer >= resetDelay)
            {
                //go to target
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x, target.transform.position.y, -10), cameraSpeed * Time.deltaTime);
            }

        }
        else
        {
            //print("Searching for player");
            if(GameManager.instance.playerFactionObject != null)
            {
                target = GameManager.instance.playerFactionObject.GetComponent<Faction>().capitalCity;
            }
        }

        //scroll/zoom
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > scrollBounds.x)
        {
            Camera.main.orthographicSize -= 1 * scrollSpeed * Time.deltaTime; 
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < scrollBounds.y)
        {
            Camera.main.orthographicSize += 1 * scrollSpeed * Time.deltaTime;
        }
    }

    public void CameraMovement()
    {
        Vector3 inputs;
        inputs.x = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
        inputs.z = 0;

        transform.position += inputs * cameraSpeed *  Time.deltaTime;

        if (inputs == Vector3.zero) timer += 1 * Time.deltaTime; //if not moving
        else timer = 0;
    }

    public void FindPlayerArmy()
    {
        GameObject[] armies = GameObject.FindGameObjectsWithTag("Army");
        foreach(GameObject army in armies)
        {
            if(army.GetComponent<Army>().isPlayer == true)
            {
                target = army;
            }
        }
    }
}
