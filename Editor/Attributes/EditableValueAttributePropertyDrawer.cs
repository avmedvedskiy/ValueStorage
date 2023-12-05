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

            var keyProp = property.FindPropertyRelative("k");
            var valueProp = property.FindPropertyRelative("v");

            bool readOnly = ((EditableValueAttribute)attribute).ReadOnly;
            
            var afterLabelPosition = EditorGUI.PrefixLabel(position, label);
            var defaultWidth = afterLabelPosition.width;
            afterLabelPosition.width = !readOnly? defaultWidth * 0.75f : defaultWidth;

            if (GUI.Button(afterLabelPosition, GetAliaseByKey(valueProp, keyProp, storage), EditorStyles.popup))
            {
                var keysRect = new Rect(position.x, position.y, position.width, position.height);

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

            if (!readOnly)
            {
                afterLabelPosition.x += afterLabelPosition.width;
                afterLabelPosition.width = defaultWidth * 0.25f;

                switch (valueProp.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        valueProp.intValue = EditorGUI.IntField(afterLabelPosition, valueProp.intValue);
                        break;
                    case SerializedPropertyType.Float:
                        valueProp.floatValue = EditorGUI.FloatField(afterLabelPosition, valueProp.floatValue);
                        break;
                    case SerializedPropertyType.String:
                        valueProp.stringValue = EditorGUI.TextField(afterLabelPosition, valueProp.stringValue);
                        break;
                }
            }

            EditorGUI.EndProperty();
        }

    }
}