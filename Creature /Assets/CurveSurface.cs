using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveSurface
{
    //Used BezierCurve + surface of Revolution
    
    private Vector3 referencePoint;
    private Vector3 ControlPoint1; private Vector3 ControlPoint2; private Vector3 ControlPoint3; private Vector3 ControlPoint4;
    private Vector3[] basicCurve;
    private Vector3[] revolution;
    private Vector3[] basePoints;

    private float height; private float fat;
    private int seed;
    private Mesh my_mesh;

    // Use this for initialization
    public CurveSurface(int randomSeed, Vector3 refPoint, Vector3 CP1,Vector3 CP2,Vector3 CP3,Vector3 CP4) {
        seed = randomSeed;
        Random.InitState(seed);
        //ControlPoint1 = new Vector3(1,0,0); ControlPoint2 = new Vector3(4,5,0);
        //ControlPoint3 = new Vector3(0,6,0); ControlPoint4 = new Vector3(1,10,0);
        referencePoint = refPoint;
        ControlPoint1 = CP1; ControlPoint2 = CP2; ControlPoint3 = CP3; ControlPoint4 = CP4;
        height = Random.Range(1.0f,5.0f); fat = Random.Range(1.0f,3.0f);

        basePoints = new Vector3[] { ControlPoint1, ControlPoint2, ControlPoint3, ControlPoint4};

        AdjustHeight(basePoints);

        // Add in fat and height factor later with random stuff. 
        // Use height and fat as scaling factor
        
        basicCurve = MyBezier(ControlPoint4 + referencePoint, ControlPoint3 + referencePoint, ControlPoint2 + referencePoint, ControlPoint1 + referencePoint, 20);
        revolution = MyRevolution(basicCurve, referencePoint + new Vector3(0,1,0), 20);

        my_mesh = Create_My_Mesh(revolution, 20,20); //vertices, # of points on a circle, # points on a curve. 

    }
    public Mesh CustomMesh() { return my_mesh; }

    public Vector3 GetHeadPoint() { return new Vector3(referencePoint.x, revolution[3].y, referencePoint.z); }

    public Vector3 GetTailPoint() { return referencePoint; }

    Vector3[] MyBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int n)
    {
        // n -> number of points
        Vector3[] bezierCurve = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)n;
            float newx = B1(t) * p1.x + B2(t) * p2.x + B3(t) * p3.x + B4(t) * p4.x;
            float newy = B1(t) * p1.y + B2(t) * p2.y + B3(t) * p3.y + B4(t) * p4.y;
            float newz = B1(t) * p1.z + B2(t) * p2.z + B3(t) * p3.z + B4(t) * p4.z;
            Vector3 newPoint = new Vector3(newx, newy, newz);
            bezierCurve[i] = newPoint;
        }
        return bezierCurve;
    }

    float B1(float t) { return Mathf.Pow(1 - t, 3); }
    float B2(float t) { return 3.0f * t * Mathf.Pow(1 - t, 2); }
    float B3(float t) { return 3.0f * Mathf.Pow(t, 2) * (1 - t); }
    float B4(float t) { return Mathf.Pow(t, 3); }

    Vector3[] MyRevolution (Vector3[] baseCurve, Vector3 axis, int numPoints)
    {
        Vector3[] finalResult = new Vector3[baseCurve.Length * numPoints];
        Vector3 newAxis;
        float unitAngle = 360 /(float) numPoints * Mathf.Deg2Rad;
        int count = 0;
        for (int i = 0; i < baseCurve.Length; i++)
        {
            newAxis = new Vector3(axis.x, baseCurve[i].y, axis.z);
            float radius = Vector3.Distance(baseCurve[i],newAxis);
            float angle = 0;
            for (int j = 1; j <=numPoints; j++)
            {
                angle += unitAngle;
                Vector3 newPoint = new Vector3(fat * radius * Mathf.Cos(angle), baseCurve[i].y, fat * radius * Mathf.Sin(angle));
                finalResult[count] = newPoint;
                count += 1;
            }
        }
        return finalResult;
    }


    private Mesh Create_My_Mesh(Vector3[] verts, int cirPoints, int curPoints)
    {
        //verts.length = param * curPoints;m
        int[] tris;
        Mesh mesh = new Mesh();
        //tris = new int[mesh_size * 6 * 2];
        tris = new int[cirPoints * 2 * 3 * curPoints];
        mesh.vertices = verts;
        //Side triangles list init
        for (int j = 0; j < curPoints - 2; j++)
        {
            for (int i = 0, t = 0; t < cirPoints; i += 6, t++)
            {
                tris[i + j * cirPoints * 6] = t + j * cirPoints;
                tris[i + 1 + j * cirPoints * 6] = tris[i + 4 + j * cirPoints * 6] = (t + j * cirPoints + 1) % cirPoints + cirPoints * j;
                tris[i + 2 + j * cirPoints * 6] = tris[i + 3 + j * cirPoints * 6] = cirPoints * (j + 1) + t;
                tris[i + 5 + j * cirPoints * 6] = (t + j * cirPoints + 1) % (cirPoints) + cirPoints * (j + 1);
            }
        }
        mesh.triangles = tris;
        mesh.RecalculateNormals();  // automatically calculate the vertex normals
        return (mesh);
    }

    void AdjustHeight(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y *= height;
        }
    }

    public Vector3[] GetBasicCurve() { return basicCurve; }
    public Vector3[] GetRevolution() { return revolution; }
}
