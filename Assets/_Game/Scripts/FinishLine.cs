using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    bool firstPassed;
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
        if(other.CompareTag("Ball") && other.gameObject.GetComponent<Ball>())
        {
            other.gameObject.GetComponent<Ball>().SetBallAsFinished();

            if (!firstPassed)
            {
                Invoke("FinishGame", 4f);
                firstPassed = true;
            }
        }
    }
    void FinishGame()
    {
        FindObjectOfType<PlayerControl>().finished = true;
        Debug.Log("Game is finished");
    }
}
