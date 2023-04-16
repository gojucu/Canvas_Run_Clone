using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravity : MonoBehaviour
{
    public float gravity = -9.81f; // varsayılan yerçekimi değeri

    private void Update()
    {
        // objeye yerçekimi uygulama
        Vector3 gravityVector = gravity * Vector3.up; // yerçekimi yönü
        GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
    }
}
