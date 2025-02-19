using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Generation
{
    public Crossroad[] crossroads;
    public Block[] blocks;
}
[System.Serializable]
public class Crossroad
{
    public Point center;
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public float trafficLight1;
    public float trafficLight2;
}
[System.Serializable]
public class Block
{
    public Point left;
    public Point right;
}
[System.Serializable]
public class Point
{
    public float x;
    public float y;
}

public class CityBuilder : MonoBehaviour
{
    public TextAsset cityData;
    public GameObject trafficLight;

    private void Start()
    {
        Generation levelData = JsonUtility.FromJson<Generation>(cityData.text);

        Vector3 planeLeft = Vector3.zero;
        Vector3 planeRight = Vector3.zero;
        foreach(Block block in levelData.blocks)
        {
            var left = block.left;
            var right = block.right;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(right.x - left.x, 40f, right.y - left.y);
            var center = new Vector3((left.x + right.x) / 2, 0f, (left.y + right.y) / 2);
            cube.transform.position = center + new Vector3(0f, 20f, 0f);
            GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sidewalk.transform.localScale = new Vector3(right.x - left.x + 3f, 0.2f, right.y - left.y + 3f);
            sidewalk.transform.position = center + new Vector3(0f, 0.1f, 0f);

            if (left.x < planeLeft.x) {  planeLeft.x = left.x; }
            if (left.y < planeLeft.z) {  planeLeft.z = left.x; }
            if (right.x > planeRight.x) { planeRight.x = right.x; }
            if (right.y > planeRight.z) { planeRight.z = right.y; }
        }

        var planeRect = planeRight - planeLeft;
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = planeRect / 10;
        var planeCenter = (planeLeft + planeRight) / 2;
        plane.transform.position = planeCenter;

        foreach (Crossroad crossroad in levelData.crossroads)
        {
            var center = crossroad.center;
            if (crossroad.up)
            {
                Instantiate(trafficLight, new Vector3(center.x - 6.5f, 0, center.y + 6.5f), Quaternion.LookRotation(Vector3.back, Vector3.up));
            }
            if (crossroad.down)
            {
                Instantiate(trafficLight, new Vector3(center.x + 6.5f, 0, center.y - 6.5f), Quaternion.LookRotation(Vector3.forward, Vector3.up));
            }
            if (crossroad.left)
            {
                Instantiate(trafficLight, new Vector3(center.x - 6.5f, 0, center.y - 6.5f), Quaternion.LookRotation(Vector3.right, Vector3.up));
            }
            if (crossroad.left)
            {
                Instantiate(trafficLight, new Vector3(center.x + 6.5f, 0, center.y + 6.5f), Quaternion.LookRotation(Vector3.left, Vector3.up));
            }
        }
    }
}
