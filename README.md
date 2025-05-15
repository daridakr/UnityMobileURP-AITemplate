# Unity Mobile 3D URP AI-Ready Template

A foundational Unity project template designed for rapid development of mobile 3D games (Android/iOS). It leverages the Universal Render Pipeline (URP), modern development practices (DI with Zenject, New Input System, UI Toolkit), and a pre-configured structure ready for AI integration (e.g., Gemini API).

**Goal:** To accelerate the start of new mobile game projects by providing a clean, optimized, and feature-rich starting point.

**Template Version:** 0.1 (Corresponds to `feat/gemini-integration` branch state as of 2025-05-15)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.23f1%20LTS-blueviolet)

---

## Core Features (Consolidated in `feat/gemini-integration`)

This template provides a robust starting point including:

* **Unity Version:** `2022.3.23f1 LTS` (Strictly use this version for compatibility).
* **Rendering:** Universal Render Pipeline (URP) configured for mobile performance.
* **Platforms:** Pre-configured for Android (IL2CPP, .NET Standard 2.1, ARM64) and iOS.
* **Key Packages:**
    * `Universal RP`
    * `UI Toolkit` & `UI Builder`
    * `Input System`
    * `TextMeshPro`
    * `Newtonsoft Json` (`com.unity.nuget.newtonsoft-json`)
    * `Addressables` (Setup for basic scene management)
    * `Zenject (Extenject)`: Dependency Injection framework.
    * `Firebase SDK` (Auth, Crashlytics, Analytics - basic setup, requires user configuration).
* **Project Structure:** Organized `Assets/_Project` folder, `Editor` scripts, `.gitignore`.
* **Core Systems:**
    * **Dependency Injection:** Zenject with `ProjectContext` and examples of `SceneContext` (via prefabs) and installers.
    * **Scene Management:** `IAddressableSceneLoader` service for loading scenes via Addressables.
    * **Secrets Management:** `ISecretKeyProvider` for handling API keys (example with `private.secrets.json` in `StreamingAssets`).
    * **Application Startup:** `ApplicationCoordinator` and `Bootstrap` system for ordered, asynchronous initialization of services and scene loading.
    * **File Service:** `IFileService` with `PersistentStreamFileService` and `TransientStreamFileService` implementations for flexible file I/O.
    * **Q&A Logger (`#if TEST`):** `IQALogger` and `FileQALogger` for logging AI interactions to a file.
    * **Diagnostic Timers (`#if TEST`):** `OperationTimer` utility and `TimedGeminiHttpClientDecorator` for measuring API call latency.
    * **Editor Build Automation:** `ProjectBuilder` and `BuildProfileSO` for configurable Debug/Release builds via Editor Window or menu.
* **Gemini API Integration Example (`feat/gemini-integration` branch):**
    * `IGeminiService` and `GeminiService` for interacting with the Gemini API.
    * DTOs for request/response.
    * Example UI (`UIGeminiExamplePresenter`, `View`, `ViewModel`) using UI Toolkit.
* **Editor Utilities:**
    * `Build Target Checker` (reminds to switch platform).
    * `Scene Loader Window` (quick scene navigation).
    * `Build Launcher Window` (for automated builds).

---

## Branches

This repository maintains several branches representing different levels of features:

* **`main`**: Minimal base template (URP, Input System, basic structure).
* **`feat/deployment`**: Based on `main`. Adds Zenject, Firebase SDK (Auth, Analytics, Crashlytics), core services (SceneLoader, Secrets), Addressables setup, and initial Editor utilities.
* **`feat/gemini-integration`**: (RECOMMENDED STARTING POINT) Based on `feat/deployment`. Adds full Gemini API integration example, Q&A logging, diagnostic timers, and advanced build automation.

**How to use:**
Clone the branch that best suits your needs. For the most feature-complete version:
`git clone <REPOSITORY_URL> -b feat/gemini-integration`

