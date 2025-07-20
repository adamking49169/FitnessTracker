# FitnessTracker

This solution now contains a .NET MAUI project **FitnessTracker.Mobile** in addition to the existing ASP.NET Core application.

## Building the MAUI project for Android

1. Install the .NET MAUI and Android workloads if they are not already installed:
   ```bash
   dotnet workload install maui-android
   ```
2. Open `FitnessTracker.sln` in Visual Studio 2022 (17.8 or later).
3. Ensure `FitnessTracker.Mobile` is the startup project.
4. Start the ASP.NET Core API (`dotnet run` from the project root) so the mobile app can reach `http://10.0.2.2:5053`.
5. Choose an Android emulator or connected device from the device dropdown.
6. Build and run the solution (`F5`) to deploy the MAUI app on Android. The app uses the emulator loopback address in `MauiProgram.cs` to reach the backend.

## Modifying the Android project in Android Studio

Platform specific files for Android are located under `FitnessTracker.Mobile/Platforms/Android`.
To tweak these using Android Studio:

1. In Visual Studio, right-click the project and choose **Open Containing Folder**.
2. Open the `Platforms/Android` directory in Android Studio.
3. Make any platform-specific changes (e.g., manifest or resources) and save.

Android Studio only edits the platform folder; the rest of the MAUI project continues to be managed in Visual Studio.
