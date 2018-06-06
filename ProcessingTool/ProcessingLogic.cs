using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace ProcessingTool
{
    public abstract class ProcessingLogic:ScriptableObject
    {
        public abstract void ProcessingGroup(ProcessingGroup group);
    }
}