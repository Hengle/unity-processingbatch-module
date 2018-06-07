using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace ProcessingBatch
{
    public abstract class ProcessingLogic:ScriptableObject
    {
        public abstract void ProcessingGroup(ProcessingGroup group);
    }
}