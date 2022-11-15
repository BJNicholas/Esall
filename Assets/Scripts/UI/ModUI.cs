using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModUI : MonoBehaviour
{
    public NationalModifier mod;
    public Text description;

    public void Start()
    {
        if(mod.change > 0)
        {
            description.text = mod.affectedVariable + " + " + mod.change.ToString();
        }
        else description.text = mod.affectedVariable + " " + mod.change.ToString();
    }


    private void FixedUpdate()
    {
        if(mod == null)
        {
            Destroy(gameObject);
        }
    }
}
