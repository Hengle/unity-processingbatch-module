using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace ProcessingBatch
{
    [System.Serializable]
    public class ProcessingGroup
    {
        public string name;
        public List<ProcessingItem> processingItems = new List<ProcessingItem>();
    }
}