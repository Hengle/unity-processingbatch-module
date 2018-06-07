using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProcessingBatch;
using System;
using System.Linq;

public class TagAndColliderSetter : ProcessingLogic
{
    public List<Rule> rules = new List<Rule>();
    public List<ColliderInfoCatch> colliderCatchs = new List<ColliderInfoCatch>();

    [System.Serializable]
    public class Rule
    {
        public string matchKey;//关键字
        public bool setTag = true;
        public string tag;//tag
        public PrimitiveType primitiveType;
        public bool addCollider = true;
    }
    [System.Serializable]
    public class ColliderInfoCatch
    {
        public string path;
        public PrimitiveType primitiveType;
        public Vector3 center;
        public float radius;
        public float height;
        public int direction;
        public Vector3 size;

        public ColliderInfoCatch(PrimitiveType primitiveType, string path)
        {
            this.primitiveType = primitiveType;
            this.path = path;
        }
    }

    public override void ProcessingGroup(ProcessingGroup processingGroup)
    {
        var rule = rules.Find(x => x.matchKey == processingGroup.name);
        if (rule != null)
        {
            foreach (var processingItem in processingGroup.processingItems)
            {
                ChargeRule(rule, processingItem);
            }
            Debug.LogFormat("成功处理{0}，共{1}个对象", processingGroup.name, processingGroup.processingItems.Count);
        }
        else
        {
            Debug.LogWarningFormat("未找到规则文件，请配制:{0}", processingGroup.name);
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void ChargeRule(Rule rule, ProcessingItem processingItem)
    {
        var item = processingItem.item;
        if (item == null)
        {
            Debug.LogWarningFormat("忽略{0}", processingItem.path);
        }
        else
        {
            if (rule.setTag)
            {
                item.tag = rule.tag;
                Debug.LogFormat(item, "设置{0}的Tag为{1}", item, rule.tag);
            }
            if (rule.addCollider)
            {
                AttachCollider(rule.primitiveType, processingItem);
            }
        }
    }
    private Type GetColliderType(PrimitiveType type)
    {
        switch (type)
        {
            case PrimitiveType.Sphere:
                return(typeof(SphereCollider));
            case PrimitiveType.Capsule:
                return (typeof(CapsuleCollider));
            case PrimitiveType.Cylinder:
                return (typeof(CapsuleCollider));
            case PrimitiveType.Cube:
                return (typeof(BoxCollider));
            case PrimitiveType.Plane:
                return (typeof(MeshCollider));
            case PrimitiveType.Quad:
                return (typeof(MeshCollider));
            default:
                return (typeof(MeshCollider));
        }
    }

    private Collider AttachCollider(PrimitiveType primitiveType, ProcessingItem processingItem)
    {
        var catchInfo = colliderCatchs.Find(x => x.path == processingItem.path && x.primitiveType == primitiveType);
        var colliderType = GetColliderType(primitiveType);
        var item = processingItem.item;
        var collider = item.GetComponent<Collider>();
        if (collider != null && collider.GetType() != colliderType)
        {
            DestroyImmediate(collider);
        }
        if (collider == null)
        {
            collider = item.AddComponent(colliderType) as Collider;
            if (collider is MeshCollider){
                (collider as MeshCollider).convex = true;
            }

            if (catchInfo != null)
            {
                ApplyInfoToCollider(collider, catchInfo);
            }
            Debug.LogFormat(item, "设置{0}的Collider为{1}", item, collider);
        }
        else
        {
            RecordColliderInfo(primitiveType, processingItem.path, collider);
            Debug.LogFormat(item, "记录{0}的 Collider 信息", item);
        }
       
        return collider;
    }

    //应用记录的信息
    private void ApplyInfoToCollider(Collider collider, ColliderInfoCatch info)
    {
        if (collider == null || info == null) return;

        var type = GetColliderType(info.primitiveType);
        var newType = info.GetType();

        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
        foreach (var item in props)
        {
            var field = newType.GetField(item.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);
            if (field != null)
            {
                item.SetValue(collider, field.GetValue(info),new object[] { });
            }
        }
    }

    //记录信息
    private void RecordColliderInfo(PrimitiveType primitiveType,string path,Collider collider)
    {
        var catchInfo = colliderCatchs.Find(x => x.path == path && x.primitiveType == primitiveType);
        if(catchInfo == null)
        {
            catchInfo = new ColliderInfoCatch(primitiveType, path);
            colliderCatchs.Add(catchInfo);
        }

        var type = GetColliderType(primitiveType);
        var newType = catchInfo.GetType();
        Debug.LogFormat("[{0}]",type);
        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
        foreach (var item in props)
        {
            var field = newType.GetField(item.Name,System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.GetField);
            if(field != null){
                field.SetValue(catchInfo, item.GetValue(collider, new object[] { }));
            }
        }
    }
}
