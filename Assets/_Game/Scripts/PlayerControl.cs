using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RowsClass
{
    public List<Transform> ballPositions = new List<Transform>();
}
public class PlayerControl : MonoBehaviour
{
    public Rigidbody playerRb,ballBaseRb;

    private Camera fixedCamera;

    public GameObject ballPrefab;
    public GameObject rowHeadPrefab;
    public Transform playerBall;


    [Header("BallFollow")]
    public float ballDistance = 1f;
    public float ballDistanceRow = 1f;
    public float followSpeed = 30f;

    [Header("Movement")]
    public float movementSpeed = 9f;
    public float slidingSpeed = 15f;

    public float playerBoundaryLeft = -5f;
    public float playerBoundaryRight = 5f;

    Vector3 firstTouchPos;
    Vector3 firstPlayerPos;

    [Header("Row extending")]
    [SerializeField]
    private List<RowsClass> rows = new List<RowsClass>();
    public int rowCount = 1;
    float extendSpeed = 10f;



    float currentRotation;
    public float rotSpeed = 100f;
    public float resetRotationSpeed = 5f;
    Quaternion defaultWagonRotation;

    public Gradient gradientBalls;
    bool started;
    public bool finished;

    bool crashed = false;
    float crashFixTimer;
    float crashTime = .6f;
    private void Awake()
    {
        fixedCamera = Camera.main;
    }
    void Start()
    {
        defaultWagonRotation = ballBaseRb.transform.rotation;

        AddNumberOfRows(5);

        AddNumberOfBallsToBehind(32);

    }

    private void FixedUpdate()
    {
        MoveSnake();
        MoveForward();
        MoveLeftRight();
    }
    private void Update()
    {
        if (!started && Input.GetMouseButtonDown(0))
        {
            started = true;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(RowsClass row in rows)
            {
                AddBall(row,true);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddRow();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RemoveBallsFromDoor(1);
            //RemoveRowFromDoor(1);
            //AddSingleBall();
        }

        if (crashed)
        {
            crashFixTimer += Time.deltaTime;
            if (crashFixTimer >= crashTime)
            {
                SetAllHeadPositions();
                crashed = false;
            }
        }

    }
    public void AddNumberOfRows(int count)
    {
        for(int i = 0; i < count; i++)
        {
            AddRow();
        }
    }
    void AddRow()
    {
        //0 dan başlatıp tek çiftlik durumuna göre sağa sola lerplet uzunluk kadar topta ekle.
        int minCount = 1;
        if (rows.Count > 0)
        {
            minCount = rows.Min(x => x.ballPositions.Count)-1;
        }

        RowsClass newRow = new RowsClass();
        rows.Add(newRow);
        float i = rows.Count;
        float x;
        if (i % 2 == 1)
        {
            x = -1 * (i / 2);
        }
        else
        {
            x = (i / 2) - (ballDistanceRow / 2);
        }
        Vector3 rowPos =new Vector3(x,0,0);
        GameObject rowHead = Instantiate(rowHeadPrefab, ballBaseRb.transform);
        rowHead.transform.localPosition = new Vector3(0,0,0);

        newRow.ballPositions.Add(rowHead.transform);


        for (int j = 0; j < minCount; j++)
        {
            AddBall(newRow,false);
        }
        StartCoroutine(LerpPositionCoroutine(rowHead, rowPos));
        //SetAllHeadPositions();
    }
    IEnumerator LerpPositionCoroutine(GameObject go,Vector3 pos)
    {
        float timeElapsed = 0;
        while (timeElapsed<.5f)
        {
            timeElapsed += Time.deltaTime;
            go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, pos, timeElapsed/.5f);
            yield return null;
        }
        go.transform.localPosition = pos;
    }
    void MoveForward()
    {
        if (started && !finished)
        {
            playerRb.gameObject.transform.position += Vector3.forward * Time.deltaTime * movementSpeed;
        }
    }
    private void MoveLeftRight()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstTouchPos = fixedCamera.ScreenToWorldPoint(Input.mousePosition - new Vector3(0, 0, 1));
            firstPlayerPos = ballBaseRb.transform.localPosition;

        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 movementVector = fixedCamera.ScreenToWorldPoint(Input.mousePosition - new Vector3(0, 0, 1)) - firstTouchPos;

            float finalXPos = firstPlayerPos.x - movementVector.x * slidingSpeed;
            finalXPos = Mathf.Clamp(finalXPos, playerBoundaryLeft, playerBoundaryRight);

            ballBaseRb.transform.localPosition = Vector3.Lerp(ballBaseRb.transform.localPosition, new Vector3(finalXPos, ballBaseRb.transform.localPosition.y, ballBaseRb.transform.localPosition.z), Time.fixedDeltaTime * 10f);

