using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Opinion
{
    public FactionManager.factions faction;
    public GameObject factionObj;
    [Range(0,100)]public float opinion;

    public List<Modifier> modifiers = new List<Modifier>();

    public void AdjustOpinion()
    {
        if(modifiers.ToArray().Length > 0)
        {
            for (int i = 0; i < modifiers.ToArray().Length; i++)
            {
                if(modifiers[i].activated == false)
                {
                    if (modifiers[i].permanent)
                    {
                        opinion += modifiers[i].opinionChange;    
                    }

                    modifiers[i].activated = true;
                }
                else
                {
                    if (modifiers[i].permanent == false)
                    {
                        modifiers[i].lifeSpan -= 1;
                        opinion += modifiers[i].opinionChange;
                    }
                }


                if(modifiers[i].lifeSpan == 0 && modifiers[i].permanent == false)
                {
                    modifiers.RemoveAt(i);
                }

            }
        }
    }
}
