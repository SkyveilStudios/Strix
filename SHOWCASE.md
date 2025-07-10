<p align="center">
  <img src="https://i.imgur.com/xhWB45k.png" alt="Strix Banner" width="300"/>
</p>

# Strix Showcase
**Strix** is a powerful all-in-one **Unity Editor toolkit** developed by **Midori** with **Skyveil Studios**
> ⚠️ **In active development (WIP)** — Tool is still evolving and not yet finalized.

---

## Editor Tools
### 📊 Project Stats
Analyze project structure, file types, asset sizes, and more

![Project Stats](Docs/Images/Tools/ProjectStats.png)

### ❓ Missing Script Finder
Scan for and locate GameObjects with missing script references

![Missing Script Finder](Docs/Images/Tools/MSF.png)

### 📋 Task Board
Simple task board for organizing your development workflow

![Task Board](Docs/Images/Tools/TaskBoard.png)

### 🗒️ Notepad
Notes panel for jotting down ideas, TODOs, or quick references

![Notepad](Docs/Images/Tools/Notepad.png)

---

## 📂 Hierarchy
### 🔗 Hierarchy Lines
Draws connecting lines between objects to visualize parent to child relationships easier.
- Enable/Disable the option in Strix Hub
- Supports solid, dotted, and dashed styles
- Supports Dark/Light Mode

### Hierarchy Icons
Replaces default object icons with component icons.
- Top component takes priority
- Supports third part tools (ex: Mirror)

![Hierarchy](Docs/Images/Hierarchy/Hierarchy.png)
---

## Attributes
### 🖼️ ImagePreview
Show static images in the Inspector with optional sizing and alignment

![ImagePreview](Docs/Images/Attributes/ImagePreview.png)
```c#
[ImagePreview("Assets/Strix/Banners/StrixBanner.jpg", 400f)]
[SerializeField] private float randomValue;
```

### 🛑 Required
Displays a warning when a field is missing or null

![Required](Docs/Images/Attributes/Required.png)
```c#
[Required]
[SerializeField] private string strixVersion;
```

### 🔒 ReadOnly
Displays fields grayed out and uneditable in the inspector

![ReadOnly](Docs/Images/Attributes/ReadOnly.png)
```c#
[ReadOnly]
[SerializeField] private string readOnlyField;
```

### 💬 HelpBox
Displays a help box above a field in the inspector

![HelpBox](Docs/Images/Attributes/HelpBox.png)
```c#
[HelpBox("This is a HelpBox\nYou can type whatever you want", MessageType.Error)]
[SerializeField] private string help;
```

### 🏷️ Title
Draws a colored title with a colored line above a field

![Title](Docs/Images/Attributes/Title.png)
```c#
[Title("Colors!!")]
[SerializeField] private string noClue;
```


---

## Components

### 🔒 Transform Lock
Locks individual transform axes (position, rotation, scale) in the Editor

![Transform Lock](Docs/Images/Components/TransformLock.png)

### 📝 Scene Note
Add color based notes and markers directly into your scene for communication or reminders

![Scene Note](Docs/Images/Components/SceneNote.png)

### 🎧 Audio Source Preview
Preview and control AudioSource playback in the editor without entering play mode

![Audio Source Preview](Docs/Images/Components/AudioSourcePreview.png)

---
## 🔧 Utilities
### 🔍 Icon Browser
Browse and preview Unity’s internal editor icons for UI development

![Icon Browser](Docs/Images/Utilities/IconBrowser.png)

### 🐦 Strix Logger
Flexible logging utility with toggles, context highlights, color-coded messages, method tracing, and optional file output

![Strix Logger](Docs/Images/Utilities/StrixLogger.png)

---

[Back to README](README.md)