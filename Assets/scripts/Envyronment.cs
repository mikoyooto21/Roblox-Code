using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Envyronment : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;

    private List<Vector3> initialPositionsCircles = new List<Vector3>();

    private bool swotchDirection = false;

    public GameObject[] circles;
    public GameObject[] movingPlatforms;

    private void Start()
    {
        circles = GameObject.FindGameObjectsWithTag("Circle");
        movingPlatforms = GameObject.FindGameObjectsWithTag("MoveMe");

        foreach (GameObject circle in circles)
        {
            initialPositionsCircles.Add(circle.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Circle();
        movingPlatform();
    }

    private void Circle()
    {

        for (int i = 0; i < circles.Length; i++)
        {
            Vector3 rotationCenter = initialPositionsCircles[i];
            circles[i].transform.RotateAround(rotationCenter, Vector3.up, rotationSpeed * Time.deltaTime);

            // Залиште вертикальну компоненту позиції без змін
            float currentY = circles[i].transform.position.y;
            circles[i].transform.position = new Vector3(circles[i].transform.position.x, currentY, circles[i].transform.position.z);
        }
    }

    private void movingPlatform()
    {

        movingPlatforms = GameObject.FindGameObjectsWithTag("MoveMe");

        for (int i = 0; i < movingPlatforms.Length; i++)
        {
            Vector3 currentPositionOfChild1 = movingPlatforms[i].transform.GetChild(0).position;
            Vector3 currentPositionOfChild2 = movingPlatforms[i].transform.GetChild(1).position;

            if(swotchDirection == false)
            {
                movingPlatforms[i].transform.position = Vector3.MoveTowards(movingPlatforms[i].transform.position, movingPlatforms[i].transform.GetChild(0).position, 2f * Time.deltaTime);
            }
            else if (swotchDirection == true)
            {
                movingPlatforms[i].transform.position = Vector3.MoveTowards(movingPlatforms[i].transform.position, movingPlatforms[i].transform.GetChild(1).position, 2f * Time.deltaTime);
            }
            if (movingPlatforms[i].transform.position == movingPlatforms[i].transform.GetChild(0).position)
            {
                swotchDirection = true;
            }
            else if (movingPlatforms[i].transform.position == movingPlatforms[i].transform.GetChild(1).position)
            {
                swotchDirection = false;
            }

            movingPlatforms[i].transform.GetChild(0).position = currentPositionOfChild1;
            movingPlatforms[i].transform.GetChild(1).position = currentPositionOfChild2;
        }
    }
}
