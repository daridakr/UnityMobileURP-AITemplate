# Unity Mobile 3D URP AI-Ready Template

A base Unity project template designed for rapid development of mobile 3D games (Android/iOS) using the Universal Render Pipeline (URP), modern practices, and a prepared structure for AI integration (e.g., Gemini API).

**Goal:** To accelerate the start of new projects by providing a pre-configured, clean, and optimized foundation.

**Assembly date:** 10.04.2025.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.23f1%20LTS-blueviolet)

---

## Base Template Features (`main` branch)

* **Unity Version:** `2022.3.23f1 LTS` (Use **this exact** version!)
* **Rendering:** Configured **Universal Render Pipeline (URP)** with default settings optimized for mobile platforms (HDR, MSAA, SSAO, expensive shadows/lights disabled by default).
* **Platforms:** Project pre-configured for **Android** and **iOS** builds (IL2CPP, .NET Standard 2.1, ARM64).
* **Core Packages Included:**
    * `Universal RP`: For rendering.
    * `UI Toolkit` (& `UI Builder`): The recommended system for Runtime UI (`Unity UI (uGUI)` package remains only as a TextMeshPro dependency).
    * `Input System`: Modern input handling for flexible control schemes.
    * `TextMeshPro`: For high-quality text rendering.
    * `Newtonsoft Json` (`com.unity.nuget.newtonsoft-json`): For robust JSON parsing (e.g., from APIs).
    * `Visual Studio Code Editor`: Integration configured for VS Code.
    * `Zenject (Extenject)`: Dependency Injection framework configured.
* **Folder Structure:** Organized structure within `Assets/_Project` for your game-specific assets and scripts (feature/system-based separation). `Editor` and `Tests` folders are at the root `Assets` level.
* **Version Control:** Includes a `.gitignore` file configured for Unity (ignores `Library`, `Temp`, etc., but **includes** `ProjectSettings`).
* **Editor Utilities Included:**
    * `Build Target Checker`: Reminds you to switch to the correct mobile platform (Android/iOS) if the wrong one is active upon opening the project.
    * `Scene Loader Window`: Provides a convenient editor window to quickly load scenes from your project without navigating the Project folders.
        * **How to use:**
            1.  Open the window via the Unity menu: `Window -> Scene Loader`.
            2.  Click the "**Add Scene Folder**" button inside the window.
            3.  In the pop-up panel, navigate to and select your main scenes folder (typically **`Assets/_Project/Scenes`**).
            4.  The window will now list all `.unity` scenes found in that folder and its subfolders.
            5.  Click the button with the scene name you want to open (it will prompt you to save current scene changes if necessary).
    * The scripts are located in `Assets/Editor/Workflow/`.

* **License:** MIT License (see `LICENSE` file).

---

## Branches & Optional Modules

This repository uses branches to provide additional functional modules on top of the base template:

* **`main`**:
    * Contains only the **base template** described above. Ideal for starting any new mobile URP project.
* **`feat/deployment`**:
    * Based on `main`.
    * Adds basic setup and/or scripts for **project deployment/building** (e.g., configuration for **WebGL** builds, potentially simple build scripts or starter CI/CD configurations).
    * *Note: The specific deployment implementation may be added or changed.*
* **`feat/gemini-integration`**:
    * Based on `feat/deployment`.
    * Adds a **minimal example of Gemini API integration**:
        * A simple script (e.g., `Assets/_Project/Scripts/AI/GeminiExample.cs`) demonstrating how to send a request to the Gemini API (e.g., using `UnityWebRequest` or `HttpClient`).
        * An example of processing the response (parsing JSON with Newtonsoft Json).
        * A basic scene with UI (using UI Toolkit) to input a prompt and display the Gemini response.
        * An **API key placeholder/config** (e.g., a ScriptableObject in `Assets/_Project/Configs`) that **must be filled in manually** and is **excluded from Git** via `.gitignore` to prevent key leaks.

**How to use the desired version:**

* For the base template: `git clone <REPOSITORY_URL>` (clones `main` by default)
* For the template with deployment features: `git clone <REPOSITORY_URL> -b feat/deployment`
* For the template with deployment and Gemini: `git clone <REPOSITORY_URL> -b feat/gemini-integration`

---

## Prerequisites

* **Unity Hub**
* **Unity Editor `2022.3.23f1 LTS`** (Install via Unity Hub)
* **Unity Modules:** When installing the editor via Hub, ensure you select:
    * `Android Build Support` (including OpenJDK and Android SDK & NDK Tools)
    * `iOS Build Support`
    * `WebGL Build Support` (if planning to use the `feat/deployment` branch)
