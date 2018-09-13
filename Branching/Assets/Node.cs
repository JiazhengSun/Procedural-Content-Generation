using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public Vector3 position;
    ArrayList bud_list = new ArrayList();
    public Bud node_bud;
    public int node_length = 2;
    public int min_bud = 1;
    public int max_bud = 5;
    private bool curveUpward;
    private bool tropical;
    public Node (Bud bud, Vector3 pos, bool curve, bool trop)
    {
        node_bud = bud;
        position = pos;
        bud_list.Add(bud);
        curveUpward = curve;
        tropical = trop;
    }

    public void Create_TipBud(Bud bud)
    {
        //bud.age += 1;
        bud_list.Add(bud);
    }

    public void Create_SideBud(Bud bud, bool curveUpward, float k,bool trop)
    { 
        Vector3 curr_direction = bud.get_direction();
        int bud_num = Random.Range(min_bud, max_bud);
        float angle = 360 / bud_num;
        int curr_age = bud.age;
        if (trop == false)
        {
            if (curveUpward == true)
            {
                for (int i = 0; i < bud_num; i++)
                {
                    Vector3 new_direction = Quaternion.Euler(15.0f, angle * i, 15.0f) * curr_direction;
                    Vector3 u = new Vector3(0.0f, 1.0f, 0.0f);
                    new_direction += u * k;
                    new_direction = new_direction.normalized;
                    Bud new_bud = new Bud(new_direction, curr_age + 3);
                    bud_list.Add(new_bud);
                }
            }
            else
            {
                for (int i = 0; i < bud_num; i++)
                {
                    Vector3 new_direction = Quaternion.Euler(15.0f, angle * i, 15.0f) * curr_direction;
                    Vector3 H = new Vector3(new_direction.x, 0.0f, new_direction.z);
                    H = H.normalized;
                    new_direction += k * H;
                    new_direction = new_direction.normalized;
                    Bud new_bud = new Bud(new_direction, curr_age + 2);
                    bud_list.Add(new_bud);
                }
            }
        }
        else
        {
            for (int i = 0; i < bud_num; i++)
            {
                Vector3 new_direction = Quaternion.Euler(15.0f, angle * i, 15.0f) * curr_direction;
                //Vector3 u = new Vector3(0.0f, 1.0f, 0.0f);
                //new_direction += u * k;
                //new_direction = new_direction.normalized;
                Bud new_bud = new Bud(new_direction.normalized, curr_age + 3);
                bud_list.Add(new_bud);
            }
        }



    }

    public void Bud_Growth(ArrayList node_list, ArrayList internode_list, float k,bool trop)
    {
        for(int i = 0; i < bud_list.Count; i++)
        {
            Bud bud = (Bud) bud_list[i];
            int curr_age = bud.age;
            if (Random.value < bud.prob_die)
            {
                bud.Set_Dead();
            }
            else if (Random.value > bud.prob_pause)
            {
                Vector3 direction = bud.get_direction();
                Vector3 new_position = position + node_length * direction;
                Bud new_bud = new Bud(direction, curr_age + 1);
                Node end_node = new Node(new_bud, new_position,curveUpward,trop);
                node_list.Add(end_node);
                internode_list.Add(new Internode(this, end_node));
                end_node.Create_SideBud(bud,curveUpward,k,trop);
                end_node.Create_TipBud(bud);
            }
        }
    }
}
