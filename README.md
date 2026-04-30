# ⚡ ApEn — App & File Environment

Hub de productividad personal para Windows.
Stack: **C# · .NET 8 · WPF · MVVM**

---

## ✅ Requisitos

| Herramienta | Versión | Link |
|---|---|---|
| **Windows** | 10 / 11 (64-bit) | — |
| **.NET 8 SDK** | 8.x | https://dotnet.microsoft.com/download/dotnet/8.0 |

---

## 🚀 Ejecutar

```bash
# IMPORTANTE: borrar bin/ y obj/ si vienes de una versión anterior
rmdir /s /q bin
rmdir /s /q obj

dotnet build
dotnet run
```

---

## 🔴 Sobre antivirus (Malwarebytes / Windows Defender)

### ¿Por qué puede aparecer una alerta?

**ApEn no es malware.** Sin embargo, algunos antivirus como Malwarebytes pueden marcarlo
como falso positivo con el nombre `MachineLearning/Anomalous`.

**La razón técnica:**

Versiones anteriores usaban P/Invoke manual (llamadas directas a `shell32.dll` y `user32.dll`
escritas en el código C#). Esta técnica es idéntica a la que usan troyanos bancarios para
crear ventanas overlay invisibles. El motor de Machine Learning de Malwarebytes detecta ese
patrón y lo bloquea preventivamente aunque el software sea legítimo.

**¿Cómo está resuelto en la versión actual?**

La versión actual usa **dos cambios críticos**:

1. **`System.Drawing.Common`** (paquete oficial de Microsoft, firmado digitalmente)
   en lugar de P/Invoke manual para extraer íconos de aplicaciones.

2. **`WindowChrome`** (API oficial de WPF de Microsoft) en lugar de
   `AllowsTransparency="True"` + `WindowStyle="None"` para la ventana personalizada.
   Esta combinación era la firma exacta de keyloggers y troyanos bancarios.

**Si tu antivirus aún lo marca:**

1. Borra las carpetas `bin/` y `obj/` completamente.
2. Vuelve a compilar con `dotnet build`.
3. El nuevo `.dll` generado **no** usa las técnicas que disparan el heurístico.
4. Si persiste, agrega la carpeta `ApEn\bin\Debug\net8.0-windows\` a las exclusiones
   del antivirus. Es seguro hacerlo porque el código es abierto y verificable.

---

## 📁 Estructura del proyecto

```
ApEn/
├── .gitignore              ← Archivos ignorados por git (ver explicación abajo)
├── ApEn.csproj             ← .NET 8 WPF + System.Drawing.Common
├── App.xaml / .cs          ← Entry point + estilos globales
├── Assets/
│   ├── apen.ico            ← Ícono de la app (generado desde el logo)
│   └── apen_logo.png       ← Logo PNG para la UI
├── Commands/
│   └── RelayCommand.cs     ← ICommand para MVVM
├── Converters/
│   └── Converters.cs       ← Value converters para bindings XAML
├── Data/
│   └── config.json         ← TU configuración personal (no se sube a git)
├── Models/                 ← AppModel, TagModel, ConfigModel
├── Services/
│   ├── ConfigService.cs    ← Leer/guardar config.json
│   ├── FileOrganizerService.cs ← Clasificar archivos
│   ├── IconService.cs      ← Extraer ícono del .exe (sin P/Invoke)
│   └── ThemeService.cs     ← Cambiar tema dark/light
├── Themes/
│   ├── DarkTheme.xaml      ← Paleta de colores oscura
│   └── LightTheme.xaml     ← Paleta de colores clara
├── ViewModels/             ← Lógica de UI (MVVM)
└── Views/
    ├── MainWindow.xaml/.cs ← Ventana principal
    └── TagDialog.xaml/.cs  ← Diálogo de nueva etiqueta
```

---

## 📄 Sobre el .gitignore — archivos "ocultos"

El archivo `.gitignore` le dice a git qué carpetas y archivos NO subir al repositorio.
No son archivos maliciosos ni ocultos en el sentido sospechoso — son archivos que el
compilador genera automáticamente y que no tienen sentido versionar.

### `bin/` — Archivos de compilación

Contiene el ejecutable compilado (`ApEn.dll`, `ApEn.exe`) y todas sus dependencias.
**Se genera automáticamente** con `dotnet build`. No se sube porque:
- Puede pesar 50-200 MB
- Cada usuario lo genera en su propia máquina
- Los antivirus pueden marcar DLLs recién compiladas sin firma de distribución

### `obj/` — Archivos intermedios

Archivos temporales que el compilador usa internamente para optimizar compilaciones
subsecuentes. No contienen código ejecutable útil. Se regeneran solos.

### `Data/config.json` — Tu configuración personal

Guarda qué aplicaciones has agregado al launcher y tu carpeta del Organizer.
No se sube porque es información personal de tu PC (rutas como `C:\Users\tu_nombre\...`).
Cada usuario tiene el suyo propio y se crea automáticamente al usar ApEn.

### `.vs/` y `.idea/` — Configuración del IDE

Carpetas que Visual Studio y JetBrains Rider crean para guardar preferencias locales
(ventanas abiertas, breakpoints, etc.). No son útiles para otros usuarios.

---

## 🎮 Cómo usar ApEn

### Launcher
- **+ Agregar** → selecciona un `.exe`
- **☰** → abre/cierra el panel de etiquetas
- **★** → marcar como favorito (aparece en sección Favoritos arriba)
- **✎** en el panel de detalle → renombrar la app (solo dentro de ApEn)
- **+ Nueva** en Etiquetas → abre diálogo para crear etiqueta con color (presets + hex)

### Organizer
- **Examinar** → elige la carpeta
- **⚡ Organizar** → clasifica los archivos en subcarpetas

---

*ApEn v3.1 — .NET 8 + WPF + MVVM*
