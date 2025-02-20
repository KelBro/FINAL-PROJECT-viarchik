using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
    public Generation levelData;

    public GameObject[] buildings;

    private void Start()
    {
        levelData = JsonUtility.FromJson<Generation>(cityData.text);

        Vector3 planeLeft = Vector3.zero;
        Vector3 planeRight = Vector3.zero;

        // Generating building blocks with sidewalks
        foreach (Block block in levelData.blocks)
        {
            Point left = block.left;
            Point right = block.right;
            Vector3 center = new Vector3((left.x + right.x) / 2, 0f, (left.y + right.y) / 2);

            GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sidewalk.transform.localScale = new Vector3(right.x - left.x + 3f, 0.2f, right.y - left.y + 3f);
            sidewalk.transform.position = center + new Vector3(0f, 0.1f, 0f);

            // Sidewalk traffic lines
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.localScale = new Vector3(right.x - left.x + 3.8f, .001f, .2f);
            line.transform.position = new Vector3(center.x, 0f, right.y + 1.8f);
            line.GetComponent<Renderer>().material.color = Color.white;
            line = Instantiate(line, new Vector3(center.x, 0f, left.y - 1.8f), Quaternion.identity);
            line.GetComponent<Renderer>().material.color = Color.white;
            line = Instantiate(line, new Vector3(right.x + 1.8f, 0f, center.z), Quaternion.identity);
            line.transform.localScale = new Vector3(.2f, .001f, right.y - left.y + 3.8f);
            line.GetComponent<Renderer>().material.color = Color.white;
            line = Instantiate(line, new Vector3(left.x - 1.8f, 0f, center.z), Quaternion.identity);
            line.GetComponent<Renderer>().material.color = Color.white;

            if (left.x < planeLeft.x) { planeLeft.x = left.x; }
            if (left.y < planeLeft.z) { planeLeft.z = left.x; }
            if (right.x > planeRight.x) { planeRight.x = right.x; }
            if (right.y > planeRight.z) { planeRight.z = right.y; }

            float remainingX = right.x - left.x;
            float remainingY = right.y - left.y;
            float max = Mathf.Max(remainingX, remainingY);
            float min = Mathf.Min(remainingX, remainingY);

            //List<Rectangle> buildingRects = FillRectangle(left.x, left.y, right.x, right.y);

            //foreach (Rectangle buildingRect in buildingRects)
            //{
            //    Instantiate(buildings[buildingRect.id], new Vector3(left.x, 0f, left.y) + buildingRect.center, Quaternion.identity);
            //}

            float countX = Mathf.Ceil((right.x - left.x) / 15f);
            float countY = Mathf.Ceil((right.y - left.y) / 15f);

            float width = (right.x - left.x) / countX;
            float height = (right.y - left.y) / countY;

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    var stories = Random.Range(30f, 40f);
                    cube.transform.localScale = new Vector3 (width, stories, height);
                    cube.transform.position = new Vector3(left.x + width * x + width / 2, stories / 2, left.y + height * y + height / 2);
                    cube.GetComponent<Renderer>().material.color = new Color(220f / 255f, 227f / 255f, 252f / 255f);
                }
            }
        }

        // Create the ground plane
        var planeRect = planeRight - planeLeft;
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = planeRect / 10;
        var planeCenter = (planeLeft + planeRight) / 2;
        plane.transform.position = planeCenter;
        plane.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);

        for (int i = 0; i < levelData.crossroads.Length; i++)
        {
            var crossroad = levelData.crossroads[i];
            Vector3 center = new(crossroad.center.x, 0f, crossroad.center.y);

            for (int j = i + 1; j < levelData.crossroads.Length; j++)
            {
                Crossroad other = levelData.crossroads[j];
                Vector3 otherCenter = new(other.center.x, 0f, other.center.y);

                // Generating the traffic lines (needs cleanup)
                if (center.x == otherCenter.x && otherCenter.z > center.z && crossroad.up)
                {
                    var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.transform.localScale = new Vector3(.2f, .001f, otherCenter.z - center.z - 10f);
                    line.transform.position = new Vector3(center.x, 0f, (center.z + otherCenter.z) / 2);
                    line.GetComponent<Renderer>().material.color = Color.white;
                }
                else if (center.x == otherCenter.x && otherCenter.z < center.z && crossroad.down)
                {
                    var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.transform.localScale = new Vector3(.2f, .001f, center.z - otherCenter.z - 10f);
                    line.transform.position = new Vector3(center.x, 0f, (center.z + otherCenter.z) / 2);
                    line.GetComponent<Renderer>().material.color = Color.white;
                }
                else if (otherCenter.x > center.x && otherCenter.z == center.z && crossroad.right)
                {
                    var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.transform.localScale = new Vector3(otherCenter.x - center.x - 10f, .001f, .2f);
                    line.transform.position = new Vector3((center.x + otherCenter.x) / 2, 0f, center.z);
                    line.GetComponent<Renderer>().material.color = Color.white;
                }
                else if (otherCenter.x < center.x && otherCenter.z == center.z && crossroad.left)
                {
                    var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.transform.localScale = new Vector3(center.x - otherCenter.x - 10f, .001f, .2f);
                    line.transform.position = new Vector3((center.x + otherCenter.x) / 2, 0f, center.z);
                    line.GetComponent<Renderer>().material.color = Color.white;
                }
            }

            // Generating the trafficLights
            if (crossroad.up)
            {
                Instantiate(trafficLight, center + new Vector3(-6.5f, 0, 6.5f), Quaternion.LookRotation(Vector3.back, Vector3.up));
            }
            if (crossroad.down)
            {
                Instantiate(trafficLight, center + new Vector3(6.5f, 0, -6.5f), Quaternion.LookRotation(Vector3.forward, Vector3.up));
            }
            if (crossroad.left)
            {
                Instantiate(trafficLight, center + new Vector3(-6.5f, 0, -6.5f), Quaternion.LookRotation(Vector3.right, Vector3.up));
            }
            if (crossroad.left)
            {
                Instantiate(trafficLight, center + new Vector3(6.5f, 0, 6.5f), Quaternion.LookRotation(Vector3.left, Vector3.up));
            }
        }

        // I did this cuz I am running out of time and ideas. Don't do this like, ever.
        Physics.simulationMode = SimulationMode.Script;
        Physics.Simulate(0.01f);

        // Cleanup extra traffic lines
        foreach (Crossroad crossroad in levelData.crossroads)
        {
            Vector3 center = new(crossroad.center.x, 0f, crossroad.center.y);

            RaycastHit hit;
            if (Physics.Raycast(center + Vector3.up, Vector3.down, out hit, 2f))
            {
                Destroy(hit.transform.gameObject);
                print("Hit detected!");
            }
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    //private struct Rectangle
    //{
    //    public Vector3 size;
    //    public Vector3 center;
    //    public int id;

    //    public Rectangle( Vector3 size, Vector3 center, int id )
    //    {
    //        this.size = size;
    //        this.center = center;
    //        this.id = id;
    //    }
    //}

    //private Vector3[] smallRectSizes = new Vector3[3]
    //{
    //    new(12f, 0f, 45f),
    //    new(20f, 0f, 18f),
    //    new(12f, 0f, 12f)
    //};
    //private float maxGapSize = 2f;

    //private List<Rectangle> FillRectangle(float leftX, float leftY, float rightX, float rightY)
    //{
    //    float x = rightX - leftX;
    //    float y = rightY - leftY;
    //    float currentX = 0;
    //    float currentY = 0;

    //    List<Rectangle> rectangles = new List<Rectangle>();

    //    while (currentY < y)
    //    {
    //        bool placedRectangle = false;

    //        Shuffle(smallRectSizes);

    //        for (int i = 0; i < 3; i++)
    //        {
    //            Vector3 rectSize = smallRectSizes[i];
    //            if (currentX + rectSize.x <= x && currentY + rectSize.z <= y)
    //            {
    //                Vector2 center = new Vector2(currentX + rectSize.x / 2, currentY + rectSize.z / 2);

    //                rectangles.Add(new Rectangle(rectSize, center, i));

    //                currentX += rectSize.x + UnityEngine.Random.Range(0, maxGapSize);
    //                placedRectangle = true;
    //                break;
    //            }
    //        }

    //        if (!placedRectangle)
    //        {
    //            currentX = 0;
    //            currentY += GetNextRowHeight(rectangles, currentY) + UnityEngine.Random.Range(0, maxGapSize);
    //        }
    //    }
    //    return rectangles;
    //}

    //private void Shuffle(Vector3[] array)
    //{
    //    for (int i = array.Length - 1; i > 0; i--)
    //    {
    //        int j = UnityEngine.Random.Range(0, i + 1);
    //        (array[j], array[i]) = (array[i], array[j]);
    //    }
    //}

    //private float GetNextRowHeight(List<Rectangle> rectangles, float currentY)
    //{
    //    float maxHeight = 0;
    //    foreach (var rect in rectangles)
    //    {
    //        if (rect.center.z > maxHeight)
    //        {
    //            maxHeight = rect.center.z + rect.size.z / 2;
    //        }
    //    }
    //    return maxHeight - currentY;
    //}
}
