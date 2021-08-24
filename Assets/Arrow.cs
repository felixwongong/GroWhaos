using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    //CONFIG:
    [SerializeField] float maxArrowLength = 3.5f;

    //REF:
    LineRenderer arrow;

    private void Awake()
    {
        arrow = GetComponent<LineRenderer>();
    }

    public void PointOut(float percentage)
    {
        arrow.enabled = !Mathf.Approximately(percentage, 0);
        arrow.SetPosition(1, new Vector3(0, 0, percentage * maxArrowLength));
    }
}
