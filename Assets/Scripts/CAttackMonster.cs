using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAttackMonster : AMonster
{
    [SerializeField] private MonsterBehaviour defaultBehaviour;
    
    private MonsterBehaviour currentBehaviour;
    private void Update()
    {
        if (currentBehaviour == MonsterBehaviour.Sleep)
        {
            
        }
    }

    protected override void ReactToSound(SoundRay soundRay)
    {
        if (currentBehaviour == MonsterBehaviour.Sleep)
        {
            currentBehaviour = MonsterBehaviour.WakeUp;
        }
    }
}
