# StoryMode 🖋️

**A world-builder's word processor.**

StoryMode is a cross-platform book writing software built with **C#** and **AvaloniaUI**. It bridges the gap between creative writing and world-building by integrating a live **Codex** directly into the editor, allowing you to manage characters, locations, and lore without leaving your flow.

---

## 🚀 Status

![Platform](https://img.shields.io/badge/platform-windows%20%7C%20macos%20%7C%20linux-lightgrey?style=flat)
[![Trello](https://img.shields.io/badge/Trello-Roadmap-blue?&logo=trello)](https://trello.com/b/zOYE8yuU/storymode)

---

## ✨ Features

* **The Codex:** A relational database for your lore. Manage characters, locations, and magic systems in one place using a dedicated SplitView.
* **Smart Hyperlinks:** Seamlessly link your text to Codex entries using `@` or `[[` triggers. Clicking a name in the text instantly opens its data entry.
* **Multi-Language:** Powered by a flexible, human-readable **TOML** localization engine.
* **Clean UI:** A distraction-free "Dark Mode" interface designed for focused writing sessions.
* **Cross-Platform:** Built on AvaloniaUI to run natively on Windows, macOS, and Linux.

---

## 🛠️ Tech Stack

* **Framework:** [AvaloniaUI](https://avaloniaui.net/) (v11+)
* **Text Engine:** [AvaloniaEdit](https://github.com/AvaloniaUI/AvaloniaEdit)
* **Language:** C# 14 / .NET 10
* **Data:** SQLite + Entity Framework Core
* **Configuration:** [Tomlyn](https://github.com/xoofx/Tomlyn) (TOML Support)

---

## 📂 Project Structure

```text
StoryMode/
├── .github/
│   └── workflows/   # CI/CD Build pipeline
├── Assets/
│   ├── Lang/        # TOML Localization files (e.g., en-US.toml)
│   └── Styles.axaml # Global UI styling and Themes
├── Models/          # SQLite Entities (CodexEntry, Character, Location)
├── Services/        # LocalizationManager, CodexService
├── ViewModels/      # MVVM Logic (CommunityToolkit.Mvvm)
└── Views/           # Avalonia XAML Controls (Editor, Codex, Settings)
