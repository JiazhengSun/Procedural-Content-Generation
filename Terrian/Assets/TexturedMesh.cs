// This sample code demonstrates how to create a texture using Perlin noise.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturedMesh : MonoBehaviour {

	public float scale = 10;

    public int max_mesh = -1; //The number of mesh we have made
    public int mesh_size = 30;

    int texture_width = 30;
    int texture_height = 30;

	// Create a quad that is textured
	void Start () {

        create_new_mesh();

	}

	// create a texture using Perlin noise
	Texture2D make_a_texture() {

		// create the texture and an array of colors that will be copied into the texture
		Texture2D texture = new Texture2D (texture_width, texture_height);
		Color[] colors = new Color[texture_width * texture_height];

		// create the Perlin noise pattern in "colors"
		for (int i = 0; i < texture_width; i++)
			for (int j = 0; j < texture_height; j++) {
				float x = scale * i / (float) texture_width;
				float y = scale * j / (float) texture_height;
				float t = Mathf.PerlinNoise (x, y);
				colors [j * texture_width + i] = new Color (t, t, t, 1.0f);  // gray scale values (r = g = b)
			}

		// copy the colors into the texture
		texture.SetPixels(colors);

		// do texture-y stuff, probably including making the mipmap levels
		texture.Apply();

		// return the texture
		return (texture);
	}

	// create a mesh that consists of two triangles that make up a quad
	Mesh CreateMyMesh() {
		// create a mesh object
		Mesh mesh = new Mesh();
        int newScale = 3;
        Vector3[] verts = new Vector3[(mesh_size + 1) * (mesh_size + 1)];
        for (int i = 0, z = 0; z <= mesh_size; z++)
        {
            for (int x = 0; x <= mesh_size; x++, i++)
            {
                float a = newScale * z / (float)mesh_size;
                float b = newScale * x / (float)mesh_size;
                float y = Mathf.PerlinNoise(a, b) + 0.5f * Mathf.PerlinNoise(a*2, b*2) + 0.25f * Mathf.PerlinNoise(a*4, b*4);
                verts[i] = new Vector3(x,y*8,z);
            }
        }
        mesh.vertices = verts;
        Vector4[] tangents = new Vector4[verts.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        int[] tris = new int[mesh_size * mesh_size * 6];
        for (int ti = 0, vi = 0, y = 0; y < mesh_size; y++, vi++)
        {
            for (int x = 0; x < mesh_size; x++, ti += 6, vi++)
            {
                tris[ti] = vi;
                tris[ti + 3] = tris[ti + 2] = vi + 1;
                tris[ti + 4] = tris[ti + 1] = vi + mesh_size + 1;
                tris[ti + 5] = vi + mesh_size + 2;
            }
        }
        mesh.triangles = tris;

        Vector2[] uv = new Vector2[verts.Length];
        for (int i = 0, y = 0; y <= texture_height; y++)
        {
            for (int x = 0; x <= texture_width; x++, i++)
            {
                //verts[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / texture_width, (float)y / texture_height);
                tangents[i] = tangent;
            }
        }

		// save the vertices and triangles in the mesh object	
		mesh.uv = uv;  // save the uv texture coordinates
        mesh.tangents = tangents;
		mesh.RecalculateNormals();  // automatically calculate the vertex normals

		return (mesh);
	}

	// Update is called once per frame
	void Update () {
        float dx = Input.GetAxis("Horizontal");
        float dz = Input.GetAxis("Vertical");

        float translate_factor = 3.0f;
        float rotate_factor = 3.0f;
        if (Camera.current != null)
        {
            Camera.current.transform.Translate(0, 0, dz * translate_factor);
            Camera.current.transform.Rotate(0, dx*rotate_factor, 0);
        }
        Vector3 cam_pos = Camera.main.transform.position;
        if (cam_pos.z > (max_mesh + 1) * mesh_size)
        {
            create_new_mesh();
        }
	}

    void create_new_mesh()
    {
        int index = max_mesh + 1;

        GameObject s = mesh_creation();
        s.name = index.ToString("Mesh 0");

        s.transform.localScale = new Vector3(scale, scale, scale);
        s.transform.position = new Vector3(0,0,scale*mesh_size*(index + 0.0f));

        max_mesh++;

    }

    GameObject mesh_creation()
    {
        // call the routine that makes a mesh (a cube) from scratch
        Mesh my_mesh = CreateMyMesh();
        my_mesh.RecalculateBounds();

        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s = new GameObject("Textured Mesh");
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();

        // associate my mesh with this object
        s.GetComponent<MeshFilter>().mesh = my_mesh;

        // change the color of the object
        Renderer rend = s.GetComponent<Renderer>();
        rend.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture = make_a_texture();

        // attach the texture to the mesh
        Renderer renderer = s.GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

        return s;
    }
		
}
