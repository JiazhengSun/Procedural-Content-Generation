using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour{
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 total_force = new Vector3(0,0,0);
    private Vector3 wander_force = new Vector3(0,0,0);
    private Vector3 center_force = new Vector3(0, 0, 0);
    private Vector3 collisionavoidance_force = new Vector3(0, 0, 0);
    private Vector3 velocitymatching_force = new Vector3(0, 0, 0);
    private GameObject[] trial = new GameObject[20];

    private float minVel = 0.5f;
    private float maxVel = 5.0f;

    public Vector3 GetVelocity(){ return velocity;}
    public Vector3 GetPosition() { return position; }
    public Vector3 GetWF() { return wander_force; }
    public void SetVelocity (Vector3 v) { velocity = v; }
    public void SetPosition (Vector3 p) { position = p; }
    public void SetWF (Vector3 f) { wander_force = f; }
    public void SetFC (Vector3 f) { center_force = f;}
    public void setCA (Vector3 f) { collisionavoidance_force = f; }
    public void SetVM (Vector3 f) { velocitymatching_force = f; }
    public void UpdateTrial(GameObject f, int index)
    {
        if(trial[index] != null) { Destroy(trial[index]); }
        trial[index] = f;
    }
    public Vector3 NewVelocity (float delT) // based on force, calcluate new vel, then RESET curr Vel
    {
        total_force = wander_force + center_force + collisionavoidance_force + velocitymatching_force;
        Vector3 newV = velocity + total_force * delT;
        if (newV.magnitude >= maxVel) { newV = newV.normalized * maxVel; }
        if (newV.magnitude <= minVel) { newV = newV.normalized * minVel; }
        SetVelocity(newV);
        return newV;
    }

    public void CheckWallHit()
    {
        if (position.x >= 20.0 || position.x <= -20.0)
        {
            Vector3 newVel = new Vector3(velocity.x * -1, velocity.y, velocity.z);
            SetVelocity(newVel);
        }
        if (position.y >= 20.0 || position.y <= -20.0)
        {
            Vector3 newVel = new Vector3(velocity.x, velocity.y * -1, velocity.z);
            SetVelocity(newVel);
        }
        if (position.z >= 20.0 || position.z <= -20.0)
        {
            Vector3 newVel = new Vector3(velocity.x, velocity.y, velocity.z * -1);
            SetVelocity(newVel);
        }
    }

}
