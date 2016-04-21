
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private Vector3 centroid;
    public Vector3 Centroid { get { return centroid; } }

    private Vector3 flockDirection;
    public Vector3 FlockDirection { get { return flockDirection; } }

    private List<GameObject> flock;
    public List<GameObject> Flock { get { return flock; } }

    public int numFlockers;
    public int numObstacles;

    public GameObject dude;
    public GameObject target;
    public GameObject dudePrefab;
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;
    public GameObject triggerBox;

    private GameObject[] obstacles;
    public GameObject[] Obstacles
    {
        get { return obstacles; }
    }

    private GameObject[,] triggers;
    public GameObject[,] Triggers
    {
        get { return triggers; }
    }

    private GameObject[] walls;
    public GameObject[] Walls
    {
        get { return walls; }
    }

	public Camera[] cams;

	void Start () 
    {
        //Make a noodle (target)
        Vector3 pos = new Vector3(0, 4.0f, 0);
        Quaternion targetRot = Quaternion.Euler(-90, 0, 0);
        
        //Conversely, the below syntax also works
        //target = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;

        //Declare the lists~!
        flock = new List<GameObject>();
		walls = GameObject.FindGameObjectsWithTag ("Wall");
		
        target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);

        //Make a seeker dudes
        for (int i = 0; i < numFlockers; i++)
        {
            pos = new Vector3(Random.Range(-6, 6), Random.Range(15, 20), Random.Range(-6, 6));
            dude = (GameObject)Instantiate(dudePrefab, pos, Quaternion.identity);
            flock.Add(dude);

            //Set dudes' target
            //flock[i].GetComponent<Seeker>().seekerTarget = target;
        }


        //Make obstacles & keep them in an array
        for (int i = 0; i < numObstacles; i++)
        {
            pos = new Vector3(Random.Range(-30, 30), Random.Range(1.1f, 15), Random.Range(-30, 30));
            Quaternion rot = Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
            Instantiate(obstaclePrefab, pos, rot);
        }
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        //Set cam's target
        //Camera.main.GetComponent<SmoothFollow>().target = GameObject.Find("GameManagerGO").transform;

        //triggers = new GameObject[50,50];   //Note: The number 50 may be subject to change
        //pos = new Vector3(-25, 0, -25);
        //for(int i = 0; i < 25; i++)
        //{
        //    for(int j = 0; j < 25; j++)
        //    {
        //        triggers[i,j] = (GameObject)Instantiate(triggerBox, pos, Quaternion.identity);
        //        pos += new Vector3(0, 0, 2);
        //    }
        //    pos += new Vector3(2, 0, -50);
        //}

		cams [0].enabled = true;
		cams [1].enabled = false;
		cams [2].enabled = false;
	}
	

	void Update () 
    {
        CalcCentroid();
        CalcFlockDirection();

        CheckUserInput();

        //for (int i = 0; i < flock.Count; i++)
        //{
        //    //compare dist between target and dude
        //    //move target if dude is too close
        //    float dist = Vector3.Distance(target.transform.position, flock[i].transform.position);
        //
        //    //randomize the target's new position
        //    if (dist < 4)
        //    {
        //        do
        //        {
        //            target.transform.position = new Vector3(Random.Range(-30, 30), Random.Range(4f, 19f), Random.Range(-30, 30));
        //        }
        //        while (NearAnObstacle());
        //    }
        //}
        GameObject.Find("GameManagerGO").transform.position = centroid;
        GameObject.Find("GameManagerGO").transform.forward = flockDirection;
        //
        //GameObject.Find("Runestone(Clone)").transform.up = flockDirection;
	}

    bool NearAnObstacle()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            //check if target is too close to an obstacle
            if (Vector3.Distance(target.transform.position, obstacles[i].transform.position) < 5f)
                return true;
        }
        //if we get out of the loop and haven't returned true, it's false
        return false;
    }

    private void CalcCentroid()
    {
        centroid = Vector3.zero;
        for(int i = 0; i < flock.Count; i++)
        {
            centroid += flock[i].transform.position;
        }
        centroid /= flock.Count;
    }

    private void CalcFlockDirection()
    {
        flockDirection = Vector3.zero;
        for(int i = 0; i < flock.Count; i++)
        {
            flockDirection += flock[i].transform.forward;
        }
        flockDirection.Normalize();
    }

    private void CheckUserInput()
    {
		//Camera Controller
		if(Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Alpha1))
		{
			cams[0].enabled = true;
			cams [1].enabled = false;
			cams [2].enabled = false;
		}
		if(Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Alpha2))
		{
			cams[0].enabled = false;
			cams [1].enabled = true;
			cams [2].enabled = false;
		}
		if(Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.Alpha3))
		{
			cams[0].enabled = false;
			cams [1].enabled = false;
			cams [2].enabled = true;
		}

		//FlockController
		if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach(GameObject flocker in flock)
            {
                flocker.GetComponent<FlowFollower>().switchNum = 4;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (GameObject flocker in flock)
            {
                flocker.GetComponent<FlowFollower>().switchNum = 5;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (GameObject flocker in flock)
            {
                flocker.GetComponent<FlowFollower>().switchNum = 6;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            foreach (GameObject flocker in flock)
            {
                flocker.GetComponent<FlowFollower>().switchNum = 7;
            }
        }
    }

}
