using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bud{
    public float prob_die = 0.3f;
    public float prob_pause = 0.5f;
    Vector3 direction;
    public int age;
    int order;
    public Bud(Vector3 d, int a)
    {
        direction = d;
        age = a;
    }
    public void Set_Dead()
    {

    }
    public Vector3 get_direction()
    {
        return direction;
    }
    public void set_direction(Vector3 d)
    {
        direction = d;
    }
    public void add_age(int a)
    {
        age += a;
    }
}
