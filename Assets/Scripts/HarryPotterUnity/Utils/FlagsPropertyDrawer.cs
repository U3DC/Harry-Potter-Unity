﻿using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace HarryPotterUnity.Utils
{
    public class EnumFlagsAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute)), UsedImplicitly]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {

        private const float SPACING = 1.4f;
        private const float BUTTON_WIDTH = 95f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (EditorGUIUtility.singleLineHeight + SPACING) * property.enumNames.Length;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int buttonsIntValue = 0;
            int enumLength = property.enumNames.Length;
            var buttonPressed = new bool[enumLength];

            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < enumLength; i++)
            {
                // Check if the button is/was pressed 
                if ((property.intValue & (1 << i)) == 1 << i)
                {
                    buttonPressed[i] = true;
                }

                var buttonPos = new Rect
                {
                    x = position.width - BUTTON_WIDTH,
                    y = position.y + (EditorGUIUtility.singleLineHeight*i*SPACING),
                    width = BUTTON_WIDTH,
                    height = EditorGUIUtility.singleLineHeight
                };

                buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], property.enumNames[i], "Button");

                if (buttonPressed[i])
                    buttonsIntValue += 1 << i;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = buttonsIntValue;
            }
        }
    }
#endif
}