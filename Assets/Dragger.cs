using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    //CONFIG:
    [SerializeField] Transform rightHandPos;
    [SerializeField] float throwAngle = 0.2f;
    [SerializeField] float baseThrowStrength = 5f;
    [SerializeField] float maxThrowStrength = 15f;
    [SerializeField] float StgGrowthPerSec = 5f;

    //STATE:
    GameObject holdingObj;
    bool isHolding = false;
    bool isPulling = false;
    float curThrowStrength = 0f;
    Coroutine pullingRoutine = null;

    //REF:
    [SerializeField] Animator animator = null;
    [SerializeField] AnimEvent animEvent = null;
    [SerializeField] Arrow arrow = null;

    private void OnEnable()
    {
        animEvent.trigger += Throw;
    }
    private void OnDisable()
    {
        animEvent.trigger -= Throw;
    }


    public void Hold(GameObject obj)
    {
        if (!isHolding)
        {
            holdingObj = obj;
            holdingObj.GetComponent<Rigidbody>().detectCollisions = false;
            holdingObj.GetComponent<Rigidbody>().isKinematic = true;
            holdingObj.transform.localPosition = rightHandPos.position;
            holdingObj.transform.SetParent(rightHandPos);
            isHolding = true;
        }
    }

    public void Pull()
    {
        if (holdingObj == null) return;
        isPulling = true;
        animator.SetBool("throwHold", isPulling);
        pullingRoutine = StartCoroutine(Pulling());
    }

    public void Release()
    {
        if (holdingObj == null && !isPulling) return;
        isPulling = false;
        animator.SetBool("throwHold", false);
    }

    private void Throw()
    {
        holdingObj.transform.parent = null;
        Rigidbody rb = holdingObj.GetComponent<Rigidbody>();
        rb.detectCollisions = true;
        rb.isKinematic = false;
        rb.AddForce((transform.forward + new Vector3(0, throwAngle, 0)) * curThrowStrength, ForceMode.Impulse);
        arrow.PointOut(0f);
        //This is an comment for commit
        isHolding = false;
    }

    private IEnumerator Pulling()
    {
        print("start routine");
        curThrowStrength = 0f;
        while (isPulling && curThrowStrength < maxThrowStrength)
        {
            arrow.PointOut(curThrowStrength / maxThrowStrength);
            curThrowStrength += maxThrowStrength * Time.deltaTime;
            print(curThrowStrength);
            yield return new WaitForFixedUpdate();
        }
    }
}
