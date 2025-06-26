using UnityEngine;
using UnityEditor;
using System.Linq; 
using System.Collections.Generic; 

namespace SKC.KanbanSystem
{
    /// <summary>
    /// Editor window for editing Kanban card details.
    /// </summary>
    public class KanbanCardEditorWindow : EditorWindow
    {
        private KanbanCardData currentCard;
        // Temporary variables to hold changes before saving.
        private string tempTitle;
        private string tempDescription;
        private KanbanColumnType tempColumn;
        private DeveloperProfile tempAssignedTo;

        /// <summary>
        /// Opens the Kanban card editor window for a specific card.
        /// </summary>
        /// <param name="card">The KanbanCardData to be edited.</param>
        public static void Open(KanbanCardData card)
        {
            KanbanCardEditorWindow window = GetWindow<KanbanCardEditorWindow>("Edit Card");
            window.currentCard = card;
            // Load current card data into temporary variables
            window.tempTitle = card.title;
            window.tempDescription = card.description;
            window.tempColumn = card.currentColumn;
            window.tempAssignedTo = card.assignedTo;

            window.ShowUtility(); // Open as a utility window (small, focused)
            window.minSize = new Vector2(300, 200);
            window.maxSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            if (currentCard == null)
            {
                EditorGUILayout.HelpBox("No card selected for editing.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Edit Card Details", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Title field
            EditorGUILayout.LabelField("Title", EditorStyles.boldLabel);
            tempTitle = EditorGUILayout.TextArea(tempTitle, GUILayout.Height(30));
            
            EditorGUILayout.Space(10);

            // Description field
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            tempDescription = EditorGUILayout.TextArea(tempDescription, GUILayout.Height(100));

            EditorGUILayout.Space(10);

            // Column selection
            tempColumn = (KanbanColumnType)EditorGUILayout.EnumPopup("Column", tempColumn);

            EditorGUILayout.Space(10);

            // Assigned Developer selection
            if (KanbanSettings.instance != null && KanbanSettings.instance.allDevelopers != null)
            {
                List<string> developerNamesList = KanbanSettings.instance.allDevelopers
                                                    .Select(dev => dev.developerName)
                                                    .ToList();
                developerNamesList.Insert(0, "Unassigned"); 
                string[] developerNamesArray = developerNamesList.ToArray();

                int currentIndex = 0; 
                if (tempAssignedTo != null)
                {
                    currentIndex = KanbanSettings.instance.allDevelopers.IndexOf(tempAssignedTo) + 1; 
                }
                
                int newIndex = EditorGUILayout.Popup("Assigned To", currentIndex, developerNamesArray);

                if (newIndex != currentIndex)
                {
                    if (newIndex == 0) // "Unassigned" selected
                    {
                        tempAssignedTo = null;
                    }
                    else
                    {
                        tempAssignedTo = KanbanSettings.instance.allDevelopers[newIndex - 1]; 
                    }
                }
            }
            else
            {
                // Fallback for ObjectField if settings are not loaded
                EditorGUILayout.HelpBox("Developer profiles not loaded or not set up in KanbanSettings. Please ensure KanbanSettings asset exists and 'allDevelopers' list is populated.", MessageType.Warning);
                tempAssignedTo = (DeveloperProfile)EditorGUILayout.ObjectField(
                    "Assigned To (Fallback)",
                    tempAssignedTo,
                    typeof(DeveloperProfile),
                    false 
                );
            }

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Pushes buttons to the right

            /// <summary>
            /// Button to save changes made in the editor window.
            /// </summary>
            if (GUILayout.Button("Save Changes", GUILayout.Height(35), GUILayout.Width(120)))
            {
                SaveChanges();
            }

            /// <summary>
            /// Button to close the editor window without saving changes.
            /// </summary>
            if (GUILayout.Button("Cancel", GUILayout.Height(35), GUILayout.Width(80)))
            {
                Close(); // Close window without saving changes
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Saves the temporary changes to the current KanbanCardData object.
        /// </summary>
        private void SaveChanges()
        {
            if (currentCard != null)
            {
                // Record Undo history before applying changes
                Undo.RecordObject(currentCard, "Edit Kanban Card");

                currentCard.title = tempTitle;
                currentCard.description = tempDescription;
                currentCard.currentColumn = tempColumn;
                currentCard.assignedTo = tempAssignedTo;

                // Mark ScriptableObject as dirty to ensure changes are saved
                EditorUtility.SetDirty(currentCard);
                // Save assets to disk
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(); // Refresh Project window

                // Repaint the main Kanban window to show updated changes
                EditorWindow.GetWindow<KanbanWindow>()?.Repaint();
            }
            Close(); // Close the window after saving changes
        }
    }
}