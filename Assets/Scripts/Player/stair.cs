using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stair : MonoBehaviour
{
    public Collider collider;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnTriggerEnter(collider);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Stair")
        {
            Player.transform.position = Vector3.zero;
        }
    }
}
