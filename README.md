# 🧷 Symbolic Link Importer

A lightweight Unity Editor tool for creating **symbolic links** directly from the Project window. Perfect for sharing assets across projects without duplicating files.

---

## 🧠 What is it?

**Symbolic Link Importer** is a Unity Editor extension that lets you create symbolic links (symlinks) to files or folders outside your project, integrating them seamlessly into the Unity workflow.

---

## 🎯 What is it for?

- Share assets between multiple Unity projects.
- Avoid duplication and save disk space.
- Maintain a modular, organized project structure.

---

## 📦 Installation

### Option 1 — Clone the repository:
1. Go to `Window/Package Management/Package Manager` or `Window/Package Manager`.
2. Click on `+` and `Install package from git URL`
3. Paste the repository URL on field:
   ```bash
   https://github.com/AnderSystems/SymbolicLinkImporter.git
   ```

### Option 2 — Manual download:
1. Download the repository as `.zip` from GitHub.
2. Extract it into your Unity project at:
   ```
   Assets/Editor/SymbolicLinkImporter
   ```

> ✅ Tested on Unity 2021 and above (recommended: Unity 2022+)

---

## ⚙️ How to use

1. In Unity, right-click inside the **Project** panel.
2. Go to:
   ```
   Assets > Symbolic Link > Create Symbolic Link
   ```
![Captura de tela 2025-06-11 125109](https://github.com/user-attachments/assets/5b257f8e-d525-4bbf-8308-7c27d95ba35c)

3. Use the window to:
   - Select a **target path** inside your project
   - Choose the **source path** from your file system
![Captura de tela 2025-06-11 125218](https://github.com/user-attachments/assets/1966f644-893c-4f29-97f6-151dfe2365a1)

4. Click **Create Link**.

Done. Your project will now reference the external file/folder as if it were local.
![Captura de tela 2025-06-11 125328](https://github.com/user-attachments/assets/6def5dea-f257-4463-9066-db7abd175ae2)


---

## ❗ Known Issues

- **"Access denied" errors** on Windows → run Unity as Administrator.
- **On macOS/Linux** → may require additional permissions via terminal.
- **Broken links** → moving or renaming the original folder will break the symlink.
- **Antivirus false positives** → some security software might block symlink creation.

---

## 👤 Author

Made with 💻 by [Ander Systems](https://github.com/AnderSystems)  
Built by Marco — with help from ChatGPT.

---
