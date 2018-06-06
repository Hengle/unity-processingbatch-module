#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-06-06 03:34:08
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
    public abstract class ReorderListDrawer
    {
        protected ReorderableList reorderList;
        protected SerializedProperty property;
        protected IList list;
        protected Type type;
        public ReorderableList.HeaderCallbackDelegate drawHeaderCallback { get; set; }
        public virtual void InitReorderList(SerializedProperty property)
        {
            this.property = property;
            reorderList = new ReorderableList(property.serializedObject, property);
            OnRegistEvents();
        }
        public virtual void InitReorderList(IList list, Type type)
        {
            this.list = list;
            this.type = type;
            reorderList = new ReorderableList(list, type);
            OnRegistEvents();
        }

        protected virtual void OnRegistEvents()
        {
            reorderList.drawHeaderCallback = DrawHeaderCallBack;
            reorderList.drawElementCallback = DrawElementCallBack;
            reorderList.elementHeightCallback = ElementHeightCallback;
        }

        protected abstract float ElementHeightCallback(int index);
        protected abstract void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused);
        protected virtual void DrawHeaderCallBack(Rect rect)
        {
            if (drawHeaderCallback != null)
                drawHeaderCallback.Invoke(rect);
        }
        public virtual void DoLayoutList()
        {
            reorderList.DoLayoutList();
        }
        public virtual void DoList(Rect rect)
        {
            reorderList.DoList(rect);
        }
    }
}