using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PureMVC;
using System;

public class StateAngleRegister : MonoBehaviour {
    public Vector3 axis;
    private string key { get { return EventKey.StageAngle; } }

    private Quaternion startRot;
    private void Start()
    {
        startRot = transform.localRotation;
        SceneMain.Current.RegisterEvent<float>(key, OnAngleChanged);
    }

    private void OnAngleChanged(float obj)
    {
        transform.localRotation = Quaternion.Euler(axis * obj) * startRot;
    }

    private void OnDestroy()
    {
        SceneMain.Current.RemoveEvent<float>(key, OnAngleChanged);
    }
}
