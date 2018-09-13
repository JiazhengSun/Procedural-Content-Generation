using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug : MonoBehaviour {
    // Use this for initialization
    private Vector3[] verts;
    private int[] tris;
    void Start () {
        Mesh my_mesh = Create_My_Mesh(new Vector3(0, 10, 0), Vector3.right, Vector3.forward, new Vector3(0, 0, 0), new Vector3(0,1,0));
        GameObject s = new GameObject("Textured Mesh");
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();
        s.GetComponent<MeshFilter>().mesh = my_mesh;
        Renderer rend = s.GetComponent<Renderer>();
        rend.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private Mesh Create_My_Mesh(Vector3 T, Vector3 N, Vector3 B, Vector3 startPoint, Vector3 endPoint)
    {
        //  T -> between caps
        Mesh mesh = new Mesh();
        int mesh_size = 50; // how many points on the edge of cap

        verts = new Vector3[mesh_size * 2 + 2];
        verts[2 * mesh_size] = startPoint;
        verts[2 * mesh_size + 1] = endPoint;

        tris = new int[mesh_size * 6 * 2];
        float angle = 360/ mesh_size;
        //Vertices list init
        for (int i = 0; i < mesh_size*2; i+=2)
        {
            Vector3 newPoint = startPoint + N * Mathf.Cos(Mathf.Deg2Rad*angle * (i/2)) + B * Mathf.Sin(Mathf.Deg2Rad*angle * (i/2));
            verts[i] = newPoint;
            verts[i + 1] = newPoint + endPoint - startPoint;
        }
        mesh.vertices = verts;
        //Side triangles list init
        for (int i = 0; i < verts.Length - 2; i++)
        {
            if (i%2 == 1)
            {
                tris[3 * i] = i;
                tris[3 * i + 1] = (i + 2)%(mesh_size*2);
                tris[3 * i + 2] = (i + 1) % (mesh_size*2);
            } else
            {
                tris[3 * i] = i;
                tris[3 * i + 1] = (i + 1) % (mesh_size * 2);
                tris[3 * i + 2] = (i + 2) % (mesh_size * 2);
            }
        }

        int x = 6 * mesh_size;
        for (int i = 0; i < verts.Length - 2; i += 2)
        {
            tris[x + i / 2*3] = 2 * mesh_size;
            tris[x + i / 2*3 + 1] = i;
            tris[x + i / 2*3 + 2] = (i + 2) % (2 * mesh_size);
        }
        int y = 9 * mesh_size;
        for (int i = 1; i <= 2 * mesh_size; i += 2)
        {
            tris[y + i / 2*3] = 2 * mesh_size + 1;
            tris[y + i / 2*3 + 1] = (i + 2) % (2 * mesh_size);
            tris[y + i/ 2*3 + 2] = i% (2 * mesh_size);
        }
        mesh.triangles = tris;
        mesh.RecalculateNormals();  // automatically calculate the vertex normals
        return (mesh);
    }
    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < verts.Length; i++)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(verts[i], 0.1f);
    //    }
       
    //}
}
