# Unity Mobile 3D URP AI-Ready Template (Core Base)

A minimal foundational Unity project template designed for rapid development of mobile 3D games (Android/iOS). It leverages the Universal Render Pipeline (URP), the New Input System, and UI Toolkit.

**Goal:** To provide an absolutely clean and essential starting point for new mobile game projects, allowing developers to build their architecture from the ground up or integrate their preferred solutions.

**Template Version:** 0.0.1 (Corresponds to `main` branch state as of 2025-05-15)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.23f1%20LTS-blueviolet)

---

## Core Features (Branch: `main`)

This branch provides the bare essentials for starting a new mobile URP project:

* **Unity Version:** `2022.3.23f1 LTS` (Strictly use this version for compatibility).
* **Rendering:** Universal Render Pipeline (URP) configured with default settings suitable for mobile platforms.
* **Platforms:** Project pre-configured for Android (IL2CPP, .NET Standard 2.1, ARM64) and iOS.
* **Key Packages Included:**
    * `Universal RP`
    * `UI Toolkit` (& `UI Builder`)
    * `Input System`
    * `TextMeshPro`
    * `Zenject (Extenject)`: Dependency Injection framework (core setup).
    * `(Optional) Newtonsoft Json` (`com.unity.nuget.newtonsoft-json`) - *Included for basic JSON needs, can be removed if not used.*
    * `(Optional) Visual Studio Code Editor` - *Configured, can be switched.*
* **Project Structure:** Basic organized structure within `Assets/_Project` for game-specific assets and scripts. `Editor` folder at the root `Assets` level.
* **Version Control:** Includes a `.gitignore` file configured for Unity.
* **Dependency Injection:** Basic Zenject setup with `ProjectContext` available. Installers and scene contexts should be added by the user as needed.
* **Editor Utilities Included (Examples):**
    * `Build Target Checker`: Reminds you to switch to the correct mobile platform (Android/iOS) if the wrong one is active upon opening the project. Found in `Assets/Editor/Workflow/`.
    * `Scene Loader Window`: Provides a convenient editor window to quickly load scenes. Found in `Assets/Editor/Workflow/`.
        * **How to use Scene Loader:**
            1.  Open via Unity menu: `Window -> -> Scene Loader`.
            2.  Click "**Add Scene Folder**", navigate to and select your scenes folder (e.g., `Assets/_Project/Scenes`).
            3.  Click scene buttons to load them.

*(Note: This branch provides basic Zenject setup. More advanced services, Firebase SDK, Addressables setup, and AI integration examples are found in branches like `feat/deployment` or `feat/gemini-integration`.)*

---

## Branches

This repository uses branches to provide additional functional modules on top of this base template:

* **`main`**: (THIS BRANCH) Contains only the **minimal base template** described above.
* **`feat/deployment`**: Based on `main`. Adds Firebase SDK (Auth, Analytics, Crashlytics), core services examples, Addressables setup, and more Editor utilities.
* **`feat/gemini-integration`**: Based on `feat/deployment`. Adds a full Gemini API integration example, Q&A logging, diagnostic timers, and advanced build automation.

**How to use:**
Clone the branch that best suits your needs. For this minimal base:
`git clone <REPOSITORY_URL>` (clones `main` by default)

---

## Prerequisites

* **Unity Hub**
* **Unity Editor `2022.3.23f1 LTS`** (Install via Unity Hub with Android & iOS Build Support modules)
* **Git** client
* **(For Android builds)** Android SDK & NDK
* **(For iOS builds)** macOS with Xcode installed

---

## Getting Started / Setup Instructions

1.  **Clone** this `main` branch of the repository.
2.  **Open** the project in Unity Hub with version **`2022.3.23f1 LTS`**.
3.  **IMPORTANT! Change Package Name:** Go to `File -> Player Settings -> Player -> Other Settings -> Identification` and set a **unique Package Name** for your new project (e.g., `com.YourCompany.YourProductName`).
4.  **Switch Platform:** In `File -> Build Settings...`, select `Android` or `iOS` and click the **`Switch Platform`** button. Wait for re-import.
5.  **(Optional) TMP Essentials:** If prompted, import via `Window -> TextMeshPro -> Import TMP Essential Resources`.
6.  **Zenject:** Zenject should be pre-imported. You can start creating your installers and binding dependencies.

---

## License

This project is licensed under the MIT License - see the `LICENSE` file for details.