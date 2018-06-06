#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-06-06 04:08:56
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace ProcessingTool
{

    public class ProcessingGroupListDrawer : ReorderListDrawer
    {
        private Dictionary<int, ProcessingItemListDrawer> processItemListDic = new Dictionary<int, ProcessingItemListDrawer>();
        public UnityEngine.Events.UnityAction<string> onProcessGroup { get; set; }
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ProcessingUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            var list_prop = prop.FindPropertyRelative("processingItems");
            var name_prop = prop.FindPropertyRelative("name");
            var processItemList = GetProcessItemListDrawer(list_prop, index);

            processItemList.drawHeaderCallback = (titleRect) =>
            {
                var labelRect = new Rect(rect.x + 2, rect.y  +2, rect.width * 0.15f,EditorGUIUtility.singleLineHeight);
                var nameRect = new Rect(rect.x + 2 + rect.width * 0.15f,rect.y + 4,rect.width * 0.15f,EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect,"name:");
                name_prop.stringValue = EditorGUI.TextField(nameRect, name_prop.stringValue,EditorStyles.miniTextField);

                var btnRect = new Rect(rect.x + rect.width * 0.35f, rect.y, 20, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(btnRect, new GUIContent("p", "批量处理"), EditorStyles.miniButton))
                {
                    if(onProcessGroup != null)
                    {
                        onProcessGroup.Invoke(name_prop.stringValue);
                    }
                    else
                    {
                        Debug.Log("如需批量处理,请填入继承于ProcessingLogic的ScriptObject");
                    }
                }
            };
            processItemList.DoList(rect);
        }

        private ProcessingItemListDrawer GetProcessItemListDrawer(SerializedProperty prop, int index)
        {
            if (!processItemListDic.ContainsKey(index) || processItemListDic[index] == null)
            {
                processItemListDic[index] = new ProcessingItemListDrawer();
                processItemListDic[index].InitReorderList(prop);
            }
            var processItemList = processItemListDic[index];
            return processItemList;
        }

        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            var arraySize = prop.FindPropertyRelative("processingItems").arraySize;
            return (Mathf.Max(arraySize, 1) + 2) * (EditorGUIUtility.singleLineHeight + ProcessingUtil.padding * 2);
        }
        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);
            EditorGUI.LabelField(rect, "分组列表");
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
        }
    }
}
