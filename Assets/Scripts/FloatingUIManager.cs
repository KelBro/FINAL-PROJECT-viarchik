using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject rect;
    [SerializeField]
    private int delay = 120;
    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private float speed = .2f;
    private int untilMove;
    
    void Start()
    {
        untilMove = delay;
        rect.transform.position = camera.transform.position + camera.transform.forward * radius;
        
        print(Vector3.Slerp(Vector3.forward, Vector3.right, .01f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var head = camera.transform.position;
        var gaze = camera.transform.forward;

        if (untilMove == 0)
        { 
            var start = rect.transform.forward;
            var newVec = Vector3.Slerp(start, gaze, speed);

            rect.transform.position = head + newVec * radius;
            rect.transform.rotation = Quaternion.LookRotation(newVec, Vector3.up);

            // TODO: Look away timer. Perhaps using ProjectOnPlane()
            // Vector3.ProjectOnPlane()
        }
        else
        {
            untilMove -= 1;
        }
    }
}
