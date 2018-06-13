using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProcessingBatch;
using System;

#if UNITY_EDITOR

using UnityEditor;
public class ScriptBinding : ProcessingBatch.ProcessingLogic
{
    public List<Rule> rules = new List<Rule>();
    [Serializable]
    public class Rule
    {
        public string key;
        public MonoScript script;
    }

    public override void ProcessingGroup(ProcessingGroup group)
    {
        var rule = rules.Find(x => x.key == group.name);
        if (rule == null)
        {
            Debug.LogError("缺少绑定规则：" + group.name);
        }
        else
        {
            foreach (var processingItem in group.processingItems)
            {
                if (processingItem.item != null)
                {
                  var script =  processingItem.item.GetComponent(rule.script.GetClass());
                    if (script == null)
                    {
                        processingItem.item.AddComponent(rule.script.GetClass());
                        Debug.LogFormat("在物体{0}上添加一个脚本：{1}",processingItem.item,rule.script);
                    }
                    else
                    {
                        Debug.LogFormat("物体{0}上已经存在脚本：{1}", processingItem.item, rule.script);
                    }
                }
            }
        }
       
    }
}
#endif

