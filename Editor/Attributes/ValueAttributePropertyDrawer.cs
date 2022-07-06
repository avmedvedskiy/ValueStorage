using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ValueStorage
{
    [CustomPropertyDrawer(typeof(ValueAttribute))]
    public class ValueAttributePropertyDrawer : PropertyDrawer
    {
        const string STORAGE_TYPE = "t:ValueStorage";

        protected int selectionIndex = 0;
        protected int constantsIndex = 0;
        protected ValueStorage storage = null;

        protected ValueAttribute Target
        {
            get { return attribute as ValueAttribute; }
        }

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

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, position.height);

            EditorGUI.LabelField(labelRect, label);
            if (GUI.Button(buttonRect, GetAliaseByKey(valueProp, keyProp, storage), EditorStyles.popup))
            {
                keysRect = new Rect(position.x, position.y, position.width, position.height);

                if (storage)
                {
                    var keys = GetKeys(valueProp, storage);
                    var aliases = GetAliases(valueProp, storage);
                    SetSelectionIndex(valueProp, ref selectionIndex, storage);
                    selectionIndex = EditorGUI.Popup(keysRect, label.text, selectionIndex, aliases.ToArray());

                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        int index = i;
                        menu.AddItem(new GUIContent(aliases[index]), selectionIndex == index,
                            () =>
                            {
                                selectionIndex = index;
                                SetConstant(valueProp, keyProp, selectionIndex, storage);
                                keyProp.serializedObject.ApplyModifiedProperties();
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

        protected void SetConstant(SerializedProperty property, SerializedProperty keyProperty, int selectionIndex,
            ValueStorage constants)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = selectionIndex != -1 && selectionIndex < constants.intValues.Count
                        ? constants.intValues[selectionIndex].v
                        : -1;
                    break;
                //case SerializedPropertyType.Float:
                //    property.floatValue = selectionIndex != -1 && selectionIndex < constants.floatValues.Count ? constants.floatValues[selectionIndex].value : 0.0f;
                //    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.floatValues.Count ? constants.floatValues[selectionIndex].key : string.Empty;
                //    break;
                case SerializedPropertyType.String:
                    property.stringValue = selectionIndex != -1 && selectionIndex < constants.stringValues.Count
                        ? constants.stringValues[selectionIndex].v
                        : string.Empty;
                    break;
            }

            switch (keyProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    keyProperty.intValue = selectionIndex != -1 && selectionIndex < constants.intValues.Count
                        ? constants.intValues[selectionIndex].v
                        : -1;
                    break;
                //case SerializedPropertyType.Float:
                //    property.floatValue = selectionIndex != -1 && selectionIndex < constants.floatValues.Count ? constants.floatValues[selectionIndex].value : 0.0f;
                //    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.floatValues.Count ? constants.floatValues[selectionIndex].key : string.Empty;
                //    break;
                case SerializedPropertyType.String:
                    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.stringValues.Count
                        ? constants.stringValues[selectionIndex].k
                        : string.Empty;
                    break;
            }
        }

        protected void FindAsset(SerializedObject obj)
        {
            bool shouldSearchInSameFolder = obj.targetObject is ScriptableObject;

            var assets = AssetDatabase.FindAssets(STORAGE_TYPE);
            var neededAssetPath = string.Empty;
            List<string> paths = new List<string>();
            for (int i = 0; i < assets.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                if (assetPath.Contains(Target.storageName))
                {
                    paths.Add(assetPath);
                }
            }

            if (paths.Count != 0)
                neededAssetPath = paths[0];

            if (shouldSearchInSameFolder)
            {
                string path = AssetDatabase.GetAssetPath(obj.targetObject);
                for (int i = 0; i < paths.Count; i++)
                {
                    if (Path.GetDirectoryName(path) == Path.GetDirectoryName(paths[i]))
                    {
                        neededAssetPath = paths[i];
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(neededAssetPath))
            {
                storage = AssetDatabase.LoadAssetAtPath<ValueStorage>(neededAssetPath);
            }
        }


        protected void SetKey(SerializedProperty property, SerializedProperty keyProperty, int selectionIndex,
            ValueStorage constants)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.intValues.Count
                        ? constants.intValues[selectionIndex].k
                        : string.Empty;
                    break;
                //case SerializedPropertyType.Float:
                //    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.floatValues.Count ? constants.floatValues[selectionIndex].key : string.Empty;
                //    break;
                case SerializedPropertyType.String:
                    keyProperty.stringValue = selectionIndex != -1 && selectionIndex < constants.stringValues.Count
                        ? constants.stringValues[selectionIndex].k
                        : string.Empty;
                    break;
            }
        }

        protected void SetSelectionIndex(SerializedProperty valueProperty, SerializedProperty keyProperty,
            ref int selectionIndex, ValueStorage constants)
        {
            switch (valueProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    selectionIndex = constants.intValues.FindIndex(x => x.k == keyProperty.stringValue);
                    break;
                //case SerializedPropertyType.Float:
                //    selectionIndex = constants.floatValues.FindIndex(x => x.key == keyProperty.stringValue);
                //    break;
                case SerializedPropertyType.String:
                    selectionIndex = constants.stringValues.FindIndex(x => x.k == keyProperty.stringValue);
                    break;
            }
        }

        protected List<string> GetKeys(SerializedProperty property, ValueStorage constants)
        {
            List<string> keys = new List<string>();
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    keys = constants.intValues.Select(x => x.k).ToList();
                    break;
                //case SerializedPropertyType.Float: keys = constants.floatValues.Select(x => x.k).ToList(); break;
                case SerializedPropertyType.String:
                    keys = constants.stringValues.Select(x => x.k).ToList();
                    break;
            }

            return keys;
        }

        protected List<string> GetAliases(SerializedProperty property, ValueStorage constants)
        {
            List<string> keys = new List<string>();
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    keys = constants.intValues.Select(x => x.a).ToList();
                    break;
                //case SerializedPropertyType.Float: keys = constants.floatValues.Select(x => x.a).ToList(); break;
                case SerializedPropertyType.String:
                    keys = constants.stringValues.Select(x => x.a).ToList();
                    break;
            }

            return keys;
        }

        protected string GetAliaseByKey(SerializedProperty valueProperty, SerializedProperty keyProperty,
            ValueStorage constants)
        {
            string alias = string.Empty;

            if (constants == null)
                return alias;

            switch (valueProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    IntAliasValue iv = null;
                    if (keyProperty.propertyType == SerializedPropertyType.String)
                        iv = constants.intValues.Find(x => x.k == keyProperty.stringValue);
                    else if (keyProperty.propertyType == SerializedPropertyType.Integer)
                        iv = constants.intValues.Find(x => x.v == keyProperty.intValue);

                    if (iv != null)
                        alias = iv.a;
                    break;
                //case SerializedPropertyType.Float: keys = constants.floatValues.Select(x => x.key).ToList(); break;
                case SerializedPropertyType.String:
                    var sv = constants.stringValues.Find(x => x.k == keyProperty.stringValue);
                    if (sv != null)
                        alias = sv.a;
                    break;
            }

            if (string.IsNullOrEmpty(alias))
                alias = keyProperty.propertyType == SerializedPropertyType.String
                    ? keyProperty.stringValue
                    : ""; // if not find alias set key;

            return alias;
        }

        protected List<string> GetKeys(ValueStorage constants)
        {
            List<string> keys = new List<string>();

            keys.AddRange(constants.intValues.Select(x => x.k).ToList());
            //keys.AddRange(constants.floatValues.Select(x => x.key).ToList());
            //keys.AddRange(constants.stringValues.Select(x => x.key).ToList());

            return keys;
        }

        protected void SetKey(SerializedProperty keyProperty, int selectionIndex, ValueStorage constants)
        {
            var keys = GetKeys(constants);
            keyProperty.stringValue = selectionIndex != -1 && selectionIndex < keys.Count
                ? keys[selectionIndex]
                : string.Empty;
        }

        protected SerializedPropertyType GetValueType(SerializedProperty keyProperty, ValueStorage constants)
        {
            SerializedPropertyType type = SerializedPropertyType.String;
            selectionIndex = constants.intValues.FindIndex(x => x.k == keyProperty.stringValue);
            if (selectionIndex != -1)
            {
                type = SerializedPropertyType.Integer;
            }

            //if (selectionIndex == -1)
            //{
            //    selectionIndex = constants.floatValues.FindIndex(x => x.key == keyProperty.stringValue);
            //    type = SerializedPropertyType.Float;
            //}

            //if (selectionIndex == -1)
            //{
            //    selectionIndex = constants.stringValues.FindIndex(x => x.key == keyProperty.stringValue);
            //    type = SerializedPropertyType.String;
            //}
            return type;
        }

        protected void SetSelectionIndexFromAll(SerializedProperty keyProperty, ref int selectionIndex,
            ValueStorage constants)
        {
            int count = 0;
            selectionIndex = constants.intValues.FindIndex(x => x.k == keyProperty.stringValue);

            //if (selectionIndex == -1)
            //{
            //    count += constants.intValues.Count;
            //    selectionIndex = constants.floatValues.FindIndex(x => x.key == keyProperty.stringValue);
            //}

            if (selectionIndex == -1)
            {
                count += constants.stringValues.Count;
                selectionIndex = constants.stringValues.FindIndex(x => x.k == keyProperty.stringValue);
            }

            selectionIndex += count;
        }

        protected void SetSelectionIndex(SerializedProperty valueProperty, ref int selectionIndex,
            ValueStorage constants)
        {
            switch (valueProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    selectionIndex = constants.intValues.FindIndex(x => x.v == valueProperty.intValue);
                    break;
                //case SerializedPropertyType.Float:
                //    selectionIndex = constants.floatValues.FindIndex(x => x.value == valueProperty.floatValue);
                //    break;
                case SerializedPropertyType.String:
                    selectionIndex = constants.stringValues.FindIndex(x => x.v == valueProperty.stringValue);
                    break;
            }
        }

        protected void SetSelectionIndexByKey(SerializedProperty valueProperty, SerializedProperty keyProperty,
            ref int selectionIndex, ValueStorage constants)
        {
            switch (valueProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    selectionIndex = constants.intValues.FindIndex(x => x.k == keyProperty.stringValue);
                    break;
                //case SerializedPropertyType.Float:
                //    selectionIndex = constants.floatValues.FindIndex(x => x.value == valueProperty.floatValue);
                //    break;
                case SerializedPropertyType.String:
                    selectionIndex = constants.stringValues.FindIndex(x => x.v == valueProperty.stringValue);
                    break;
            }
        }
    }
}