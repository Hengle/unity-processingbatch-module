using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProcessingBatch;
using System;
using UnityEngine.AI;
#if UNITY_EDITOR

using UnityEditor;

public class NaviStaticSetter : ProcessingLogic
{
    public enum Area
    {
        WalkAble = 0,
        NotWalkAble = 1,
        Jump = 2
    }
    [System.Serializable]
    public class Rule
    {
        public string key;
        public Area area;
    }

    public List<Rule> rules = new List<Rule>();

    public override void ProcessingGroup(ProcessingGroup group)
    {
        var rule = rules.Find(x => x.key == group.name);
        if (rule != null)
        {
            foreach (var processingItem in group.processingItems)
            {
                var go = processingItem.item;
                if (go == null)
                {
                    Debug.LogWarningFormat("忽略：{0}",processingItem.path);
                    continue;
                }

                var flags = GameObjectUtility.GetStaticEditorFlags(go);
                flags |= UnityEditor.StaticEditorFlags.NavigationStatic;
                GameObjectUtility.SetStaticEditorFlags(go, flags);
                GameObjectUtility.SetNavMeshArea(go, (int)rule.area);
                Debug.LogFormat(go, "设置{0}的NaviMeshArea为{1}", go.name, rule.area);
            }
            Debug.LogFormat("成功处理{0}，共{1}个对象", group.name, group.processingItems.Count);
        }
        else
        {
            Debug.LogFormat("未找到{0}对就的规则，无法处理", group.name);
        }

    }

    public override void ProcessingAll(List<ProcessingGroup> groups)
    {
        Debug.Log("按规则顺序批量处理中...");
        foreach (var rule in rules)
        {
            var group = groups.Find(x => x.name == rule.key);
            if (group != null)
            {
                ProcessingGroup(group);
            }
        }
    }
}
#endif
