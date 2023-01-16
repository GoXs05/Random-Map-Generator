using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaterBob : MonoBehaviour
{
    [SerializeField] WaterBob WaterBobScript;

    private void OnTriggerEnter(Collider waterTrigger) 
    {
        WaterBobScript.TimeValue();
    }
}
