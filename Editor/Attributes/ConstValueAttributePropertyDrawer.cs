using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace ValueStorage
{
    [CustomPropertyDrawer(typeof(ConstValueAttribute))]
    public class ConstValueAttributePropertyDrawer : ValueAttributePropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (storage == null)
            {
                FindAsset(property.serializedObject);
            }

            if (property.serializedObject.hasModifiedProperties)
                property.serializedObject.ApplyModifiedProperties();

            property.serializedObject.Update();

            var keyProp = property;
            var valueProp = property;
            
            
            position = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(position, GetAliaseByKey(valueProp, keyProp, storage), EditorStyles.popup))
            {
                var keysRect = new Rect(position.x, position.y, position.width, position.height);

                if (storage)
                {
                    var keys = GetKeys(valueProp, storage);
                    var aliases = GetAliases(valueProp, storage);
                    SetSelectionIndex(valueProp, ref selectionIndex, storage);
                    selectionIndex = EditorGUI.Popup(keysRect, label.text, selectionIndex, aliases.ToArray());

                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Edit"), false, () => { Selection.activeObject = storage; });
                    menu.AddSeparator("");
                    
                    for (int i = 0; i < keys.Count; i++)
                    {
                        int index = i;
                        menu.AddItem(new GUIContent(aliases[index]), selectionIndex == index,
                            () =>
                            {
                                valueProp.serializedObject.Update();
                                selectionIndex = index;
                                SetConstant(valueProp, keyProp, selectionIndex, storage);
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
