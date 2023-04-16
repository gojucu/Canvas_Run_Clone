using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool isAtFinish;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool GetFinishValue()
    {
        return isAtFinish;
    }

    public void SetBallAsFinished()
    {
        isAtFinish = true;
        GetComponent<Rigidbody>().constraints =RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddForce(Vector3.forward*50,ForceMode.Force);
        GetComponent<Rigidbody>().AddForce(Vector3.up*50, ForceMode.Force);

        Rigidbody rb = GetComponent<Rigidbody>();
        float angle = Random.Range(-45, 45);
        Vector3 force = Quaternion.Euler(0, angle, 0) * transform.forward * 100;
        rb.AddForce(force);
    }
}
