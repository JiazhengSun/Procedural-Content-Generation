using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour {
    public ArrayList internode_list;
    public ArrayList node_list;
    public int growth_cap = 2;
    public int random_seed;
    public bool curveUpward;
    public float kValueForCurve;
    public float thicknessCoefficient;
    public bool NoTropicalImplementation;
    public GameObject flower;
    private float determine;
    // Use this for initialization
    void Start () {
        float k = kValueForCurve;
        internode_list = new ArrayList();
        node_list = new ArrayList();
        Random.InitState(random_seed);
        Bud start_bud = new Bud(new Vector3(0, 1, 0),1);
        Node start_node = new Node(start_bud, this.transform.position, curveUpward,NoTropicalImplementation);
        node_list.Add(start_node);

        for (int i = 0; i < growth_cap; i++)
        {
            int count = node_list.Count;
            for (int j = 0; j < count; j++)
            {
                ((Node)node_list[j]).Bud_Growth(node_list,internode_list, k,NoTropicalImplementation);
            }
        }
        foreach (Internode internode in internode_list)
        {
            int color_coe = internode.start_node.node_bud.age;
            draw_internode(internode, color_coe);
            if (flower != null)
            {
                if (internode.end_node.node_bud.age >= 5)
                {
                    Vector3 pos = internode.end_node.position + this.transform.position;
                    GameObject myFlower = Instantiate(flower, pos, Quaternion.identity) as GameObject;
                    myFlower.transform.parent = this.transform;
                    //myFlower.transform.rotation = Random.rotation;
                }
            }
        }

    }
	// Update is called once per frame
	void Update () {

	}

    private void draw_internode(Internode internode, int coe)
    {
        Vector3 startPos = internode.start_node.position + this.transform.position;
        Vector3 endPos = internode.end_node.position + this.transform.position;
        Bud bud = internode.start_node.node_bud;
        Vector3 T = bud.get_direction();
        Vector3 N = Quaternion.Euler(0.0f,0.0f,90.0f)*T;
        Vector3 B = Vector3.Cross(T,N);
        int start_age = internode.start_node.node_bud.age;
        int end_age = internode.end_node.node_bud.age;
        Mesh my_mesh = Create_My_Mesh(T.normalized, N.normalized, B.normalized, startPos, endPos, start_age, end_age);
        GameObject s = new GameObject("Textured Mesh");
        s.transform.position = this.transform.position;
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();
        s.GetComponent<MeshFilter>().mesh = my_mesh;
        Renderer rend = s.GetComponent<Renderer>();
        s.transform.parent = this.transform;
        if (curveUpward == false)
        {
            rend.material.color = new Color(Random.value, 1.0f, 1.0f, 1.0f);
        } else if (curveUpward == true)
        {
            rend.material.color = new Color(1.0f, Random.value, 1.0f, 1.0f);
        }
    }

    private Mesh Create_My_Mesh(Vector3 T, Vector3 N, Vector3 B, Vector3 startPoint, Vector3 endPoint, int start_age, int end_age)
    {
        //  T -> between caps
        Mesh mesh = new Mesh();
        int mesh_size = 50; // how many points on the edge of cap

        Vector3[] verts = new Vector3[mesh_size * 2 + 2];
        verts[2 * mesh_size] = startPoint;
        verts[2 * mesh_size + 1] = endPoint;

        int[] tris = new int[mesh_size * 6 * 2];
        float angle = 360 / mesh_size;
        float start_thic = thicknessCoefficient / start_age;
        float end_thic = thicknessCoefficient / end_age;
        //Vertices list init
        for (int i = 0; i < mesh_size * 2; i += 2)
        {
            Vector3 newPoint = startPoint + (N * Mathf.Cos(Mathf.Deg2Rad * angle * (i / 2)) + B * Mathf.Sin(Mathf.Deg2Rad * angle * (i / 2))) * start_thic;
            verts[i] = newPoint;
            Vector3 newEndPoint = endPoint + (N * Mathf.Cos(Mathf.Deg2Rad * angle * (i / 2)) + B * Mathf.Sin(Mathf.Deg2Rad * angle * (i / 2))) * end_thic;
            verts[i + 1] = newEndPoint;
        }
        mesh.vertices = verts;
        //Side triangles list init
        for (int i = 0; i < verts.Length - 2; i++)
        {
            if (i % 2 == 1)
            {
                tris[3 * i] = i;
                tris[3 * i + 1] = (i + 2) % (mesh_size * 2);
                tris[3 * i + 2] = (i + 1) % (mesh_size * 2);
            }
            else
            {
                tris[3 * i] = i;
                tris[3 * i + 1] = (i + 1) % (mesh_size * 2);
                tris[3 * i + 2] = (i + 2) % (mesh_size * 2);
            }
        }

        int x = 6 * mesh_size;
        for (int i = 0; i < verts.Length - 2; i += 2)
        {
            tris[x + i / 2 * 3] = 2 * mesh_size;
            tris[x + i / 2 * 3 + 1] = i;
            tris[x + i / 2 * 3 + 2] = (i + 2) % (2 * mesh_size);
        }
        int y = 9 * mesh_size;
        for (int i = 1; i <= 2 * mesh_size; i += 2)
        {
            tris[y + i / 2 * 3] = 2 * mesh_size + 1;
            tris[y + i / 2 * 3 + 1] = (i + 2) % (2 * mesh_size);
            tris[y + i / 2 * 3 + 2] = i % (2 * mesh_size);
        }
        mesh.triangles = tris;
        mesh.RecalculateNormals();  // automatically calculate the vertex normals
        return (mesh);
    }
}




//private void OnDrawGizmos()
//{
//    foreach (Internode internode in internode_list)
//    {
//        //Gizmos.color = Color.green;
//        //Gizmos.DrawLine(internode.start_node.position + this.transform.position, internode.end_node.position + this.transform.position);
//        //Gizmos.color = Color.red;
//        //Gizmos.DrawSphere(internode.end_node.position, 0.1f);
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawSphere(internode.start_node.position,0.1f);

//        Vector3 startPos = internode.start_node.position;
//        Vector3 endPos = internode.end_node.position;
//        Vector3 T = endPos - startPos;
//        Vector3 N = Quaternion.Euler(0.0f, 0.0f, 90.0f) * T;
//        Vector3 B = Vector3.Cross(T, N);
//        T = T.normalized; N = N.normalized; B = B.normalized;
//        Gizmos.color = Color.green;
//        Gizmos.DrawLine(internode.start_node.position + this.transform.position, internode.start_node.position + this.transform.position + T);
//        Gizmos.DrawLine(internode.start_node.position + this.transform.position, internode.start_node.position + this.transform.position + N);
//        Gizmos.DrawLine(internode.start_node.position + this.transform.position, internode.start_node.position + this.transform.position + B);
//    }
//}