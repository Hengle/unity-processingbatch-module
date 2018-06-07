using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace ProcessingBatch
{
    public class ProcessingItemListDrawer : ReorderListDrawer
    {
        private float buttonWidth = 50;
        protected float padding = 5;
        private ProcessingBehaiver behaiver
        {
            get
            {
                return property.serializedObject.targetObject as ProcessingBehaiver;
            }
        }
        private List<GameObject> dragedGameObjects = new List<GameObject>();

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = DrawBoxRect(rect, index);
            var leftRect = new Rect(rect.x, rect.y, rect.width * 0.7f, rect.height);
            var prop = property.GetArrayElementAtIndex(index);
            var path_prop = prop.FindPropertyRelative("path");
            var item_prop = prop.FindPropertyRelative("item");
            path_prop.stringValue = EditorGUI.TextField(leftRect, path_prop.stringValue);
            if (string.IsNullOrEmpty(path_prop.stringValue) && item_prop.objectReferenceValue != null)
            {
                path_prop.stringValue = CalcutePath(item_prop.objectReferenceValue as GameObject);
            }
            var rightRect = new Rect(rect.x + rect.width * 0.7f, rect.y, rect.width * 0.28f, rect.height);
            EditorGUI.BeginChangeCheck();
            item_prop.objectReferenceValue = EditorGUI.ObjectField(rightRect, item_prop.objectReferenceValue, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                if (item_prop.objectReferenceValue != null)
                {
                    path_prop.stringValue = CalcutePath(item_prop.objectReferenceValue as GameObject);
                }
            }
        }

        private Rect DrawBoxRect(Rect rect, int index)
        {
            var boxRect = GetPaddingRect(rect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var labelRect = new Rect(boxRect.x, boxRect.y, 20, 20);
            EditorGUI.LabelField(labelRect, index.ToString());
            var innerRect = GetPaddingRect(rect, padding);
            return innerRect;
        }

        private Rect GetPaddingRect(Rect rect, float padding)
        {
            return new Rect(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);
        }

        protected override float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight + padding * 2;
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);

            var btnRect = new Rect(rect.x + rect.width - buttonWidth, rect.y, buttonWidth, rect.height);
            if (GUI.Button(btnRect, new GUIContent("re-find", "快速更新"), EditorStyles.miniButtonRight))
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    var prop = property.GetArrayElementAtIndex(i);
                    var path_prop = prop.FindPropertyRelative("path");
                    var item_prop = prop.FindPropertyRelative("item");
                    var newItem = FindFromPath(path_prop.stringValue);

                    if (newItem != null)
                    {
                        Debug.Log("更新：" + path_prop.stringValue);
                        item_prop.objectReferenceValue = newItem;
                    }
                    else
                    {
                        Debug.LogWarning("未成功更新：" + path_prop.stringValue);
                    }
                }
            }
        }

        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ProcessingUtil.GetDragRect();
            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ProcessingUtil.UpdateDragedGameObjectsFromScene(dragedGameObjects);
                if (dragedGameObjects.Count == 0)
                {
                    ProcessingUtil.UpdateDragedObjectsFromFile(".prefab", dragedGameObjects);
                }
            }
            else if (Event.current.type == EventType.dragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObjects)
                {
                    if(!ProcessingUtil.HaveItem(property,"item",item))
                    {
                       var prop = property.AddItem();
                        SetProperty(prop, item);
                    }
                    else
                    {
                        Debug.Log("already exists:" + item);
                    }
                }
            }
        }

        private void SetProperty(SerializedProperty prop,GameObject item)
        {
            if (prop == null || item == null) return;
            prop.FindPropertyRelative("item").objectReferenceValue = item;
            prop.FindPropertyRelative("path").stringValue = CalcutePath(item);
        }

        //计算路径
        private string CalcutePath(GameObject item)
        {
            List<string> path = new List<string>();
            path.Add(item.name);
            var literater = item;
            while (literater.transform.parent != null)
            {
                literater = literater.transform.parent.gameObject;
                if (literater == behaiver.root)
                {
                    break;
                }
                else
                {
                    path.Add(literater.name);
                }
            }
            path.Reverse();
            return string.Join("/", path.ToArray());
        }

        private GameObject FindFromPath(string path)
        {
            string[] array = path.Split('/');
            var item = behaiver.root;
            for (int i = 0; i < array.Length; i++)
            {
                var child = item.transform.Find(array[i]);
                if (child != null)
                {
                    item = child.gameObject;
                }
                else
                {
                    return null;
                }
            }
            return item;
        }
    }
}