            RotateHead();
        }
        else
        {
            ResetRotation();
        }
    }

    private void RotateHead()
    {
        float dir = Input.GetAxis("Mouse X");
        if (dir == 0)
        {
            currentRotation = Mathf.Lerp(currentRotation, 0, resetRotationSpeed * Time.deltaTime);
        }
        float newRot = currentRotation + dir * rotSpeed * Time.deltaTime;
        currentRotation = Mathf.Clamp(newRot, -20, 20);
        ballBaseRb.transform.localRotation = Quaternion.Euler(0, newRot, 0);
    }

    private void ResetRotation()
    {
        currentRotation = 0;
        ballBaseRb.transform.localRotation = Quaternion.Lerp(ballBaseRb.transform.localRotation, defaultWagonRotation, Time.deltaTime * resetRotationSpeed);
    }

    void MoveSnake()
    {
        foreach (RowsClass row in rows)
        {
            for (int i = 1; i < row.ballPositions.Count; i++)
            {
                //float newXPos = row.ballPositions[i - 1].transform.position.x;
                // float newZPos = row.ballPositions[i - 1].transform.forward * ballDistance;
                if (!row.ballPositions[i].GetComponent<Ball>().GetFinishValue())
                {
                    Vector3 newVect = row.ballPositions[i - 1].transform.position - row.ballPositions[i - 1].transform.forward * ballDistance;

                    Vector3 newTransformPos = new Vector3(newVect.x, row.ballPositions[i].transform.position.y, newVect.z);

                    if (i != 0)
                        row.ballPositions[i].transform.position = Vector3.Lerp(row.ballPositions[i].transform.position, newTransformPos, Time.fixedDeltaTime * followSpeed);
                }
            }
        }
    }


    public void AddNumberOfBallsToBehind(int count)
    {
        for(int i=0;i< count; i++)
        {

            // Find the RowsClass object with the smallest number of balls
            RowsClass smallestRow = rows.OrderBy(x => x.ballPositions.Count).First();
            AddBall(smallestRow,true);
        }

    }
    void AddBall(RowsClass row,bool fromBehind)
    {
        Vector3 newBallPosition;
        if (fromBehind)
        {
            newBallPosition = row.ballPositions[row.ballPositions.Count - 1].position - (row.ballPositions[0].transform.forward * ballDistance * 50);
        }
        else
        {
            newBallPosition = row.ballPositions[row.ballPositions.Count - 1].position - (row.ballPositions[0].transform.forward * ballDistance );
        }

        GameObject newBall = Instantiate(ballPrefab, newBallPosition, Quaternion.identity, transform);
        Transform newBallTransform = newBall.transform;
        row.ballPositions.Add(newBallTransform);
        SetBallColorForNewBall();
    }

    private void SetBallColorForNewBall()
    {

        for (int i = 0; i < rows.Count; i++)
        {
            RowsClass row = rows[i];

            for (int j = row.ballPositions.Count-1; j >=1 ; j--)
            {
                Transform ball = row.ballPositions[j];
                Renderer ballRenderer = ball.GetComponent<Renderer>();

                float t = (float)(j)/50f;
                ballRenderer.material.color = gradientBalls.Evaluate(t);
            }
        }
    }
    //void AddSingleBall()
    //{
    //    AddNumberOfBallsToBehind(12);
    //}


    public void RemoveFromPoleTrap(GameObject rowHead)
    {
        RowsClass crashedRow = rows.FirstOrDefault(x => x.ballPositions[0] == rowHead.transform);
        Vector3 rowPos = rowHead.transform.localPosition;
        foreach (Transform ballTransform in crashedRow.ballPositions)
        {
            Destroy(ballTransform.gameObject);
        }
        //int destroyedIndex = rows.IndexOf(crashedRow);

        //List<RowsClass> oddIndexedRows = rows.Where((row, index) => index % 2 == 1).ToList();

       // int rowCount = rows.Count;

        rows.Remove(crashedRow);

        crashed = true;
        crashFixTimer = 0;
        //Invoke("SetAllHeadPositions", .7f);

        //StartCoroutine(LerpPositionCoroutine(rowHead, rowPos));

    }
    void SetAllHeadPositions()
    {
        for (int j = rows.Count; j > 0; j--)
        {
            float x;
            if (j % 2 == 1)
            {
                x = -1 * ((float)j / 2);
            }
            else
            {
                x = ((float)j / 2) - (ballDistanceRow / 2);
            }
            Vector3 rowPos = new Vector3(x, 0, 0);
            GameObject rowHead = rows[j-1].ballPositions[0].gameObject;
            StartCoroutine(LerpPositionCoroutine(rowHead, rowPos));
        }
    }
    public void RemoveRowFromDoor(int count)
    {
        if (rows.Count <= count)
        {
            Debug.Log("Game over");
            return;
        }
        for(int i = 1; i <= count; i++)
        {
            int indexToRemove = rows.Count - 1;
            RowsClass lastRow = rows[indexToRemove];

            foreach (Transform ballTransform in lastRow.ballPositions)
            {
                Destroy(ballTransform.gameObject);
            }

            rows.RemoveAt(rows.Count - 1);
        }
    }

    public void RemoveBallsFromDoor(int count)
    {
        for (int i = 0; i < count; i++)
        {

            // Find the RowsClass object with the smallest number of balls
            RowsClass maxBallsRow = rows.OrderByDescending(r => r.ballPositions.Count).First();

            RemoveBall(maxBallsRow);
        }
    }

    private void RemoveBall(RowsClass maxBallsRow)
    {

        // Eğer en yüksek ball sayısı 0'dan büyük ise...
        if (maxBallsRow.ballPositions.Count > 0)
        {
            // Son ball'ı seç ve yok et
            Transform lastBall = maxBallsRow.ballPositions.Last();
            Destroy(lastBall.gameObject);

            // RowsClass listesinden son ball'ın bulunduğu sırayı çıkar
            maxBallsRow.ballPositions.Remove(lastBall);

            // Eğer sıradaki tüm toplar yok edildiyse, RowsClass'ı da listenin içinden çıkar
            if (maxBallsRow.ballPositions.Count == 0)
            {
                rows.Remove(maxBallsRow);
            }
        }
    }
}
