using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assemble : MonoBehaviour {
    //Torso
    public int seed;
    public Vector3 referencePoint;
    public Vector3 TorsoControlPoint1; public Vector3 TorsoControlPoint2; public Vector3 TorsoControlPoint3; public Vector3 TorsoControlPoint4;
    
    //Arms
    public GameObject Arm1; public GameObject Arm2; public GameObject Arm3;
    private List<GameObject> armList = new List<GameObject>();

    public GameObject Leg1; public GameObject Leg2; public GameObject Leg3;
    private List<GameObject> legList = new List<GameObject>();

    public GameObject Head1; public GameObject Head2; public GameObject Head3;
    private List<GameObject> headList = new List<GameObject>();

    // Use this for initialization
    void Start () {
        Random.InitState(seed);
        armList.Add(Arm1); armList.Add(Arm2); armList.Add(Arm3);
        legList.Add(Leg1); legList.Add(Leg2); legList.Add(Leg3);
        headList.Add(Head1); headList.Add(Head2); headList.Add(Head3);

        //Create Torso
        //CurveSurface(int randomSeed, Vector3 referencePoint, Vector3 ControlPoint1,Vector3 ControlPoint2,Vector3 ControlPoint3,Vector3 ControlPoint4)
        CurveSurface TorsoSurface = new CurveSurface(seed, referencePoint, TorsoControlPoint1, TorsoControlPoint2, TorsoControlPoint3, TorsoControlPoint4);
        GameObject torso = new GameObject("Torso");
        torso.transform.SetParent(this.transform);
        torso.transform.position = referencePoint;
        Mesh torso_mesh = TorsoSurface.CustomMesh();
        torso.AddComponent<MeshFilter>();
        torso.AddComponent<MeshRenderer>();
        torso.GetComponent<MeshFilter>().mesh = torso_mesh;
        Renderer rend = torso.GetComponent<Renderer>();
        rend.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        

        //Append Arms
        Vector3[] armPos = GetLimbPos(0.3f, TorsoSurface.GetBasicCurve(), TorsoSurface.GetRevolution(), 20);
        int arm_one_index = Random.Range(0,3);
        int arm_two_index = Random.Range(0,3);
        GameObject chosenArmOne = armList[arm_one_index]; GameObject chosenArmTwo = armList[arm_two_index];
        GameObject LeftArm = Instantiate(chosenArmOne, armPos[1] + referencePoint, Quaternion.identity);
        GameObject RightArm = Instantiate(chosenArmTwo, armPos[0] + referencePoint, Quaternion.identity);
        RightArm.transform.localScale = new Vector3(-LeftArm.transform.localScale.x, LeftArm.transform
            .localScale.y, LeftArm.transform.localScale.z);
        LeftArm.transform.parent = this.transform; RightArm.transform.parent = this.transform;


        //Append Legs
        Vector3[] legPos = GetLimbPos(0.9f, TorsoSurface.GetBasicCurve(), TorsoSurface.GetRevolution(), 20);
        int leg_index = Random.Range(0,3);
        GameObject chosenLeg = legList[leg_index];
        GameObject LeftLeg = Instantiate(chosenLeg, legPos[0] + referencePoint, Quaternion.identity);
        GameObject RightLeg = Instantiate(chosenLeg, legPos[1] + referencePoint, Quaternion.identity);
        RightLeg.transform.localScale = new Vector3(-LeftLeg.transform.localScale.x, LeftLeg.transform
            .localScale.y, LeftLeg.transform.localScale.z);
        LeftLeg.transform.parent = this.transform; RightLeg.transform.parent = this.transform;

        //Append Head
        Vector3 HeadPos = TorsoSurface.GetHeadPoint();
        int head_index = Random.Range(0, 3);
        GameObject chosenHead = headList[head_index];
        GameObject Head = Instantiate(chosenHead, HeadPos, Quaternion.identity);
        Head.transform.parent = this.transform;

    }

    Vector3[] GetLimbPos(float ratio, Vector3[] basicCurve, Vector3[] revolution, int cirPoints)
    {
        int pos = Mathf.RoundToInt(ratio * basicCurve.Length);
        //Debug.Log(pos);
        Vector3 positionRight = revolution[pos * cirPoints];
        Vector3 positionLeft = revolution[pos * cirPoints + cirPoints/2];
        Vector3[] result = new Vector3[2]; result[0] = positionLeft; result[1] = positionRight;
        return result;
    }

}
