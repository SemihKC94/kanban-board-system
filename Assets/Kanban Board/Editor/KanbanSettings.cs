using UnityEngine;
using System.Collections.Generic;
using UnityEditor; 

namespace SKC.KanbanSystem
{
    /// <summary>
    /// ScriptableObject for storing global Kanban board settings and preferences.
    /// </summary>
    public class KanbanSettings : ScriptableObject
    {
        private static KanbanSettings s_Instance;
        /// <summary>
        /// Singleton instance of the KanbanSettings. Creates the asset if it doesn't exist.
        /// </summary>
        public static KanbanSettings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    // Try to find the existing settings asset
                    string[] guids = AssetDatabase.FindAssets("t:KanbanSettings");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        s_Instance = AssetDatabase.LoadAssetAtPath<KanbanSettings>(path);
                    }
                    else // If asset not found, create one
                    {
                        Debug.LogWarning("Kanban Settings asset not found. Creating a new one. Consider using Assets/Create/Kanban System/Kanban Settings for proper placement.");
                        CreateKanbanSettingsAssetInternal(); // Internal method to create and assign instance
                    }
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// Creates a new Kanban Settings asset in the project.
        /// Accessible via Assets/Create/Kanban System/Kanban Settings.
        /// </summary>
        [MenuItem("Assets/Create/Kanban System/Kanban Settings")]
        public static void CreateKanbanSettingsAsset()
        {
            CreateKanbanSettingsAssetInternal();
        }

        // Internal method to create and assign the settings asset.
        private static void CreateKanbanSettingsAssetInternal()
        {
            KanbanSettings newSettings = CreateInstance<KanbanSettings>();
            string path = "Assets/Kanban Board/Editor/Resources/KanbanSettings.asset"; // Default path for editor resources
            string directory = System.IO.Path.GetDirectoryName(path);

            if (!AssetDatabase.IsValidFolder(directory))
            {
                // Create folder structure if it doesn't exist
                string[] parts = directory.Split('/');
                string currentPath = "Assets";
                for(int i = 1; i < parts.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder(currentPath + "/" + parts[i]))
                    {
                        AssetDatabase.CreateFolder(currentPath, parts[i]);
                    }
                    currentPath += "/" + parts[i];
                }
            }
            
            AssetDatabase.CreateAsset(newSettings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            s_Instance = newSettings; // Assign new settings as the singleton instance
            Debug.Log($"Kanban Settings asset created at: {path}");
        }


        [Header("Column Colors")]
        /// <summary>
        /// Color for "To Do" column background.
        /// </summary>
        public Color toDoColumnColor = new Color(0.8f, 0.8f, 0.8f, 0.5f); 
        /// <summary>
        /// Color for "In Progress" column background.
        /// </summary>
        public Color inProgressColumnColor = new Color(0.8f, 0.8f, 0.5f, 0.5f); 
        /// <summary>
        /// Color for "Done" column background.
        /// </summary>
        public Color doneColumnColor = new Color(0.5f, 0.8f, 0.5f, 0.5f); 

        [Header("Column Header Colors")]
        /// <summary>
        /// Color for "To Do" column header text.
        /// </summary>
        public Color toDoHeaderColor = Color.black;
        /// <summary>
        /// Color for "In Progress" column header text.
        /// </summary>
        public Color inProgressHeaderColor = Color.black;
        /// <summary>
        /// Color for "Done" column header text.
        /// </summary>
        public Color doneHeaderColor = Color.black;

        [Header("Card Colors")]
        /// <summary>
        /// Background color for "To Do" cards.
        /// </summary>
        public Color toDoCardColor = new Color(0.9f, 0.9f, 0.9f); 
        /// <summary>
        /// Background color for "In Progress" cards.
        /// </summary>
        public Color inProgressCardColor = new Color(1f, 1f, 0.8f); 
        /// <summary>
        /// Background color for "Done" cards.
        /// </summary>
        public Color doneCardColor = new Color(0.8f, 1f, 0.8f); 
        /// <summary>
        /// Background color for cards when hovered over.
        /// </summary>
        public Color cardHoverColor = new Color(0.7f, 0.7f, 1f); 

        [Header("Card Text Colors")]
        /// <summary>
        /// Color for card title text.
        /// </summary>
        public Color titleTextColor = Color.black;
        /// <summary>
        /// Font size for card title text.
        /// </summary>
        public int cardTitleFontSize = 12;
        /// <summary>
        /// Font asset for card title text.
        /// </summary>
        public Font titleFont; 
        /// <summary>
        /// Color for card description text.
        /// </summary>
        public Color descriptionTextColor = Color.gray;
        /// <summary>
        /// Font size for card description text.
        /// </summary>
        public int cardDescriptionFontSize = 10;
        /// <summary>
        /// Font asset for card description text.
        /// </summary>
        public Font descriptionFont; 
        
        /// <summary>
        /// Color for assigned developer text on cards.
        /// </summary>
        public Color assignedToColor = Color.blue; 

        [Header("Status Icons")]
        /// <summary>
        /// Whether to display status icons on cards.
        /// </summary>
        public bool showStatusIcons = true;
        /// <summary>
        /// Icon for "To Do" status.
        /// </summary>
        public Texture2D toDoIcon;
        /// <summary>
        /// Icon for "In Progress" status.
        /// </summary>
        public Texture2D inProgressIcon;
        /// <summary>
        /// Icon for "Done" status.
        /// </summary>
        public Texture2D doneIcon;

        [Header("Developer Profiles")]
        /// <summary>
        /// List of all developer profiles available for assignment.
        /// </summary>
        public List<DeveloperProfile> allDevelopers = new List<DeveloperProfile>();


        /// <summary>
        /// Saves the changes made to the ScriptableObject asset.
        /// </summary>
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Resets all Kanban settings to their default values.
        /// </summary>
        public void ResetToDefaults()
        {
            toDoCardColor = new Color(0.8f, 0.8f, 0.95f, 1f);
            inProgressCardColor = new Color(0.95f, 0.95f, 0.8f, 1f);
            doneCardColor = new Color(0.8f, 0.95f, 0.8f, 1f);

            cardHoverColor = new Color(0.7f, 0.7f, 0.9f, 1f);
            
            titleTextColor = Color.black;
            descriptionTextColor = Color.gray;
            assignedToColor = Color.blue; 

            titleFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            descriptionFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            cardTitleFontSize = 12;
            cardDescriptionFontSize = 10;

            toDoIcon = null; 
            inProgressIcon = null;
            doneIcon = null;

            allDevelopers.Clear(); 

            Save();
        }
    }
}