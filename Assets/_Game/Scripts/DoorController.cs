using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public enum DoorType { Width, Length };
    public DoorType doorType;
    public Image panelImage, topImage;
    public Color greenColor, redColor;
    public GameObject doorInside;
    public ParticleSystem destroyParticles;

    public TextMeshProUGUI levelText;


    public int addedLineCount = 2;
    public int addedBallCount=5;
    // Start is called before the first frame update
    void Start()
    {
        if (addedLineCount > 0&&addedBallCount>0)
        {
            if (doorType == DoorType.Width)
            {
                levelText.text = "+" + addedLineCount.ToString();
            }
            else if (doorType == DoorType.Length)
            {
                levelText.text = "+" + addedBallCount.ToString();
            }

            panelImage.color = greenColor;
            topImage.color = greenColor;
        }
        else
        {
            if (doorType == DoorType.Width)
            {
                levelText.text =  addedLineCount.ToString();
            }
            else if (doorType == DoorType.Length)
            {
                levelText.text =  addedBallCount.ToString();
            }
            panelImage.color = redColor;
            topImage.color = redColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //PlayerControlden arttır azalt ve destroy ettir.
            if (FindObjectOfType<PlayerControl>() != null)
            {
                if (doorType ==DoorType.Width)
                {
                    if (addedLineCount > 0)
                    {
                        FindObjectOfType<PlayerControl>().AddNumberOfRows(addedLineCount);
                    }
                    else if(addedLineCount<0)
                    {
                        FindObjectOfType<PlayerControl>().RemoveRowFromDoor(-addedLineCount);
                    }

                }else if (doorType == DoorType.Length)
                {
                    if (addedBallCount > 0)
                    {
                        FindObjectOfType<PlayerControl>().AddNumberOfBallsToBehind(addedBallCount);

                    }
                    else if (addedBallCount < 0)
                    {

                        FindObjectOfType<PlayerControl>().RemoveBallsFromDoor(-addedBallCount);
                    }
                }
            }
            DestroyDoor();
        }
    }

    public int GetLevel()
    {
        return addedLineCount;
    }

    public void DestroyDoor()
    {
        //TODO bunu effect ile birlikte yap
        //Close collide
        GetComponent<Collider>().enabled = false;
        if (destroyParticles != null && !destroyParticles.isPlaying) destroyParticles.Play();
        Destroy(doorInside);
    }
}
