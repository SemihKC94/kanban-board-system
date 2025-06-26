using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace SKC.KanbanSystem
{
    /// <summary>
    /// Handles drawing of the Kanban "In Progress" menu overlay in the Scene View.
    /// It initializes on load and subscribes to SceneView drawing events.
    /// </summary>
    [InitializeOnLoad]
    public static class KanbanSceneOverlay
    {
        private static bool showKanbanMenu = true; 
        private static Vector2 scrollPosition; 
        /// <summary>
        /// The current Rect of the Kanban menu window in the Scene View.
        /// </summary>
        private static Rect windowRect = new Rect(10, 10, 0, 0); 

        /// <summary>
        /// The maximum height the menu window can expand to.
        /// </summary>
        private const float MAX_MENU_HEIGHT = 400f; 
        private const float EXPANDED_MENU_HEIGHT = 200f;

        /// <summary>
        /// The height of the menu when it is collapsed.
        /// </summary>
        private const float MIN_COLLAPSED_MENU_HEIGHT = 50f; 
        /// <summary>
        /// The maximum height the scroll view for cards can take within the menu.
        /// </summary>
        private const float MAX_SCROLLVIEW_HEIGHT = 300f;

        /// <summary>
        /// Controls whether the Kanban Scene View menu is currently active and visible.
        /// </summary>
        private static bool isOn = false;
        
        /// <summary>
        /// Shows the Kanban menu in the Scene View via a Unity menu item.
        /// </summary>
        [MenuItem("SKC/Kanban/Kanban In Scene View")]
        public static void ShowWindow()
        {
            isOn = true;
        }

        /// <summary>
        /// Static constructor called when the Unity editor loads or scripts are recompiled.
        /// Subscribes to SceneView drawing events.
        /// </summary>
        static KanbanSceneOverlay()
        {
            Debug.Log("[KanbanSceneOverlay] Initialized. Subscribing to SceneView.duringSceneGui.");
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Callback method invoked during Scene View drawing events.
        /// Draws the Kanban menu if the editor is not playing and it's a Repaint event.
        /// </summary>
        /// <param name="sceneView">The current SceneView instance.</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && isOn)
            {
                Handles.BeginGUI();
                DrawKanbanMenuInScene();
                Handles.EndGUI();
            }
        }

        /// <summary>
        /// Draws the main Kanban menu window as an overlay in the Scene View.
        /// </summary>
        private static void DrawKanbanMenuInScene()
        {
            // Set windowRect height based on foldout state
            if (!showKanbanMenu)
            {
                windowRect = new Rect(10, 10, 200, MIN_COLLAPSED_MENU_HEIGHT); 
            }
            else
            {
                windowRect = new Rect(10, 10, 200, EXPANDED_MENU_HEIGHT); 
            }
            
            // GUILayout.Window handles positioning and resizing.
            // MaxHeight option ensures the window doesn't exceed a certain height.
            windowRect = GUILayout.Window(123456, windowRect, DrawKanbanMenuContent, "Kanbans In Progress", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), 
                GUILayout.MinHeight(MIN_COLLAPSED_MENU_HEIGHT), GUILayout.MaxHeight(EXPANDED_MENU_HEIGHT));
        }

        /// <summary>
        /// Draws the content within the Kanban Scene View menu window.
        /// This method is called by GUILayout.Window.
        /// </summary>
        /// <param name="windowID">The ID of the window.</param>
        private static void DrawKanbanMenuContent(int windowID)
        {
            GUIStyle contentStyle = new GUIStyle(EditorStyles.helpBox);
            contentStyle.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.15f, 0.6f)); // Darker gray, slightly transparent
            contentStyle.padding = new RectOffset(5, 5, 5, 5);

            // General vertical group for window content
            EditorGUILayout.BeginVertical(); 
            
            // Foldout header for "In Progress Cards"
            string content = showKanbanMenu ? "Collapse" : "Expand";
            showKanbanMenu = EditorGUILayout.Foldout(showKanbanMenu, content, true, EditorStyles.foldoutHeader);

            if (showKanbanMenu)
            {
                // Content area for the cards, wrapped in a vertical group with a style.
                // This group must never be empty in the layout pass.
                EditorGUILayout.BeginVertical(contentStyle); 
                
                // Always draw a small space to ensure the layout group is never empty, preventing ArgumentException.
                GUILayout.Space(1f); 

                List<KanbanCardData> inProgressCards = null;
                
                if (KanbanWindow.allCards == null)
                {
                    EditorGUILayout.LabelField("Kanban cards not loaded.", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField("Please open Kanban Board window.", EditorStyles.miniLabel); 
                }
                else
                {
                    inProgressCards = KanbanWindow.allCards
                        .Where(card => card.currentColumn == KanbanColumnType.InProgress)
                        .OrderBy(card => card.orderIndex)
                        .ToList();

                    if (inProgressCards.Count == 0)
                    {
                        EditorGUILayout.LabelField("No 'In Progress' cards.", EditorStyles.miniLabel);
                    }
                    else
                    {
                        // ScrollView for the card list, with a max height
                        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(MAX_SCROLLVIEW_HEIGHT)); 

                        foreach (KanbanCardData card in inProgressCards)
                        {
                            GUIStyle cardLabelStyle = new GUIStyle(EditorStyles.label);
                            cardLabelStyle.normal.textColor = KanbanSettings.instance != null ? KanbanSettings.instance.inProgressCardColor : Color.yellow;
                            cardLabelStyle.wordWrap = true;
                            cardLabelStyle.fontStyle = FontStyle.Bold;

                            GUIStyle assignedToLabelStyle = new GUIStyle(EditorStyles.miniLabel);
                            assignedToLabelStyle.normal.textColor = Color.cyan; 

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox); 
                            EditorGUILayout.LabelField(card.title, cardLabelStyle);
                            EditorGUILayout.LabelField($"Assigned: {(card.assignedTo != null ? card.assignedTo.developerName : "Unassigned")}", assignedToLabelStyle);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space(2);
                        }
                        EditorGUILayout.EndScrollView();
                    }
                }
                EditorGUILayout.EndVertical(); // End contentStyle vertical group
            }
            
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Close Menu"))
            {
                isOn = false; // Artık dışarıdan kontrol ediliyor
                SceneView.RepaintAll();
            }
            
            // Allow dragging the window by clicking anywhere within its content.
            GUI.DragWindow(); 
            // End the general vertical group for the window content.
            EditorGUILayout.EndVertical(); 
        }

        /// <summary>
        /// Helper function to create a 1x1 Texture2D of a given color for GUI backgrounds.
        /// </summary>
        /// <param name="width">Width of the texture.</param>
        /// <param name="height">Height of the texture.</param>
        /// <param name="col">Color of the texture.</param>
        /// <returns>A new Texture2D with the specified color.</returns>
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}