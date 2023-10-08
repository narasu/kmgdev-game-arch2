using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class StateJumping : AbstractState
{
    private bool checkIfGrounded;
    private Rigidbody playerRigidbody;
    private GameObject playerBody;
    private int counter = 0;
    private int counterMax = 100;
    private Vector3 beginPos;
    private float JumpForce = 300f;

    private Player.MoveStates previousState;

    private Action<KeyWEvent> onKeyW;
    private Action<KeyAEvent> onKeyA;
    private Action<KeySEvent> onKeyS;
    private Action<KeyDEvent> onKeyD;
    private Action<KeySpaceEvent> onKeySpace;
    private Action<KeyLeftShiftEvent> onLeftShift;





    public StateJumping(Scratchpad _ownerData, StateMachine _ownerStateMachine) : base(_ownerData, _ownerStateMachine)
    {
        checkIfGrounded = false;
    }

    public override void OnEnter()
    {
        LinkEvents();
        SubscribeEvents();
        previousState = OwnerData.Read<Player.MoveStates>("previousState");
        Debug.Log("Current State : Jumping");
        Debug.Log("Previous State was : " + previousState.ToString());
        PlayerSetup();
        Jump();

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        Timer();

    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        CheckIfGrounded();
        CheckForWalls();
        



    }
    public override void OnExit()
    {
        UnSubscribeEvents();
        OwnerData.Delete("previousState");
        previousState = Player.MoveStates.Jumping;
        OwnerData.Write("previousState", previousState);
    }
    private void Jump()
    {
        playerRigidbody.AddForce(Vector3.up * JumpForce);
    }

    private void CheckIfGrounded()
    {
        if (checkIfGrounded == true)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.down).normalized, out hit, 2.0f))
            {
                if (previousState == Player.MoveStates.Standing)
                {
                    checkIfGrounded = false;
                    SwitchToStanding();

                }

                if (previousState == Player.MoveStates.Walking)
                {
                    checkIfGrounded = false;
                    SwitchTowalking();
                }

                if (previousState == Player.MoveStates.Running)
                {
                    checkIfGrounded = false;
                    SwitchToRunning();

                }
            }
        }
    }
    private void PlayerSetup()
    {
        playerBody = OwnerData.Read<GameObject>("playerDataPrefab");
        playerRigidbody = playerBody.GetComponent<Rigidbody>();

        beginPos = playerBody.transform.position;

    }

    private void Timer()
    {

        if (counter <= counterMax)
        {
            counter++;
        }
        else
        {
            //ResetTimer();
            checkIfGrounded = true;
        }
    }
    private void ResetTimer()
    {
        counter = 0;
    }

    private void CheckForWalls()
    {
        //Rechts \
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right).normalized, out hit, 2.0f))
        {
            Debug.DrawRay(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("WalkAbleWall") /*&& previousState == Player.MoveStates.Running*/)
            {
                SwitchToWallRunning();
            }
        }
        else
        {
            Debug.DrawRay(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right) * 1000, Color.red);
        }

        //links \

        if (Physics.Raycast(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right).normalized, out hit, 2.0f))
        {
            Debug.DrawRay(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("WalkAbleWall") /*&& previousState == Player.MoveStates.Running*/)
            {
                SwitchToWallRunning();
            }
        }
        else
        {
            Debug.DrawRay(playerBody.transform.position, playerBody.transform.TransformDirection(Vector3.right) * 1000, Color.red);
        }
    }
    //Events
    private void SubscribeEvents()
    {
        EventManager.Subscribe(typeof(KeyWEvent), onKeyW);
        EventManager.Subscribe(typeof(KeyAEvent), onKeyA);
        EventManager.Subscribe(typeof(KeySEvent), onKeyS);
        EventManager.Subscribe(typeof(KeyDEvent), onKeyD);
        EventManager.Subscribe(typeof(KeySpaceEvent), onKeySpace);
    }
    private void UnSubscribeEvents()
    {
        EventManager.Unsubscribe(typeof(KeyWEvent), onKeyW);
        EventManager.Unsubscribe(typeof(KeyAEvent), onKeyA);
        EventManager.Unsubscribe(typeof(KeySEvent), onKeyS);
        EventManager.Unsubscribe(typeof(KeyDEvent), onKeyD);
        EventManager.Unsubscribe(typeof(KeySpaceEvent), onKeySpace);
    }
    private void LinkEvents()
    {

    }


    // Switch States
    private void SwitchToStanding()
    {
        OwnerStateMachine.SwitchState(typeof(StateStanding));
    }
    private void SwitchTowalking()
    {
        OwnerStateMachine.SwitchState(typeof(StateWalking));
    }

    private void SwitchToRunning()
    {
        OwnerStateMachine.SwitchState(typeof(StateRunning));
    }
    private void SwitchToWallRunning()
    {
        OwnerStateMachine.SwitchState(typeof (StateWallRunning));
    }
}
