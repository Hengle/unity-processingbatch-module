#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-06-06 03:16:23
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
    [CustomEditor(typeof(ProcessingBehaiver))]
    public class ProcessingBehavierDrawer : Editor
    {
        private SerializedProperty groups_prop;
        private SerializedProperty logic_prop;
        private ProcessingGroupListDrawer groupsList = new ProcessingGroupListDrawer();
        private ProcessingItemListDrawer itemList;
        private const float buttonWidth = 50;
        private bool? _toolbarShow;
        private bool toobarShow
        {
            get
            {
                if (_toolbarShow == null)
                {
                    _toolbarShow = EditorPrefs.GetBool("prefer_ProcessingGroupListDrawer_toolbarShow");
                }
                return (bool)_toolbarShow;
            }
            set
            {
                if (_toolbarShow != value)
                {
                    _toolbarShow = value;
                    EditorPrefs.SetBool("prefer_ProcessingGroupListDrawer_toolbarShow", value);
                }
            }
        }

        private string[] _options;
        private string[] options
        {
            get
            {
                if (_options == null || _options.Length == 0)
                {
                    _options = (target as ProcessingBehaiver).groups.ConvertAll(x => x.name).ToArray();
                }
                return _options;
            }
        }
        private int selected;

        private void OnEnable()
        {
            InitProps();
            InitLists();
            InitItemList();
        }

        private void InitProps()
        {
            groups_prop = serializedObject.FindProperty("groups");
            logic_prop = serializedObject.FindProperty("logic");
        }
        private void InitLists()
        {
            groupsList.InitReorderList(groups_prop);
            groupsList.onProcessGroup = OnProcessGroup;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSwitch();
            serializedObject.Update();
            if (toobarShow)
            {
                DrawToolBars();
            }
            else
            {
                groupsList.DoLayoutList();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSwitch()
        {
            var text = toobarShow ? "+" : "-";
            if (GUILayout.Button(text, EditorStyles.toolbarButton))
            {
                toobarShow = !toobarShow;
                if (toobarShow) ResetOptions();
            }
        }

        private void DrawToolBars()
        {
            EditorGUI.BeginChangeCheck();
            selected = GUILayout.Toolbar(selected, options);
            if (EditorGUI.EndChangeCheck())
            {
                InitItemList();
            }

            if (itemList != null)
            {
                itemList.DoLayoutList();
            }
        }
        private void InitItemList()
        {
            itemList = null;
            if (groups_prop.arraySize == 0) return;
            itemList = new ProcessingItemListDrawer();
            var prop = groups_prop.GetArrayElementAtIndex(selected);
            var list_prop = prop.FindPropertyRelative("processingItems");
            var name_prop = prop.FindPropertyRelative("name");
            itemList.InitReorderList(list_prop);
            itemList.drawHeaderCallback = (Rect rect) =>
            {
                var labelRect = new Rect(rect.x + 2, rect.y + 2, rect.width * 0.15f, EditorGUIUtility.singleLineHeight);
                var nameRect = new Rect(rect.x + 2 + rect.width * 0.15f, rect.y + 4, rect.width * 0.15f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, "name:");
                name_prop.stringValue = EditorGUI.TextField(nameRect, name_prop.stringValue, EditorStyles.miniTextField);

                var btnRect = new Rect(rect.x + rect.width * 0.35f, rect.y, 20, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(btnRect, new GUIContent("p", "批量处理"), EditorStyles.miniButton))
                {
                    OnProcessGroup(name_prop.stringValue);
                }
            };
        }


        private void OnProcessGroup(string groupName)
        {
            var group = (target as ProcessingBehaiver).groups.Find(x => x.name == groupName);
          
            if (logic_prop.objectReferenceValue != null)
            {
                var logic = logic_prop.objectReferenceValue as ProcessingLogic;
                logic.ProcessingGroup(group);
            }
            else
            {
                Debug.Log("如需批量处理,请填入继承于ProcessingLogic的ScriptObject");
            }
        }

     

        private void ResetOptions()
        {
            _options = null;
            selected = 0;
        }
    }
}