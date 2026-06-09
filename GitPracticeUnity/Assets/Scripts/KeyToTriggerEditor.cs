using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(KeyToTriggerManager))]
public class KeyToTriggerEditor : Editor
{
    private string[] _orderedKeyNames;
    private KeyCode[] _orderedKeyCodes;

    private void OnEnable()
    {
        PrepareSortedKeys();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        KeyToTriggerManager manager = (KeyToTriggerManager)target;

        // On va chercher les triggers directement depuis l'asset de ton panneau Project
        string[] triggerNames = GetAnimatorTriggers(manager.animatorController);

        SerializedProperty listProp = serializedObject.FindProperty("listeLiaisons");

        // Affichage des champs de configuration
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animatorController"), new GUIContent("Animator Controller (Project)"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animatorCible"), new GUIContent("Animator Cible (Scène)"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Liaisons Touches -> Triggers", EditorStyles.boldLabel);

        for (int i = 0; i < listProp.arraySize; i++)
        {
            SerializedProperty element = listProp.GetArrayElementAtIndex(i);
            SerializedProperty nomDesc = element.FindPropertyRelative("nomDescription");
            SerializedProperty touche = element.FindPropertyRelative("toucheClavier");
            SerializedProperty trigger = element.FindPropertyRelative("triggerAnimator");

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(nomDesc, new GUIContent("Nom Repère"));

            // Dropdown touches (Lettres en premier)
            KeyCode currentKey = (KeyCode)touche.intValue;
            int currentKeyIndex = Array.IndexOf(_orderedKeyCodes, currentKey);
            if (currentKeyIndex == -1) currentKeyIndex = 0;

            int newKeyIndex = EditorGUILayout.Popup("Touche Clavier", currentKeyIndex, _orderedKeyNames);
            touche.intValue = (int)_orderedKeyCodes[newKeyIndex];

            // Dropdown Triggers liés à l'Asset
            if (triggerNames.Length > 0)
            {
                int currentIndex = Mathf.Max(0, Array.IndexOf(triggerNames, trigger.stringValue));
                int newIndex = EditorGUILayout.Popup("Trigger Animator", currentIndex, triggerNames);
                trigger.stringValue = triggerNames[newIndex];
            }
            else
            {
                EditorGUILayout.HelpBox("Aucun trigger trouvé. Glisse un Animator Controller valide depuis ton panneau Project.", MessageType.Warning);
                EditorGUILayout.PropertyField(trigger, new GUIContent("Trigger (Nom manuel)"));
            }

            if (GUILayout.Button("Supprimer cette liaison", EditorStyles.miniButtonRight))
            {
                listProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Ajouter une nouvelle liaison"))
        {
            listProp.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void PrepareSortedKeys()
    {
        List<KeyCode> allKeys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Distinct().ToList();
        List<KeyCode> letters = new List<KeyCode>();
        List<KeyCode> others = new List<KeyCode>();

        foreach (KeyCode key in allKeys)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                letters.Add(key);
            else
                others.Add(key);
        }

        others = others.OrderBy(k => k.ToString()).ToList();

        List<KeyCode> finalSortedList = new List<KeyCode>();
        finalSortedList.AddRange(letters);
        finalSortedList.AddRange(others);

        _orderedKeyCodes = finalSortedList.ToArray();
        _orderedKeyNames = finalSortedList.Select(k => k.ToString()).ToArray();
    }

    // Modification ici pour lire directement l'asset RuntimeAnimatorController
    private string[] GetAnimatorTriggers(RuntimeAnimatorController runtimeController)
    {
        if (runtimeController == null)
            return new string[0];

        List<string> triggers = new List<string>();

        // Cast vers le type Editor pour lire les paramètres du fichier d'origine
        var controller = runtimeController as UnityEditor.Animations.AnimatorController;

        if (controller != null)
        {
            foreach (var parameter in controller.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    triggers.Add(parameter.name);
                }
            }
        }
        return triggers.ToArray();
    }
}