// KanbanWindow.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; 

namespace SKC.KanbanSystem
{
    public class KanbanWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        public static List<KanbanCardData> allCards = new List<KanbanCardData>(); 
    
        private string[] columnNames = { "To Do", "In Progress", "Done" };
        private int minColumnWidth = 250;
        private int columnPadding = 10;
        private int iconSize = 25; 
        private int iconRightOffset = 15;
        private int iconTopOffset = 5;  
        
        private static KanbanSettings settings;
        public static KanbanSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = KanbanSettings.instance;
                }
                return settings;
            }
        }

        [MenuItem("SKC/Kanban/Kanban Board")]
        public static void ShowWindow()
        {
            GetWindow<KanbanWindow>("Kanban Board");
        }
        
        private void OnEnable()
        {
            LoadAllKanbanCards();
        }
    
        private void OnFocus()
        {
            LoadAllKanbanCards();
            Repaint();
        }
    
        private void LoadAllKanbanCards()
        {
            string[] guids = AssetDatabase.FindAssets("t:KanbanCardData");
            allCards.Clear();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                KanbanCardData card = AssetDatabase.LoadAssetAtPath<KanbanCardData>(path);
                if (card != null)
                {
                    allCards.Add(card);
                }
            }
        }
    
        private void OnGUI()
        {
            if (Settings == null)
            {
                EditorGUILayout.HelpBox("Kanban Settings not found. Please create one via Assets/Create/Kanban System/Kanban Settings.", MessageType.Warning);
                if (GUILayout.Button("Create Kanban Settings"))
                {
                    KanbanSettings.CreateKanbanSettingsAsset();
                    settings = KanbanSettings.instance; 
                }
                return; 
            }

            if (GUILayout.Button("Create New Card", GUILayout.Height(30)))
            {
                CreateNewKanbanCard();
            }
    
            EditorGUILayout.Space(10);
    
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
    
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(columnPadding);

            for (int i = 0; i < columnNames.Length; i++)
            {
                KanbanColumnType currentColumnType = (KanbanColumnType)i;
    
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinWidth(minColumnWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                
                GUIStyle columnHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                columnHeaderStyle.alignment = TextAnchor.MiddleCenter;
                columnHeaderStyle.fontSize = 14;
                switch (currentColumnType)
                {
                    case KanbanColumnType.ToDo:
                        columnHeaderStyle.normal.textColor = Settings.toDoHeaderColor;
                        break;
                    case KanbanColumnType.InProgress:
                        columnHeaderStyle.normal.textColor = Settings.inProgressHeaderColor;
                        break;
                    case KanbanColumnType.Done:
                        columnHeaderStyle.normal.textColor = Settings.doneHeaderColor;
                        break;
                    default: 
                        columnHeaderStyle.normal.textColor = Color.black;
                        break;
                }
                EditorGUILayout.LabelField(columnNames[i], columnHeaderStyle, GUILayout.Height(30));
    
                EditorGUILayout.Space(5);
    
                DrawCardsInColumn(currentColumnType);

                GUILayout.FlexibleSpace();
    
                EditorGUILayout.EndVertical();

                if (i < columnNames.Length - 1)
                {
                    GUILayout.Space(columnPadding);
                }
            }
            GUILayout.Space(columnPadding);
    
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    
        private void DrawCardsInColumn(KanbanColumnType columnType)
        {
            List<KanbanCardData> cardsInColumn = allCards
                .Where(c => c.currentColumn == columnType)
                .OrderBy(c => c.orderIndex)
                .ToList();

            for (int j = 0; j < cardsInColumn.Count; j++)
            {
                KanbanCardData card = cardsInColumn[j];

                GUIStyle cardStyle = new GUIStyle(EditorStyles.helpBox);
                
                Color cardBgColor;
                switch (card.currentColumn)
                {
                    case KanbanColumnType.ToDo:
                        cardBgColor = Settings.toDoCardColor;
                        break;
                    case KanbanColumnType.InProgress:
                        cardBgColor = Settings.inProgressCardColor;
                        break;
                    case KanbanColumnType.Done:
                        cardBgColor = Settings.doneCardColor;
                        break;
                    default:
                        cardBgColor = Settings.toDoCardColor;
                        break;
                }
                
                cardStyle.normal.background = MakeTex(1, 1, cardBgColor);
                cardStyle.hover.background = MakeTex(1, 1, Settings.cardHoverColor);
                cardStyle.padding = new RectOffset(5, 5, 5, 5); 
                cardStyle.alignment = TextAnchor.UpperLeft;
                cardStyle.wordWrap = true;

                EditorGUILayout.BeginVertical(cardStyle); 

                Rect cardContentDrawingRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(iconSize * 2));
                
                if (Event.current.type == EventType.Repaint)
                {
                    Texture2D iconToDraw = null;
                    if (Settings != null)
                    {
                        switch (card.currentColumn)
                        {
                            case KanbanColumnType.ToDo:
                                iconToDraw = Settings.toDoIcon;
                                break;
                            case KanbanColumnType.InProgress:
                                iconToDraw = Settings.inProgressIcon;
                                break;
                            case KanbanColumnType.Done:
                                iconToDraw = Settings.doneIcon;
                                break;
                            default: 
                                iconToDraw = null;
                                break;
                        }
                    }

                    if (iconToDraw != null)
                    {
                        GUI.BeginClip(cardContentDrawingRect);
                        
                        Rect iconRect = new Rect(
                            cardContentDrawingRect.width - iconSize - iconRightOffset, 
                            iconTopOffset,                                          
                            iconSize,
                            iconSize
                        );
                        GUI.DrawTexture(iconRect, iconToDraw);

                        GUI.EndClip();
                    }
                }
  
                GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
                titleStyle.wordWrap = true;
                titleStyle.fontSize = Settings.cardTitleFontSize;
                titleStyle.normal.textColor = Settings.titleTextColor;
                titleStyle.font = Settings.titleFont;
                EditorGUILayout.LabelField(card.title, titleStyle);

                GUIStyle descriptionStyle = new GUIStyle(EditorStyles.miniLabel);
                descriptionStyle.wordWrap = true;
                descriptionStyle.fontSize = Settings.cardDescriptionFontSize;
                descriptionStyle.normal.textColor = Settings.descriptionTextColor;
                descriptionStyle.font = Settings.descriptionFont;
                EditorGUILayout.LabelField(card.description, descriptionStyle);
  
                GUIStyle assignedToStyle = new GUIStyle(EditorStyles.miniLabel);
                assignedToStyle.normal.textColor = Settings.assignedToColor; 
                assignedToStyle.fontStyle = FontStyle.Bold;
                
                string assignedToText = (card.assignedTo != null) ? card.assignedTo.developerName : "Unassigned"; 
                EditorGUILayout.LabelField("Assigned To: " + assignedToText, assignedToStyle);


                EditorGUILayout.Space(2);
    
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Edit", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
                {
                    KanbanCardEditorWindow.Open(card);
                }

                if (card.currentColumn != KanbanColumnType.ToDo) 
                {
                    if (GUILayout.Button("Move <<", EditorStyles.miniButtonMid, GUILayout.Width(60))) 
                    {
                        MoveKanbanCard(card, false); 
                        GUIUtility.ExitGUI(); 
                    }
                }

                if (card.currentColumn != KanbanColumnType.Done) 
                {
                    if (GUILayout.Button("Move >>", EditorStyles.miniButtonMid, GUILayout.Width(60))) 
                    {
                        MoveKanbanCard(card, true); 
                        GUIUtility.ExitGUI(); 
                    }
                }

                if (j > 0)
                {
                    if (GUILayout.Button("↑", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                    {
                        MoveCardOrder(cardsInColumn, j, -1);
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button("↑", EditorStyles.miniButtonMid, GUILayout.Width(25));
                    EditorGUI.EndDisabledGroup();
                }

                if (j < cardsInColumn.Count - 1)
                {
                    if (GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                    {
                        MoveCardOrder(cardsInColumn, j, 1);
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(25));
                    EditorGUI.EndDisabledGroup();
                }


                if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete Card", "Are you sure you want to delete this card: " + card.title + "?", "Yes", "No"))
                    {
                        DeleteKanbanCard(card);
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();
    
                EditorGUILayout.EndVertical(); 
                EditorGUILayout.Space(5); 
            }
        }
        
        private void MoveKanbanCard(KanbanCardData card, bool isForward)
        {
            KanbanColumnType oldColumn = card.currentColumn;

            if (isForward)
            {
                if (card.currentColumn == KanbanColumnType.ToDo)
                    card.currentColumn = KanbanColumnType.InProgress;
                else if (card.currentColumn == KanbanColumnType.InProgress)
                    card.currentColumn = KanbanColumnType.Done;
            }
            else 
            {
                if (card.currentColumn == KanbanColumnType.Done)
                    card.currentColumn = KanbanColumnType.InProgress;
                else if (card.currentColumn == KanbanColumnType.InProgress)
                    card.currentColumn = KanbanColumnType.ToDo;
            }

            if (oldColumn != card.currentColumn)
            {
                int maxOrderInNewColumn = 0;
                if (allCards.Any(c => c.currentColumn == card.currentColumn))
                {
                    maxOrderInNewColumn = allCards
                        .Where(c => c.currentColumn == card.currentColumn)
                        .Max(cardData => (int?)cardData.orderIndex) ?? 0;
                }
                card.orderIndex = maxOrderInNewColumn + 1;
            }

            EditorUtility.SetDirty(card);
            AssetDatabase.SaveAssets();
            LoadAllKanbanCards();
        }


        private void MoveCardOrder(List<KanbanCardData> cardsInCurrentColumn, int currentIndex, int direction)
        {
            int targetIndex = currentIndex + direction;

            if (targetIndex >= 0 && targetIndex < cardsInCurrentColumn.Count)
            {
                KanbanCardData cardToMove = cardsInCurrentColumn[currentIndex];
                KanbanCardData targetCard = cardsInCurrentColumn[targetIndex];

                int tempOrder = cardToMove.orderIndex;
                cardToMove.orderIndex = targetCard.orderIndex;
                targetCard.orderIndex = tempOrder;

                EditorUtility.SetDirty(cardToMove);
                EditorUtility.SetDirty(targetCard);
                AssetDatabase.SaveAssets();

                LoadAllKanbanCards();
                Repaint();
            }
        }
    
        private void CreateNewKanbanCard()
        {
            KanbanCardData newCard = ScriptableObject.CreateInstance<KanbanCardData>();
    
            string kanbanSystemPathName = "Kanban Board";
            string cardContainerName = "Cards";
            string directory = "Assets/" + kanbanSystemPathName +  "/" + cardContainerName;
            string path = AssetDatabase.GenerateUniqueAssetPath(directory + "/New KanbanCard.asset");
            if (!AssetDatabase.IsValidFolder(directory))
            {
                AssetDatabase.CreateFolder("Assets/" + kanbanSystemPathName, cardContainerName);
            }

            int maxOrderInToDo = 0;
            if (allCards.Any(c => c.currentColumn == KanbanColumnType.ToDo))
            {
                maxOrderInToDo = allCards
                    .Where(c => c.currentColumn == KanbanColumnType.ToDo)
                    .Max(card => (int?)card.orderIndex) ?? 0;
            }
            newCard.orderIndex = maxOrderInToDo + 1;
    
            AssetDatabase.CreateAsset(newCard, path);
            EditorUtility.SetDirty(newCard);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = newCard;
            LoadAllKanbanCards();
        }
    
        private void DeleteKanbanCard(KanbanCardData cardToDelete)
        {
            string path = AssetDatabase.GetAssetPath(cardToDelete);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LoadAllKanbanCards();
        }
    
        private Texture2D MakeTex(int width, int height, Color col)
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