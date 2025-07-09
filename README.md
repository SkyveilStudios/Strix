<p align="center">
  <img src="https://i.imgur.com/xhWB45k.png" alt="Strix Banner" width="300"/>
</p>

# Strix
**Strix** is a powerful all-in-one **Unity Editor toolkit** developed by **Midori** with **Skyveil Studios**, designed to enhance your workflow with:

- ✨ Custom attributes
- 🧩 Reusable components
- 🛠️ Editor windows and utilities
- 📊 Project diagnostics and tools
- 🚀 Productivity enhancements

> ⚠️ **In active development (WIP)** — Tool is still evolving and not yet finalized.

---

## ✅ Current Features
### 🧭 Project Tools
- **Update Checker** - Checks for the latest Strix Version and downloads updates from Github
- **Project Stats** – Analyze project structure, file types, asset sizes, and more
- **Missing Script Finder** – Scan for and locate GameObjects with missing script references
- **Icon Browser** – Browse and preview Unity’s internal editor icons for UI development
- **Task Board** – Simple task board for organizing your development workflow
- **Notepad** – Notes panel for jotting down ideas, TODOs, or quick references

### 🧬 Attributes
Enhance your inspector experience with these custom attributes:
- `[ImagePreview]` – Show static images in the Inspector with optional sizing and alignment
- `[Required]` - Displays a warning when a field is missing or null
- `[ReadOnly]` - Displays fields grayed out and uneditable in the inspector
- `[HelpBox]` - Displays a help box above a field in the inspector
- `[Title]` - Draws a colored title with a colored line above a field
- More coming soon...

### 🧱 Components
Reusable and editor-friendly components to aid development:
- **Transform Lock** – Locks individual transform axes (position, rotation, scale) in the Editor
- **Scene Note** – Add color based notes and markers directly into your scene for communication or reminders
- **AudioSource Preview** – Preview and control AudioSource playback in the editor without entering play mode
- More coming soon...

### 🧰 Utilities
- **Strix Logger** – Flexible logging utility with toggles, context highlights, color-coded messages, method tracing, and optional file output
- **Reflection Probe Resizer** - Automatically resizes the selected probe to fit the surrounding space using raycast
- More coming soon...

---

## 📸 Want to See It in Action?
Check out the [📖 Showcase](SHOWCASE.md) for screenshots and code examples of each tool, attribute, and component.

---

## 🛠️ Installation
You have two options to install the tool:
### 📁 Option 1: Manual Installation
1. Download or clone the repository.
2. Copy the tool's files into your Unity project under: `Assets/Strix/`

### 📦 Option 2: Unity Package
1. Go to the [Releases](https://github.com/SkyveilStudios/Strix/releases) section.
2. Download the `.unitypackage` file for the latest version.
3. In Unity, open `Assets > Import Package > Custom Package` and select the downloaded file.
4. It will automatically place the tool under: `Assets/Strix/`

---

## 🧩 Compatibility
- ✅ Unity 6.0 LTS or newer recommended
- ❓ Untested on previous versions, but should work
- 🎨 Supports URP, HDRP, and Built-in Render Pipelines
- 🧠 Editor-only tools — no impact on builds or runtime

---

## ❗ License Summary
- ✅ Free to use in personal and commercial Unity projects
- ❌ You may not sell, redistribute, or repackage this tool or its source code

See [`LICENSE.txt`](LICENSE.txt) for full terms.

---

## 📌 Join the Community
Have questions, suggestions, or want to follow development?  
**Join our Discord:** [https://discord.gg/XkRh7CEccy](https://discord.gg/XkRh7CEccy)