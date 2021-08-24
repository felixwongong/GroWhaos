using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown.Control
{
    public class PlayerController : MonoBehaviour
    {
        //CONFIG:
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float senseDistance = 1f;

        //STATE:
        Vector2 moveInput;
        bool takeInput = false;
        Vector3 moveValue;
        Vector3 mousePos;
        GameObject detectedObj;

        //REF:
        #region REFERENCE
        PlayerInputAction playerAction;
        Rigidbody rb;
        Camera mc;
        [SerializeField] Animator anim;
        [SerializeField] Transform topSensor;
        [SerializeField] Transform botSensor;
        Dragger dragger;
        int groundMask;
        int playerMask;
        #endregion

        private void Awake()
        {
            playerAction = new PlayerInputAction();
            rb = GetComponent<Rigidbody>();
            mc = Camera.main;
            playerMask = LayerMask.GetMask("Player");
            dragger = GetComponent<Dragger>();
        }

        private void OnEnable()
        {
            playerAction.Enable();
            playerAction.PlayerAction.Take.performed += ctx => HandleDetect();
            playerAction.PlayerAction.Throw.performed += ctx => HandlePullRelease(true);
            playerAction.PlayerAction.Throw.canceled += ctx => HandlePullRelease(false);
        }

        private void OnDisable()
        {
            playerAction.Disable();
        }

        void Start()
        {

        }

        private void Update()
        {
            moveInput = playerAction.PlayerAction.Movement.ReadValue<Vector2>();
            HandleMousePosition();
            detectedObj = FrontDetect();
        }

        void FixedUpdate()
        {
            anim.SetFloat("velocity", (moveInput != Vector2.zero) ? 1f : 0f);

            moveValue = new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed);
            rb.velocity = moveValue;
            Rotate();
        }
        private void HandleMousePosition()
        {
            Ray camRay = mc.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit, Mathf.Infinity, ~1 << groundMask))
            {
                mousePos = hit.point;
                mousePos.y = 0;
            }
        }

        private void HandleDetect()
        {
            if (detectedObj)
            {
                GetComponent<Dragger>().Hold(detectedObj);
            }
        }

        private void HandlePullRelease(bool _isPulling)
        {
            if (_isPulling)
            {
                dragger.Pull();
            }
            else
            {
                dragger.Release();
            }
        }

        private void Rotate()
        {
            Vector3 lookDirection = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Quaternion.Euler(0, -angle, 0);
        }

        private GameObject FrontDetect()
        {
            RaycastHit[] hits = new RaycastHit[2];
            Debug.DrawRay(topSensor.transform.position, transform.forward, Color.blue, 0.1f);
            Debug.DrawRay(botSensor.transform.position, transform.forward, Color.blue, 0.1f);
            Physics.Raycast(topSensor.transform.position, transform.forward, out hits[0], senseDistance, 1 << playerMask);
            Physics.Raycast(botSensor.transform.position, transform.forward, out hits[1], senseDistance, 1 << playerMask);
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    return hit.collider.gameObject;
                }
            }
            return null;
        }
    }
}