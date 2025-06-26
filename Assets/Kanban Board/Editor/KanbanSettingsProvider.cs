using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SKC.KanbanSystem
{
    /// <summary>
    /// Provides the custom settings panel for Kanban Board within Unity's Project Settings.
    /// </summary>
    public class KanbanSettingsProvider : SettingsProvider
    {
        private SerializedObject m_KanbanSettings;
        private bool settingsChanged = false;

        public KanbanSettingsProvider(string path, SettingsScope scopes = SettingsScope.User)
            : base(path, scopes) { }

        /// <summary>
        /// Creates the SettingsProvider instance for Kanban Board settings.
        /// </summary>
        /// <returns>A new SettingsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateKanbanSettingsProvider()
        {
            // Ensure the KanbanSettings asset exists (it will be created if not found).
            KanbanSettings.instance.ToString(); 

            var provider = new KanbanSettingsProvider("Project/SKC/Kanban Board Settings", SettingsScope.Project);
            provider.keywords = new HashSet<string>(new[] { "Kanban", "Board", "Card", "Color", "Font", "Background", "To Do", "In Progress", "Done", "Text Color", "Title Color", "Description Color", "Assigned To Color", "Icon", "Developer" }); 
            return provider;
        }

        /// <summary>
        /// Called when the settings panel is activated (opened).
        /// </summary>
        /// <param name="searchContext">The current search string.</param>
        /// <param name="rootElement">The root VisualElement for UI Toolkit (not used in IMGUI).</param>
        public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
        {
            m_KanbanSettings = new SerializedObject(KanbanSettings.instance);
            settingsChanged = false;
        }

        /// <summary>
        /// Draws the GUI for the Kanban Board settings panel.
        /// </summary>
        /// <param name="searchContext">The current search string.</param>
        public override void OnGUI(string searchContext)
        {
            m_KanbanSettings.Update();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Card Colors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("toDoCardColor"), new GUIContent("To Do Card Color"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("inProgressCardColor"), new GUIContent("In Progress Card Color"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("doneCardColor"), new GUIContent("Done Card Color"));
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Text Colors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("titleTextColor"), new GUIContent("Title Text Color"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("descriptionTextColor"), new GUIContent("Description Text Color"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("assignedToColor"), new GUIContent("Assigned To Color")); 
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Fonts", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("titleFont"), new GUIContent("Title Font"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("descriptionFont"), new GUIContent("Description Font"));

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Status Icons", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("toDoIcon"), new GUIContent("To Do Icon"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("inProgressIcon"), new GUIContent("In Progress Icon"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("doneIcon"), new GUIContent("Done Icon"));

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Other Visual Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("cardHoverColor"), new GUIContent("Card Hover Color"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("cardTitleFontSize"), new GUIContent("Card Title Font Size"));
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("cardDescriptionFontSize"), new GUIContent("Card Description Font Size"));
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Developers", EditorStyles.boldLabel); 
                EditorGUILayout.PropertyField(m_KanbanSettings.FindProperty("allDevelopers"), new GUIContent("Manage Developers"), true); 

            }
            if (EditorGUI.EndChangeCheck())
            {
                settingsChanged = true;
                m_KanbanSettings.ApplyModifiedPropertiesWithoutUndo(); 
            }

            EditorGUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            /// <summary>
            /// Button to reset all Kanban settings to their default values.
            /// </summary>
            if (GUILayout.Button("Reset to Default", GUILayout.Width(120), GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset all Kanban Board settings to their default values?", "Yes", "No"))
                {
                    KanbanSettings.instance.ResetToDefaults();
                    m_KanbanSettings = new SerializedObject(KanbanSettings.instance); 
                    settingsChanged = false;
                    EditorWindow.GetWindow<KanbanWindow>()?.Repaint();
                }
            }

            EditorGUI.BeginDisabledGroup(!settingsChanged);
            /// <summary>
            /// Button to apply changes made in the settings panel.
            /// </summary>
            if (GUILayout.Button("Apply", GUILayout.Width(80), GUILayout.Height(25)))
            {
                KanbanSettings.instance.Save();
                settingsChanged = false;
                EditorWindow.GetWindow<KanbanWindow>()?.Repaint();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }
    }
}