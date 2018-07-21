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
using System.Linq;


namespace ProcessingBatch
{
    [CustomEditor(typeof(ProcessingBehaiver),true)]
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
        private const string prefer_selected = "prefer_ProcessingBehavierDrawer_selected";
        private Type[] _processingLogs;
        private Type[] processingLogs
        {
            get
            {
                if (_processingLogs == null)
                {
                    var types = typeof(ProcessingLogic).Assembly.GetTypes();
                    _processingLogs = (from type in types
                                       where type.IsSubclassOf(typeof(ProcessingLogic))
                                       where !type.IsAbstract
                                       select type
                            ).ToArray();
                }
                return _processingLogs;
            }

        }

        private void OnEnable()
        {
            InitPrefer();
            InitProps();
            InitLists();
            InitItemList();
        }
        private void InitPrefer()
        {
            if (PlayerPrefs.HasKey(prefer_selected))
            {
                selected = PlayerPrefs.GetInt(prefer_selected);
                if(options.Length <= selected)
                {
                    selected = 0;
                }
            }
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
            groupsList.onProcessAll = OnProcessAll;
        }

       
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if(logic_prop.objectReferenceValue == null)
            {
                DrawCreateNew();
            }
            else
            {
                DrawSwitch();

                if (toobarShow)
                {
                    DrawToolBars();
                }
                else
                {
                    groupsList.DoLayoutList();
                }
            }
         
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawCreateNew()
        {
            if(GUILayout.Button("create new processingLogic！"))
            {
                var logicNames = processingLogs.Select(x => new GUIContent(x.FullName)).ToArray();
                EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), logicNames, -1, (x, y, id) => {
                    var type = processingLogs[id];
                    var instence = ScriptableObject.CreateInstance(type);
                    ProjectWindowUtil.CreateAsset(instence, string.Format("new {0}.asset", type.Name));
                    DelyAccept(instence, (obj) =>
                    {
                        if (obj != null && obj is ProcessingLogic)
                        {
                            (target as ProcessingBehaiver).logic = obj as ProcessingLogic;
                            EditorUtility.SetDirty(target);// '
                        }
                    });
                }, null);
            }
        }

        private void DelyAccept(UnityEngine.Object instence,UnityEngine.Events.UnityAction<UnityEngine.Object> onCreate)
        {
            if (onCreate == null) return;
            EditorApplication.update = () =>
            {
                var path = AssetDatabase.GetAssetPath(instence);
                if(!string.IsNullOrEmpty(path))
                {
                    var obj = AssetDatabase.LoadAssetAtPath(path,instence.GetType());
                    onCreate(obj);
                    EditorApplication.update = null;
                }
            };
            
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
                PlayerPrefs.SetInt(prefer_selected, selected);
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

        private void OnProcessAll()
        {
            var groups = (target as ProcessingBehaiver).groups;

            if (logic_prop.objectReferenceValue != null)
            {
                var logic = logic_prop.objectReferenceValue as ProcessingLogic;
                logic.ProcessingAll(groups);
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