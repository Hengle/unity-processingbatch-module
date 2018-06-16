using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ProcessingBatch
{
    public abstract class ProcessingLogic:ScriptableObject
    {
        public abstract void ProcessingGroup(ProcessingGroup group);
        public abstract void ProcessingAll(List<ProcessingGroup> groups);
    }
}