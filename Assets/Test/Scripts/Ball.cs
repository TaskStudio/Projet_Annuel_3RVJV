using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour
{
    
     private float nextActionTime = 0.0f;
     public float period = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


    void Update () {
        if (Time.time > nextActionTime ) {
            nextActionTime += period;
            if(EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Pointing to Ball");
            }
        }
    }
}
