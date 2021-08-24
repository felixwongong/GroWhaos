using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    public event Action trigger = null;

    public void Trigger()
    {
        if (trigger != null) trigger();
    }
}
