using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Opinion
{
    public FactionManager.factions faction;
    [Range(0,100)]public float opinion;

    public List<Modifier> modifiers = new List<Modifier>();

    public void AdjustOpinion()
    {
        if(modifiers.ToArray().Length > 0)
        {
            foreach (Modifier mod in modifiers)
            {
                if (!mod.activated)
                {
                    opinion += mod.opinionChange;
                    mod.activated = true;
                }
                
                if (!mod.permanent) mod.lifeSpan -= 1;

                if (mod.lifeSpan == 0) modifiers.Remove(mod);
            }
        }
    }
}
