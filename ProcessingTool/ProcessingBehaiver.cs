#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-06-06 02:14:12
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ProcessingTool
{
    /// <summary>
    /// MonoBehaiver
    /// <summary>
    public class ProcessingBehaiver : UnityEngine.MonoBehaviour
    {
        public GameObject root;
        public ProcessingLogic logic;
        [HideInInspector]
        public List<ProcessingGroup> groups = new List<ProcessingGroup>();
    }
}