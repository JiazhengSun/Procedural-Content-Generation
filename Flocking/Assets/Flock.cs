using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
    public GameObject creature;
    public int numberOfCreature;
    public int seed;

    public bool wanderforcetoggle;
    public float wanderforceweight;

    public bool flockcentertoggle;
    public float flockcenterweight;

    public bool collisionavoidancetoggle;
    public float collisionavoidanceweight;

    public bool velocitymatchingtoggle;
    public float velocitymatchingweight;

    public float mateRadius;
    public float repelRadius;
    public float velmatchRadius;
    public float rotationSpeed;

    public bool trailToggle;
    public Color trailColor;

    private ArrayList creature_list;
    private ArrayList neighbour_list;
    private ArrayList repel_list;
    private ArrayList match_list;

    private int frame_num;

    // Use this for initialization
    void Start () {
        frame_num = 0;
        Random.InitState(seed);
        creature_list = new ArrayList();
        Vector3 pos, vel;
        for (int i = 0; i < numberOfCreature; i++)
        {
            pos = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
            vel = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
            
            GameObject fish = Instantiate(creature);
            fish.name = "Fish" + i;

            Creature fishScript = fish.GetComponent<Creature>();
            fishScript.SetPosition(pos); //set initial position
            fishScript.SetVelocity(vel); //set initial velocity

            creature_list.Add(fish);
        }

	}
	
	// Update is called once per frame
	void Update () {
        //If get space key, scatter the fish position
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject fish in creature_list)
            {
                Creature rs = fish.GetComponent<Creature>();
                Vector3 randomPos = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
                rs.SetPosition(randomPos);
                Vector3 randomVel = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
                rs.SetVelocity(randomVel);
            }
        }

        //The reglar update loop
        float delT = Time.deltaTime;
        neighbour_list = new ArrayList();
        repel_list = new ArrayList();
        match_list = new ArrayList();
        foreach (GameObject fish in creature_list) // Compute and save new (position)
        {
            Creature fishScript = fish.GetComponent<Creature>();

            neighbour_list = FindFlockMates(fish); //Get all the neighbour within a radius
            repel_list = FindRepelMates(fish); // Get all the too close neighbour, to be repelled
            match_list = FindMatchMates(fish);

            int neighbour_number = neighbour_list.Count; // FC part
            if (neighbour_number > 0 && flockcentertoggle == true) //There are flockmates around the current fish
            {
                Vector3 centroid = FindCentroid(neighbour_list, neighbour_number); // Get unweighted centroid
                Vector3 flock_cener = centroid - fishScript.GetPosition(); //Get flocking center force fc
                flock_cener *= flockcenterweight;
                fishScript.SetFC(flock_cener); //Set and save fc
            }

            int repel_num = repel_list.Count; // CA part
            if (repel_num > 0 && collisionavoidancetoggle == true)
            {
                Vector3 ca = new Vector3(0,0,0);
                foreach (GameObject repelFish in repel_list)
                {
                    Creature rs = repelFish.GetComponent<Creature>();
                    ca += (fishScript.GetPosition() - rs.GetPosition());
                }
                ca *= collisionavoidanceweight;
                fishScript.setCA(ca);
            }

            int match_num = match_list.Count; // VM part
            if (match_num > 0 && velocitymatchingtoggle == true)
            {
                Vector3 vm = new Vector3(0, 0, 0);
                foreach (GameObject vf in match_list)
                {
                    Creature vs = vf.GetComponent<Creature>();
                    vm += (vs.GetVelocity() - fishScript.GetVelocity());
                }
                vm *= velocitymatchingweight;
                fishScript.SetVM(vm);
            }

            // Get wandering force
            if (wanderforcetoggle == true)
            {
                Vector3 wander_force = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                wander_force *= wanderforceweight;
                fishScript.SetWF(wander_force); //save in the wandering force
            }

            Vector3 delP = fishScript.NewVelocity(delT) * delT; //Compute and update the velocity based on force and delTime
            Vector3 newP = fishScript.GetPosition() + delP; // Update new position and save it 
            fishScript.SetPosition(newP); // save the new position
            if (trailToggle == true) // trails
            {
                GameObject trail_ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                trail_ball.GetComponent<Renderer>().material.color = trailColor;
                trail_ball.transform.position = fishScript.GetPosition();
                trail_ball.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                fishScript.UpdateTrial(trail_ball, frame_num % 20); // append the position to trial
            }

            fish.transform.position = fishScript.GetPosition(); //Update gameobject position to new position
            fishScript.CheckWallHit(); // check hit with world box. If yes then negate the velocity

            fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, Quaternion.LookRotation( fishScript.GetVelocity()),
                rotationSpeed * Time.deltaTime);
            
        }
        frame_num += 1;
	}

    ArrayList FindFlockMates(GameObject curfish)
    {
        ArrayList myList = new ArrayList();
        Creature curScript = curfish.GetComponent<Creature>();
        Vector3 curPos = curScript.GetPosition();
        foreach (GameObject fish in creature_list)
        {
            Creature fishScript = fish.GetComponent<Creature>();
            Vector3 fishPos = fishScript.GetPosition();
            float dist = Vector3.Distance(curPos, fishPos);
            if (dist <= mateRadius)
            {
                myList.Add(fish);
            }
        }
        return myList;
    }

    ArrayList FindRepelMates(GameObject curfish)
    {
        ArrayList myList = new ArrayList();
        Creature curScript = curfish.GetComponent<Creature>();
        Vector3 curPos = curScript.GetPosition();
        foreach (GameObject fish in creature_list)
        {
            Creature fishScript = fish.GetComponent<Creature>();
            Vector3 fishPos = fishScript.GetPosition();
            float dist = Vector3.Distance(curPos, fishPos);
            if (dist <= repelRadius)
            {
                myList.Add(fish);
            }
        }
        return myList;
    }

    ArrayList FindMatchMates(GameObject curfish)
    {
        ArrayList myList = new ArrayList();
        Creature curScript = curfish.GetComponent<Creature>();
        Vector3 curPos = curScript.GetPosition();
        foreach (GameObject fish in creature_list)
        {
            Creature fishScript = fish.GetComponent<Creature>();
            Vector3 fishPos = fishScript.GetPosition();
            float dist = Vector3.Distance(curPos, fishPos);
            if (dist <= velmatchRadius)
            {
                myList.Add(fish);
            }
        }
        return myList;
    }


    Vector3 FindCentroid(ArrayList FishList, int num)
    {
        Vector3 centroid = new Vector3(0, 0, 0);
        foreach (GameObject fish in FishList)
        {
            Creature fs = fish.GetComponent<Creature>();
            centroid += fs.GetPosition();
        }
        centroid = centroid / num;
        return centroid;
    } 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(45,45,45));
    }
}
