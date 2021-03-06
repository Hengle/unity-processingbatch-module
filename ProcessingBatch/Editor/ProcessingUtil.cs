﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace ProcessingBatch
{
    public static class ProcessingUtil
    {
        public const float smallButtonWidth = 20f;
        public const float middleButtonWidth = 45f;
        public const float bigButtonWidth = 60f;
        public const float padding = 5;
        public static string searchWord;
        public static Color IgnoreColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.red : Color.black;
            }
        }
        public static Color NormalColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.green : Color.white;
            }
        }
        public static Color WarningColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.yellow : Color.white;
            }
        }
        public static Color MatchColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? Color.cyan : Color.white;
            }
        }
        public static SerializedProperty AddItem(this SerializedProperty arrayProp)
        {
            var size = arrayProp.arraySize;
            arrayProp.InsertArrayElementAtIndex(size);
            return arrayProp.GetArrayElementAtIndex(size);
        }

        public static bool HaveItem(SerializedProperty property, string path, UnityEngine.Object obj)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                var prop = property.GetArrayElementAtIndex(i).FindPropertyRelative(path);
                var item = prop.objectReferenceValue;
                if (item == obj)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 手动把脚本绘制出来
        /// </summary>
        /// <param name="script_prop"></param>
        public static void DrawDisableProperty(SerializedProperty script_prop)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script_prop, true);
            EditorGUI.EndDisabledGroup();
        }
        /// <summary>
        /// 更新从场景拖入的gameObject
        /// </summary>
        /// <param name="dragedGameObject"></param>
        public static void UpdateDragedGameObjectsFromScene(List<GameObject> dragedGameObject)
        {
            dragedGameObject.Clear();
            foreach (var item in DragAndDrop.objectReferences)
            {
                if (item is GameObject)
                {
                    var path = AssetDatabase.GetAssetPath(item);
                    if (string.IsNullOrEmpty(path) && !dragedGameObject.Contains(item as GameObject))
                    {
                        dragedGameObject.Add(item as GameObject);
                    }
                    else
                    {
                        Debug.Log("ignre:" + item);
                    }
                }
            }
            DragAndDrop.visualMode = dragedGameObject.Count > 0 ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
        }
        /// <summary>
        /// address: ".prefab"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="dragedObjects"></param>
        public static void UpdateDragedObjectsFromFile<T>(string address, List<T> dragedObjects) where T : UnityEngine.Object
        {
            dragedObjects.Clear();
            foreach (var item in DragAndDrop.objectReferences)
            {
                if (item is T)
                {
                    dragedObjects.Add(item as T);
                }
                else if (ProjectWindowUtil.IsFolder(item.GetInstanceID()))
                {
                    var folder = AssetDatabase.GetAssetPath(item);
                    SearchDeep(folder, address, dragedObjects);
                }
            }
            DragAndDrop.visualMode = dragedObjects.Count > 0 ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
        }

        public static void SearchDeep<T>(string folder, string address, List<T> list) where T : UnityEngine.Object
        {
            var files = System.IO.Directory.GetFiles(folder, "*" + address, System.IO.SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                var root = System.IO.Path.GetPathRoot(filePath);

                if (filePath.EndsWith(address))
                {
                    var path = filePath.Substring(root.Length);
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (asset != null)
                    {
                        list.Add(asset);
                    }
                }
            }
        }
        public static Rect DrawBoxRect(Rect orignalRect, string index)
        {
            var idRect = new Rect(orignalRect.x - padding, orignalRect.y + padding, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var boxRect = PaddingRect(orignalRect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }
        public static Rect PaddingRect(Rect orignalRect, float padding = padding)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }

        public static Rect GetDragRect()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            return rect;
        }

    }
}