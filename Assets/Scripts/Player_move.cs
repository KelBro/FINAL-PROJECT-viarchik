using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Player_move : MonoBehaviour
{
    private SteamVR_Action_Vector2 stick = null;
    private SteamVR_Action_Boolean m_Boolean = null;

    private CharacterController controller = null;

    public float speed = 1f;

    private bool checkWalk = false;

    private void Awake()
    {
        stick = SteamVR_Actions._default.Stick_move;
        m_Boolean = SteamVR_Actions._default.Touch_Controller;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (m_Boolean.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            checkWalk = true;
        }
        if (m_Boolean.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            checkWalk = false;
        }
        if (checkWalk)
        {
            Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(stick.axis.x, 0, stick.axis.y));
            controller.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up) - new Vector3(0, 9.81f, 0) * Time.deltaTime);
        }

    }
}
