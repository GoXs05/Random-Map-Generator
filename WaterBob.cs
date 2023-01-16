using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBob : MonoBehaviour
{

    [SerializeField] private BoxCollider WaterCollider;
    [SerializeField] private float bobSpeed;
    [SerializeField] private Transform playerTransform;
    //[SerializeField] private Collider waterTrigger;
    
    private float startTime = 0f;


    private void Update()
    {
        
        if (playerTransform.position.y < 0)
        {
            WaterCollider.center = new Vector3(WaterCollider.center.x, SinValue(), WaterCollider.center.z);
        }

        else
            WaterCollider.center = new Vector3(WaterCollider.center.x, -0.35f, WaterCollider.center.z);
        
    }

    private float SinValue()
    {
        return 0.15f * (Mathf.Sin((Time.time - startTime - 0.15f) * bobSpeed) - 1.3f);
    }

    public void TimeValue()
    {
        startTime = Time.time;
    }

}
