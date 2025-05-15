# Unity Mobile 3D URP AI-Ready Template (Deployment Base)

A foundational Unity project template designed for rapid development of mobile 3D games (Android/iOS). It leverages the Universal Render Pipeline (URP), modern development practices (DI with Zenject, New Input System, UI Toolkit), and includes initial setup for Firebase services.

**Goal:** To accelerate the start of new mobile game projects by providing a clean, optimized starting point with essential deployment-related SDKs pre-configured.

**Template Version:** 0.1 (Corresponds to `feat/deployment` branch state as of 2025-05-15)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Unity Version](https://img.shields.io/badge/Unity-2022.3.23f1%20LTS-blueviolet)

---

## Core Features (Branch: `feat/deployment`)

This branch builds upon `main` and includes:

* **Unity Version:** `2022.3.23f1 LTS` (Strictly use this version).
* **Rendering:** Universal Render Pipeline (URP) configured for mobile.
* **Platforms:** Pre-configured for Android (IL2CPP, .NET Standard 2.1, ARM64) and iOS.
* **Key Packages:**
    * `Universal RP`
    * `UI Toolkit` & `UI Builder`
    * `Input System`
    * `TextMeshPro`
    * `Newtonsoft Json` (`com.unity.nuget.newtonsoft-json`)
    * `Zenject (Extenject)`: Dependency Injection framework.
    * `Firebase SDK`: Core, Authentication, Crashlytics, Analytics.
* **Project Structure:** Organized `Assets/_Project` folder, `Editor` scripts, `.gitignore`.
* **Core Systems (Examples/Base):**
    * **Dependency Injection:** Zenject with `ProjectContext` and examples of installers.
* **Editor Utilities:**
    * `Build Target Checker`.
    * `Scene Loader Window`.

---

## Branches & Optional Modules

This repository uses branches to provide additional functional modules:

* **`main`**: Minimal base template (URP, Input System, basic structure).
* **`feat/deployment`**: (THIS BRANCH) Based on `main`. Adds Zenject, Firebase SDK (Auth, Analytics, Crashlytics), core services examples (SceneLoader), and initial Editor utilities.
* **`feat/gemini-integration`**: Based on `feat/deployment`. Adds a full Gemini API integration example, Q&A logging, diagnostic timers, and advanced build automation.

**How to use:**
Clone the branch that best suits your needs.
`git clone <REPOSITORY_URL> -b feat/deployment`

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

1.  **Clone** this `feat/deployment` branch of the repository.
2.  **Open** the project in Unity Hub with version **`2022.3.23f1 LTS`**.
3.  **IMPORTANT! Change Package Name:** Go to `File -> Player Settings -> Player -> Other Settings -> Identification` and set a **unique Package Name** for your new project (e.g., `com.YourCompany.YourProductName`). This is crucial for store submissions and Firebase integration.
4.  **Switch Platform:** In `File -> Build Settings...`, select `Android` or `iOS` and click the **`Switch Platform`** button. Wait for Unity to finish re-importing assets.
5.  **(Optional) TMP Essentials:** If prompted, import via `Window -> TextMeshPro -> Import TMP Essential Resources`.

**For Firebase Integration:**

6.  **Create Firebase Project:** Go to the [Firebase Console](https://console.firebase.google.com/) and create your own Firebase project.
7.  **Register Apps:**
    * Add an **Android app** to your Firebase project. Follow instructions, using the **Package Name** you set in step 3. Download the `google-services.json` file.
    * Add an **iOS app** to your Firebase project. Follow instructions, using the **Bundle ID** (which should match your Package Name). Download the `GoogleService-Info.plist` file.
8.  **Place Config Files:**
    * Place the downloaded `google-services.json` into the `Assets/` folder of your Unity project.
    * Place the downloaded `GoogleService-Info.plist` into the `Assets/` folder.
    * *(These specific filenames are typically gitignored in this template; you will be adding your own. Ensure your project's `.gitignore` does not accidentally commit your specific config files if you fork this template publicly.)*
9.  **Resolve Firebase Dependencies (VERY IMPORTANT):**
    * After adding the `google-services.json` and/or `GoogleService-Info.plist` files, you **must** resolve the Android and iOS dependencies for Firebase.
    * In the Unity Editor, navigate to:
        * `Assets -> External Dependency Manager -> Android Resolver -> Resolve`
        * `Assets -> External Dependency Manager -> Android Resolver -> Force Resolve` (use this if simple Resolve doesn't seem to work or if you've updated SDK versions).
    * For iOS, dependency resolution is usually handled by CocoaPods during the Xcode build process, but ensure the EDM4U settings for iOS are appropriate (`Assets -> External Dependency Manager -> iOS Resolver -> Settings`).
    * **This step will download and configure the necessary native Firebase libraries for your project.** Without it, Firebase will not function correctly in your builds.
10. **Firebase Console Setup:**
    * **Authentication:** In the Firebase Console, go to "Authentication" -> "Sign-in method" and enable the providers you want (e.g., Email/Password, Google). The template includes the Auth SDK, but UI/logic for login/registration needs to be implemented by you.
    * **Analytics & Crashlytics:** These services will generally start working automatically once the config files are in place and dependencies are resolved.

**(Note: Gemini API specific setup is NOT included in this `feat/deployment` branch. For Gemini integration, please refer to the `feat/gemini-integration` branch.)**

---

## License

This project is licensed under the MIT License - see the `LICENSE` file for details.