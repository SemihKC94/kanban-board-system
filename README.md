# 🚀 Unity Editor Kanban Board System

Welcome to the **Unity Editor Kanban Board System**! This is a powerful, customizable, and intuitive task management tool built directly within the Unity Editor. Designed to streamline your game development workflow, it helps teams track tasks, manage progress, and assign responsibilities without ever leaving your development environment.

This project is currently in its **early stages of development**, but it's already a highly functional tool that aims to evolve into a comprehensive solution for game development task management. Your feedback and contributions are always welcome!

---

## ✨ Features

* **Customizable Kanban Boards:** Organize your tasks into "To Do," "In Progress," and "Done" columns.
* **Interactive Cards:** Each card represents a task with a title, description, and assigned developer.
* **Effortless Progress Tracking:**
    * **"Move >>"** button: Advance tasks to the next stage.
    * **"Move <<"** button: Revert tasks to a previous stage if priorities shift.
    * **"↑" / "↓" buttons:** Reorder tasks within their current column.
* **Developer Assignment:** Assign tasks to specific team members using customizable developer profiles.
* **Dynamic Customization:** Personalize your Kanban board's look and feel directly from Unity's Project Settings:
    * **Card Colors:** Set distinct background colors for cards in each column.
    * **Text & Font Styles:** Control title, description, and assignee text colors and assign custom fonts.
    * **Status Icons:** Add unique icons to the top-right corner of cards for each progress stage (To Do, In Progress, Done).
* **Scene View Integration:** Get a real-time overview of "In Progress" tasks displayed directly as a draggable, collapsible overlay in your Scene View – perfect for context-aware development!

### 👁️ Visuals

Here's a glimpse of the Kanban System in action, including its customizable features and Scene View overlay.

[![Watch the demo video](https://github.com/SemihKC94/kanban-board-system/blob/main/Content/3.png)](https://github.com/SemihKC94/kanban-board-system/raw/refs/heads/main/Content/KanbanOnboarding.mp4)
*(Click the image above to watch the 4-minute demo video!)*

---

## 🛠️ Installation

Getting started with the Kanban Board system is straightforward:

1.  **Manual Installation (Unity Package):**
    * Download the latest `.unitypackage` from the [Releases](https://github.com/SemihKC94/kanban-board-system/releases)) section (e.g., `Kanban-Board-v0.1.unitypackage`).
    * In your Unity project, go to `Assets > Import Package > Custom Package...` and select the downloaded file.
    * Import all necessary files.
2.  **Create Kanban Settings:**
    * In Unity, go to `Assets/Create/Kanban System/Kanban Settings`. This will create a `KanbanSettings.asset` file (typically under `Assets/Kanban Board/Editor/Resources/` by default).
3.  **Configure Settings (Optional but Recommended):**
    * Customize your Kanban Board's appearance and behavior by navigating to `Edit > Project Settings > SKC > Kanban Board Settings`.
    * Here, you'll find various customization options for colors, fonts, and icons.
    * **Crucially, add your `DeveloperProfile` assets (created via `Assets/Create/SKC/Kanban/Developer Profile`) to the "Manage Developers" list within these settings.**
4.  **Open Kanban Board Window:** Go to `SKC/Kanban/Kanban Board` in the Unity Editor top menu.
5.  **Activate Scene View Overlay:** Go to `SKC/Kanban/Kanban In Scene View` in the Unity Editor top menu to enable the "In Progress" task overlay in your Scene View.

---

## 🏗️ Design Principles (OOP & SOLID)

This Kanban system is built with a focus on clean code and maintainability, adhering to Object-Oriented Programming (OOP) principles and aiming for SOLID principles where applicable:

* **Single Responsibility Principle (SRP):**
    * `KanbanCardData`: Solely responsible for holding card data.
    * `DeveloperProfile`: Solely responsible for holding developer profile data.
    * `KanbanSettings`: Solely responsible for global system settings.
    * `KanbanWindow`: Primarily responsible for drawing the main Kanban board UI and managing card interactions.
    * `KanbanCardEditorWindow`: Solely responsible for the modal card editing UI.
    * `KanbanSettingsProvider`: Solely responsible for providing the Project Settings UI for `KanbanSettings`.
    * `KanbanSceneOverlay`: Solely responsible for drawing the overlay in the Scene View.
* **Open/Closed Principle (OCP) (Aspirational):** The system is designed to be extensible. For instance, adding new card types or display options can be done by extending data models and UI drawing logic, rather than modifying core existing classes.
* **Dependency Inversion Principle (DIP) (Aspirational):** While a fully abstracted architecture might be overkill for an Editor-only tool of this scope, we strive to reduce tight coupling. For example, `KanbanWindow` depends on `KanbanSettings.instance` via a static property, providing a loosely coupled way to access settings.
* **Encapsulation:** Data and methods are encapsulated within their respective classes, exposing only necessary functionality via `public` members.
* **Modularity:** The system is broken down into distinct modules (cards, settings, UI windows, scene overlay), making it easier to understand, test, and maintain.
* **Separation of Concerns:** UI logic, data storage, and business logic are kept as separate as possible (e.g., `KanbanCardData` for data, `KanbanWindow` for UI, `KanbanSettings` for configuration).

---

## 🔜 Future Updates (Roadmap)

This project is continuously evolving! Here's a glimpse of what's planned for upcoming versions:

* **Enhanced Card Content:**
    * Adding more flexible content types to Kanban cards, such as **checkboxes, dropdowns, and customizable lists**, beyond just text descriptions. This will allow for more detailed task breakdowns directly within the card.
* **Advanced Developer Profiles:**
    * Enriching `DeveloperProfile` ScriptableObjects with additional information like **avatars and email addresses**.
    * Implementing **permission settings** for profiles (e.g., "Kanban Admin" can create and edit all tasks, "Kanban User" can only edit assigned tasks).
* **Cloud Integration (Ambitious!):**
    * Exploring **cloud synchronization** for developer profiles and Kanban content. Imagine teams seamlessly sharing boards and updates across different machines! This will be a significant challenge but a powerful feature.
* **UI/UX Overhaul:**
    * Migrating the `Kanban Board` window and `Scene View` menu UI to **Unity's UI Toolkit**. This will provide a more modern, performant, and flexible user interface, enhancing the overall user experience.
* **Additional Customization:**
    * More granular control over column visual styles.
    * Customizable card templates.
* **Task Linking:**
    * Ability to link Kanban cards directly to specific GameObjects or assets in the Unity project.

---

Your feedback is invaluable as I continue to build this out! Feel free to open issues or pull requests. Let's make game development workflow even better together!
