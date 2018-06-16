using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProcessingBatch;
using System;

public class LayerSetter : ProcessingLogic
{
    public bool ergodic;//遍历
    public List<LayerSetterRule> rules = new List<LayerSetterRule>();
    [System.Serializable]
    public class LayerSetterRule
    {
        public string matchKey;//关键字
        public string layer;//层
    }

    public override void ProcessingGroup(ProcessingGroup group)
    {
        var rule = rules.Find(x => x.matchKey == group.name);
        if (rule != null)
        {
            var items = CollectItems(group.processingItems);
            foreach (var item in items)
            {
                item.layer = LayerMask.NameToLayer(rule.layer);
                Debug.LogFormat(item, "设置{0}的层级为{1}", item.name, rule.layer);
            }
            Debug.LogFormat("成功处理{0}，共{1}个对象", group.name, group.processingItems.Count);
        }
        else
        {
            Debug.LogWarningFormat("未找到规则文件，请配制:{0}", group.name);
        }
    }
    private List<GameObject> CollectItems(List<ProcessingItem> processItems)
    {
        var itemList = new List<GameObject>();
        foreach (var processItem in processItems)
        {
            if (processItem.item != null)
            {
                if (!itemList.Contains(processItem.item))
                {
                    itemList.Add(processItem.item);
                }
            }
            else
            {
                Debug.Log("ignore:" + processItem.path);
            }
        }
        if (!ergodic)
        {
            return itemList;
        }
        else
        {
            var array = itemList.ToArray();
            foreach (var item in array)
            {
                RetiveTransform(item.transform, itemList);
            }
            return itemList;
        }
    }
    private void RetiveTransform(Transform parent, List<GameObject> itemList)
    {
        if (!itemList.Contains(parent.gameObject))
        {
            itemList.Add(parent.gameObject);
        }
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                RetiveTransform(child, itemList);
            }
        }
    }

    public override void ProcessingAll(List<ProcessingGroup> groups)
    {
        foreach (var group in groups)
        {
            ProcessingGroup(group);
        }
    }
}
