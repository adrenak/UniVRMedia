using UnityEditor;
using UnityEngine;

namespace UniVRMedia {
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    class LayerAttributeDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}