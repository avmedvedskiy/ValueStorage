using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace ValueStorage
{
    [CustomPropertyDrawer(typeof(EditableValueAttribute))]
    public class EditableValueAttributePropertyDrawer : ValueAttributePropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (storage == null)
            {
                FindAsset(property.serializedObject);
            }

            var keysRect = default(Rect);

            var keyProp = property.FindPropertyRelative("k");
            var valueProp = property.FindPropertyRelative("v");

            float width = position.width / 4f;

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, width, position.height);

            var propertyRect = new Rect(position.x + EditorGUIUtility.labelWidth + width + 10f, position.y, width, position.height);

            EditorGUI.LabelField(labelRect, label);

            switch (valueProp.propertyType)
            {
                case SerializedPropertyType.Integer: valueProp.intValue = EditorGUI.IntField(propertyRect, valueProp.intValue); break;
                case SerializedPropertyType.Float: valueProp.floatValue = EditorGUI.FloatField(propertyRect, valueProp.floatValue); break;
                case SerializedPropertyType.String: valueProp.stringValue = EditorGUI.TextField(propertyRect, valueProp.stringValue); break;
            }

            if (GUI.Button(buttonRect, GetAliaseByKey(valueProp, keyProp, storage), EditorStyles.popup))
            {
                keysRect = new Rect(position.x, position.y, position.width, position.height);

                if (storage)
                {
                    var keys = GetKeys(valueProp, storage);
                    var aliases = GetAliases(valueProp, storage);
                    SetSelectionIndexByKey(valueProp, keyProp, ref selectionIndex, storage);
                    selectionIndex = EditorGUI.Popup(keysRect, label.text, selectionIndex, aliases.ToArray());

                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        int index = i;
                        menu.AddItem(new GUIContent(aliases[index]), selectionIndex == index,
                            () =>
                            {
                                selectionIndex = index;
                                SetKey(valueProp, keyProp, selectionIndex, storage);
                                valueProp.serializedObject.ApplyModifiedProperties();
                            });
                    }

                    menu.ShowAsContext();
                }
                else
                {
                    Debug.LogErrorFormat("{0} constants [{1}] null", Target.GetType(), Target.storageName);
                }
            }

            EditorGUI.EndProperty();
        }

    }
}