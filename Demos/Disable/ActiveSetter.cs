using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProcessingBatch;
using System;
[CreateAssetMenu(menuName ="ActiveSetter")]
public class ActiveSetter : ProcessingLogic
{
    public string activeKey = "Active";
    public string disableKey = "InActive";

    public override void ProcessingAll(List<ProcessingGroup> groups)
    {
        foreach (var item in groups)
        {
            ProcessingGroup(item);
        }
    }

    public override void ProcessingGroup(ProcessingGroup group)
    {
        foreach (var item in group.processingItems)
        {
            if(group.name == activeKey)
            {
                item.item.SetActive(true);
            }
            else if(group.name == disableKey)
            {
                item.item.SetActive(false);
            }
        }
    }
}
