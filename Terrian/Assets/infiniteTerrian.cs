using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class infiniteTerrian : MonoBehaviour {

    // Use this for initialization
    public GameObject onePiece;
    public GameObject myself;
    
    int meshSize = 10;
    int boundary = 5;
    Vector3 startPos;
    List<GameObject> meshList = new List<GameObject>();

	void Start () {
        create_my_mesh();
	}
	
	// Update is called once per frame
	void Update () {
        int x = (int)(myself.transform.position.x - startPos.x);
        int z = (int)(myself.transform.position.z - startPos.z);
        if (Mathf.Abs(x) >= meshSize/2 || Mathf.Abs(z) >= meshSize/2)
        {
            foreach (GameObject mesh in meshList)
            {
                Destroy(mesh);
            }
            meshList = create_my_mesh();
            startPos = myself.transform.position;
        }
	}

    List<GameObject> create_my_mesh()
    {
        gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;
        int meshNum = 0;
        for (int i = -boundary; i < boundary; i++)
        {
            for (int j = -boundary; j < boundary; j++)
            {
                float x = myself.transform.position.x + i * meshSize + startPos.x;
                float y = 0;
                float z = myself.transform.position.z + j * meshSize + startPos.z;
                Vector3 pos = new Vector3(x, y, z);
                GameObject my_mesh = (GameObject)Instantiate(onePiece, pos, Quaternion.identity);
                meshNum++;
                my_mesh.name = meshNum.ToString();
                meshList.Add(my_mesh);
            }
        }
        return meshList;
    }

}
