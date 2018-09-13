using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Internode {
    public Node start_node;
    public Node end_node;

    public Internode(Node start, Node end)
    {
        start_node = start;
        end_node = end;
    }
}