---

## Prerequisites

* **Unity Hub**
* **Unity Editor `2022.3.23f1 LTS`** (Install via Unity Hub with Android & iOS Build Support modules)
* **Git** client
* **(For Android builds)** Android SDK & NDK (usually managed by Unity Hub)
* **(For iOS builds)** macOS with Xcode installed
* **(Optional, for Firebase CLI)** Node.js and npm

---

## Getting Started / Setup Instructions

1.  **Clone** the desired branch of this repository.
2.  **Open** the project in Unity Hub with version **`2022.3.23f1 LTS`**.
3.  **IMPORTANT! Change Package Name:** Go to `File -> Player Settings -> Player -> Other Settings -> Identification` and set a **unique Package Name** for your new project (e.g., `com.YourCompany.YourProductName`). This is crucial for store submissions and Firebase integration.
4.  **Switch Platform:** In `File -> Build Settings...`, select `Android` or `iOS` and click **`Switch Platform`**. Wait for re-import.
5.  **(Optional) TMP Essentials:** If prompted, import via `Window -> TextMeshPro -> Import TMP Essential Resources`.

**For Firebase Integration (if using `feat/deployment` or `feat/gemini-integration`):**

6.  **Create Firebase Project:** Go to the [Firebase Console](https://console.firebase.google.com/) and create your own Firebase project.
7.  **Register Apps:**
    * Add an **Android app** to your Firebase project. Follow instructions, using the **Package Name** you set in step 3. Download the `google-services.json` file.
    * Add an **iOS app** to your Firebase project. Follow instructions, using the **Bundle ID** (which should match your Package Name). Download the `GoogleService-Info.plist` file.
8.  **Place Config Files:**
    * Place the downloaded `google-services.json` into the `Assets/` folder of your Unity project.
    * Place the downloaded `GoogleService-Info.plist` into the `Assets/` folder.
    * *(These files are typically gitignored in the template, so you'll add your own).*
9.  **Resolve Firebase Dependencies (VERY IMPORTANT):**
    * After adding the `google-services.json` and/or `GoogleService-Info.plist` files, you **must** resolve the Android and iOS dependencies for Firebase.
    * In the Unity Editor, navigate to:
        * `Assets -> External Dependency Manager -> Android Resolver -> Resolve`
        * `Assets -> External Dependency Manager -> Android Resolver -> Force Resolve` (use this if simple Resolve doesn't seem to work or if you've updated SDK versions).
    * For iOS, dependency resolution is usually handled by CocoaPods during the Xcode build process, but ensure the EDM4U settings for iOS are appropriate (`Assets -> External Dependency Manager -> iOS Resolver -> Settings`).
    * **This step will download and configure the necessary native Firebase libraries for your project.** Without it, Firebase will not function correctly in your builds.
10.  **Firebase Console Setup:**
    * **Authentication:** In the Firebase Console, go to "Authentication" -> "Sign-in method" and enable the providers you want (e.g., Email/Password, Google). The template includes the Auth SDK, but UI/logic for login/registration needs to be implemented by you.
    * **Analytics & Crashlytics:** These services will start working automatically once the config files are in place and the SDK initializes.

**For Gemini API Integration (if using `feat/gemini-integration`):**

10. **Obtain Gemini API Key:** Get your API key from [Google AI Studio](https://aistudio.google.com/app/apikey).
11. **Configure API Key:**
    * In your Unity project, navigate to `Assets/StreamingAssets/`.
    * You should find a file like `private.secrets.json`.
    * Add your Gemini API key to this file in the following JSON format:
        ```json
        {
          "GeminiApiKey": "YOUR_ACTUAL_GEMINI_API_KEY"
        }
        ```
    * **Important:** The `private.secrets.json` file is (and should always remain) in your project's `.gitignore` to prevent accidental key exposure.

---

## License

This project is licensed under the MIT License - see the `LICENSE` file for details.