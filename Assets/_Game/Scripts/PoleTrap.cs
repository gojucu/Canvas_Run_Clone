using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleTrap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RowHead"))
        {
            FindObjectOfType<PlayerControl>().RemoveFromPoleTrap(other.gameObject);
        }
    }
}
