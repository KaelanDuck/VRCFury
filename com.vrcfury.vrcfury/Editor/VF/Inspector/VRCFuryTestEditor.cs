using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VF.Model;

namespace VF.Inspector {

    [CustomEditor(typeof(VRCFuryTest), true)]
    public class VRCFuryTestEditor : VRCFuryComponentEditor {
        public override VisualElement CreateEditor(SerializedObject serializedObject, UnityEngine.Component target, GameObject gameObject) {
            return VRCFuryEditorUtils.Error(
                "This avatar is a VRCFury editor test copy. Do not upload test copies, they are intended for" +
                " temporary in-editor testing only. Any changes made to this copy will be lost.");
        }
    }
    
}
