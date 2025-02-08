using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Model;

namespace Model
{
    [CustomEditor(typeof(TetrisListTemplate))]
    public class TetrisListTemplateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TetrisListTemplate tetrisListTemplate = (TetrisListTemplate)target;

            // 添加“打印数据到控制台”按钮
            if (GUILayout.Button("打印数据到控制台"))
            {
                Debug.Log(tetrisListTemplate.template);
            }
        }
        
  
    }
}