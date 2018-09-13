using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshByPlane : MonoBehaviour {

    // Use this for initialization
    public float meshScale = 10.0f;
    public int heightScale = 30;
    public int texture_width = 10;
    public int texture_height = 10;
    public int scale = 10;
    public GameObject plant;

    void Start () {
        Mesh myMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verts = myMesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            float a = (verts[i].x + this.transform.position.x) / meshScale;
            float b = (verts[i].z + this.transform.position.z) / meshScale;
            float y = Mathf.PerlinNoise(a, b) + 0.5f * Mathf.PerlinNoise(a * 2, b * 2) + 0.25f * Mathf.PerlinNoise(a * 4, b * 4);
            verts[i].y = y * heightScale;
        }
        myMesh.vertices = verts;
        myMesh.RecalculateBounds();
        myMesh.RecalculateNormals();

        create_my_plant(verts);
        create_water(verts);

        // create a texture
        Texture2D texture = make_a_texture(verts.Length, verts);

        // attach the texture to the mesh
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

    }

    void create_my_plant(Vector3[] verts)
    {
        Random.InitState(5);
        for (int i = 0; i < verts.Length; i += 2 * (int)Mathf.Sqrt(verts.Length))
        {
            float checker = Random.value;
            //Debug.Log(checker);
            if (checker < 0.1)
            {
                GameObject my_plant = Instantiate(plant, transform.TransformPoint(verts[i]), Quaternion.identity);
                my_plant.transform.SetParent(this.transform);
            }
        }
    }

    void create_water(Vector3[] verts)
    {
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Plane);
        water.GetComponent<Renderer>().material.color = new Color(0.1f, 0.2f, 1.0f, 0.5f);
        water.GetComponent<Renderer>().material.SetFloat("_Mode", 3.0f);
        water.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");

        float waterHeight = 0f;
        for (int i = 0; i < verts.Length; i++)
        {
            waterHeight += verts[i].y;
        }
        waterHeight /= verts.Length;
        waterHeight *= 0.8f;
        water.transform.position = new Vector3(this.transform.position.x, waterHeight, this.transform.position.z);
        water.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        water.transform.SetParent(this.transform);
    }

    Texture2D make_a_texture(int len, Vector3[] verts)
    {

        // create the texture and an array of colors that will be copied into the texture
        int edge = (int)Mathf.Sqrt(len);
        Texture2D texture = new Texture2D(edge, edge);
        Color[] colors = new Color[len];
        // create the Perlin noise pattern in "colors"
        Random.InitState(42);
        for (int i = 0; i < len; i++)
        {
            float height = verts[i].y / heightScale;

           // Debug.Log(height);
            colors[i] = new Color(height * 0.8f,height *0.5f,Random.value* height*0.5f, 1.0f);
        }

        // copy the colors into the texture
        texture.SetPixels(colors);

        // do texture-y stuff, probably including making the mipmap levels
        texture.Apply();

        // return the texture
        return (texture);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
