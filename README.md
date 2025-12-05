# 🖥️ MuteX – Global Microphone Mute Tool for Windows

MuteX is a lightweight, modern Windows utility that allows you to **mute and unmute your microphone globally using a customizable hotkey**.  
It runs silently in the **system tray**, supports **automatic startup**, and features a polished **Windows 11–styled UI** with Mica effects.

---

## ✨ Features

- 🎙️ **Global microphone mute/unmute**
- ⌨️ **Customizable global hotkey**
- 🔊 **Sound feedback (mute / unmute)**
- 🟢 **Always-running background service**
- 🪟 **Modern Windows 11 UI (Mica, rounded corners, borderless)**
- 🖱️ **System tray integration**
- 🏁 **Start with Windows**
- 🔒 **Prevents launching multiple instances**

---

## 📦 Installation

### **Option A – MSIX (Coming soon)**
Modern install method with automatic updates.

### **Option B – Setup EXE (Recommended)**
Classic Windows installer using **Inno Setup**.

Installers are located in:

```
/installers/
    ├── MSIX/
    └── SetupExe/
```

---

## 🚀 Running MuteX from Source

### **Requirements**
- Windows 10 or Windows 11  

### **Build**
```bash
dotnet build src/MuteX.App/MuteX.App.csproj -c Release
```

### **Publish standalone executable**
```bash
dotnet publish src/MuteX.App/MuteX.App.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o publish-win
```

The output `.exe` runs even without .NET installed.

---

## 📁 Project Structure

```
MuteX/
├── installers/         # Setup EXE & MSIX installers
├── src/
│   └── MuteX.App/
│       ├── Core/       # Audio, hotkeys, settings, startup logic
│       ├── UI/         # Tray icon, icons, sounds
│       ├── Windows/    # Hotkey configuration window
│       ├── MainWindow.xaml
│       └── MuteX.App.csproj
├── README.md
└── MuteX.sln
```

---

## 🛠️ Technologies Used

- **WPF (.NET 8)**
- **Windows DWM APIs** (Mica background, rounded corners)
- **NAudio** (microphone mute/unmute)
- **NotifyIcon** (tray integration)
- **JSON config system**
- **Single-file publishing**

---

## 📜 License

MIT License — free for personal and commercial use.

---

## 👤 Author

**Diego Almeida**  
Developer & Creator of MuteX

---

## ⭐ Support the Project

If you like MuteX, please consider starring the repository.  
Feedback & contributions are welcome!