* **Git** client
* **VS Code** (or your preferred IDE, but integration is pre-configured for VS Code)
* **.NET SDK** (v6.0 or newer, usually installed with VS / VS Code C# tools)

---

## Getting Started / Setup Instructions

1.  **Clone** the repository with your desired branch (see "Branches & Optional Modules" section).
2.  **Open** the project in Unity Hub, ensuring the **`2022.3.23f1 LTS`** version is selected.
3.  **(VERY IMPORTANT!) Switch Platform:** Immediately after opening, go to `File -> Build Settings...`. Select `Android` or `iOS` from the list on the left and click the **`Switch Platform`** button. Wait for Unity to finish re-importing assets (this can take a few minutes).
4.  **(Optional) TextMeshPro Essentials:** If prompted, import the TMP Essential Resources via `Window -> TextMeshPro -> Import TMP Essential Resources`.
5.  **(Optional) IDE Project Files:** If VS Code or your IDE has trouble with auto-completion, try regenerating project files via `Edit -> Preferences -> External Tools -> Regenerate project files`.
6.  **(For `feat/gemini-integration` branch):** Locate the API key configuration asset (e.g., within `Assets/_Project/Configs`) and **enter your own Gemini API key** into the appropriate field. Remember this file should be ignored by Git.

---

## Core Architecture: Dependency Injection (Zenject)

This template utilizes **Zenject (Extenject fork)** for Dependency Injection (DI) to promote modular and testable code.

* **Setup:** Zenject is included via `.unitypackage` import (see Prerequisites/Setup if missing).
    * A `ProjectContext` prefab (located in `Assets/Resources/`) handles global, project-wide bindings.
    * `SceneContext` components should be added to specific scenes for scene-level bindings (As it's set up in the Main scene `System/MainSceneContext`). Instead of adding `SceneContext` directly to GameObjects within each scene, this template promotes creating **prefabs** for each distinct scene context configuration (`Assets/_Project/Prefabs/Systems/Contexts/...`). Attach necessary `MonoInstaller`(s) directly to these context prefabs. To use a context in a scene, simply **add an instance of the corresponding Scene Context prefab** to that scene's hierarchy.
    * **_Why Prefabs?_** This approach centralizes dependency configuration within prefabs (`.prefab` files). Editing dependencies involves modifying the prefab, which minimizes direct changes to scene files (`.unity`). This significantly reduces the frequency and complexity of merge conflicts in version control systems like Git, making teamwork smoother.
* **Bindings:** Define your dependencies in Installer scripts (e.g., `MonoInstaller`, `ScriptableObjectInstaller`) located in `Assets/_Project/Scripts/DI/Installers/`. Attach these installers to the relevant `ProjectContext` or `SceneContext`.
* **Injection:** Use the `[Inject]` attribute in your classes (constructors, fields, properties, methods) to receive dependencies managed by Zenject.
* **Documentation:** For detailed usage, please refer to the official **Extenject (Zenject) documentation**.

---

## Folder Structure Overview

This template uses a structured approach within the `Assets` folder to keep things organized. Your primary workspace is the `_Project` folder.

* **`_Project`**: Contains all your game-specific assets and code.
    * **`Animations`**: Animation files and Animator Controllers.
    * **`Audio`**: Sound effects (`SFX`) and music (`Music`).
    * **`Configs`**: ScriptableObjects and other configuration files.
        * Subfolders like `AI`, `Audio`, `GameSettings` (for global tunable values like difficulty, volumes), `Input`, `Player`, `UI` hold specific configuration assets.
    * **`Fonts`**: Font assets.
    * **`Graphics`**: Visual assets.
        * `Materials`: Material assets.
        * `Models`: 3D models (sub-categorized, e.g., `Chibi`, `Environment`, `Props`).
        * `Shaders`: Custom shader files/graphs.
        * `Textures`: Texture files (sub-categorized, e.g., `Environment`, `FX`, `Props`).
        * `UI`: UI assets used directly for UI Toolkit (sub-categorized, e.g., `Backgrounds`, `Icons`).
        * `VFX`: Visual Effect assets (Particle Systems, etc.).
    * **`Physics`**: Physic Material assets.
    * **`Prefabs`**: Pre-configured GameObjects.
        * Sub-categorized by type (e.g., `Cameras`, `Environments`, `Gameplay`, `Props`, `Systems`, `UI`).
    * **`Scenes`**: Unity scenes.
        * `_Main`: Main gameplay scenes.
        * `_Sandbox`: Scenes for testing and prototyping.
    * **`Scripts`**: All runtime C# scripts.
        * `AI`: AI-specific logic, Gemini integration.
        * `Audio`: Audio playback logic.
        * `Core`: Foundational systems (e.g., `Bootstrap`, `StateMachine`).
        * `Data`: Data handling logic.
        * `DI`: Dependency Injection (`Installers`).
        * `Gameplay`: Logic related to core gameplay loops.
            * `Camera`, `Player`.
        * `Networking`: Network communication logic.
        * `SaveSystem`: Saving and loading logic, including Data Transfer Objects (`DTO`).
        * `Services`: High-level manager classes or service locators.
        * `Test`: Some temporary scripts for test something or like temp solution.
        * `UI`: Logic for UI elements and windows (presenters, controllers, views).
        * `Utilities`: Common helper classes, extension methods (`Attributes`, `Constants`).

* **`Editor`**: Scripts that run only within the Unity Editor (e.g., `CodeGeneration`, `DataImporters`, `Workflow`).
* **`Plugins`**: Third-party compiled libraries (DLLs).
* **`Rendering`**: Contains URP pipeline assets (URP Asset, Renderer Assets).
* **`Resources`**: *Use minimally or avoid*. Assets here are always included in builds. Prefer direct references or Addressables.
* **`StreamingAssets`**: Files copied directly to the build (e.g., configuration files needed at runtime).
* **`Tests`**: Recommended location for Unity Test Runner scripts.

---

## License

This project is licensed under the MIT License - see the `LICENSE` file for details.
