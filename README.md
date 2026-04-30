<div align="center">

<img src="Assets/apen_logo.png" alt="ApEn Logo" width="120" />

# ApEn — App & File Environment

### Hub de productividad personal · Offline · Sin instalación

*Launcher de apps · Organizer de archivos · Etiquetas · Favoritos · Temas*

---

![.NET](https://img.shields.io/badge/.NET_8-WPF-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-10%2F11-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![C#](https://img.shields.io/badge/C%23-MVVM-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQLite](https://img.shields.io/badge/Config-JSON_local-F7DF1E?style=for-the-badge&logoColor=black)

</div>

---

## ¿Qué es ApEn?

**ApEn** es una aplicación de escritorio para Windows que funciona como hub de productividad personal. Lanza tus apps favoritas con un clic y organiza carpetas de archivos automáticamente por tipo. Todo funciona **100% offline** — sin cuentas, sin servidores, sin que nada salga de tu PC.

---

## Características

| | Función | Descripción |
|:---:|---|---|
| 🚀 | **App Launcher** | Agrega tus aplicaciones favoritas y láncalas con un clic |
| ★ | **Favoritos** | Marca apps como favoritas para tenerlas siempre arriba |
| ✎ | **Renombrar** | Ponle el nombre que quieras a cada app dentro de ApEn |
| 🏷️ | **Etiquetas** | Crea etiquetas con colores personalizados (presets + hex) y filtra por ellas |
| 📂 | **File Organizer** | Clasifica automáticamente los archivos de una carpeta en subcarpetas por tipo |
| 🌗 | **Tema claro/oscuro** | Cambia entre tema oscuro y claro con un clic |
| 💾 | **100% offline** | Configuración guardada en JSON local. Nada sale de tu PC |

---

## Para usuarios: descargar y ejecutar

> No necesitas instalar nada. Solo descarga y ejecuta.

### ⬇️ Descarga

1. Ve a [**Releases**](https://github.com/JuanEstebanHerreraH/ApEn---App-File-Environment/releases)
2. Descarga **`ApEn-v1.0-win-x64.zip`**
3. Descomprime el ZIP en cualquier lugar (Escritorio, Documentos, donde prefieras)
4. Haz doble clic en **`ApEn.exe`**

> **¿Windows muestra "aplicación desconocida"?** → Haz clic en **"Más información" → "Ejecutar de todas formas"**. ApEn no tiene firma de código de pago, pero el código fuente es 100% público y verificable aquí en GitHub.

> **¿Tu antivirus lo marca?** → Es un falso positivo conocido. Los ejecutables compilados localmente sin firma de distribución a veces disparan heurísticos de ML. El código es abierto — puedes revisarlo tú mismo. Agrega `ApEn.exe` a las exclusiones de tu antivirus.

---

## Cómo usar ApEn

### Launcher

```
1. Clic en "+ App" → selecciona un .exe desde tu PC
2. La app aparece en la lista — clic en ▶ para ejecutarla
3. Clic en ★ para marcarla como favorita (aparece arriba)
4. Desde el panel de detalle (derecha) puedes:
   - ✎ Renombrarla (solo dentro de ApEn, no toca el archivo)
   - Asignarle etiquetas de color
5. Clic en "🏷 Filtrar" para filtrar por etiquetas
```

### File Organizer

```
1. Clic en "Examinar..." → elige la carpeta a organizar
2. Clic en "⚡ Organizar"
3. Los archivos se mueven automáticamente a subcarpetas:
```

| Subcarpeta | Tipos de archivo |
|---|---|
| `PDF/` | .pdf |
| `Images/` | .jpg .png .gif .bmp .webp .svg... |
| `Videos/` | .mp4 .mkv .avi .mov .wmv... |
| `Audio/` | .mp3 .wav .flac .aac .ogg... |
| `Docs/` | .docx .xlsx .pptx .txt .csv... |
| `Zips/` | .zip .rar .7z .tar .gz... |
| `Others/` | Todo lo demás |

> Los archivos que ya están en subcarpetas no se duplican.

---

## Para desarrolladores: compilar desde código fuente

### Requisitos

| Herramienta | Versión | Link |
|---|---|---|
| **.NET 8 SDK** | 8.x | https://dotnet.microsoft.com/download/dotnet/8.0 |
| **Windows** | 10 / 11 (64-bit) | — |
| **Visual Studio 2022** *(opcional)* | Community gratis | https://visualstudio.microsoft.com/ |

### Ejecutar en modo desarrollo

```cmd
git clone https://github.com/JuanEstebanHerreraH/ApEn---App-File-Environment.git
cd ApEn---App-File-Environment\ApEn
dotnet build
dotnet run
```

### Generar ejecutable para distribución

```cmd
rem Compilar en modo Release — genera un solo .exe sin dependencias externas
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

rem Crear el ZIP para el release
cd publish
powershell Compress-Archive -Path ApEn.exe -DestinationPath ..\ApEn-v1.0-win-x64.zip
```

El archivo `ApEn-v1.0-win-x64.zip` (~70 MB) contiene el ejecutable listo para distribuir. El usuario final **no necesita instalar .NET** — va todo incluido.

---

## Estructura del proyecto

```
ApEn/
├── Assets/
│   ├── apen_logo.png          ← Logo de la app
│   └── apen.ico               ← Ícono del ejecutable
├── Commands/
│   └── RelayCommand.cs        ← ICommand para MVVM
├── Converters/
│   └── Converters.cs          ← Value converters para XAML
├── Data/
│   └── config.json            ← Tu configuración personal (no se sube a git)
├── Models/
│   ├── AppModel.cs            ← Datos de una app del launcher
│   ├── TagModel.cs            ← Etiqueta con color
│   └── ConfigModel.cs         ← Raíz del JSON
├── Services/
│   ├── ConfigService.cs       ← Leer/guardar config.json
│   ├── FileOrganizerService.cs ← Clasificar archivos por tipo
│   ├── IconService.cs         ← Extraer ícono del .exe
│   └── ThemeService.cs        ← Cambiar tema dark/light
├── Themes/
│   ├── DarkTheme.xaml         ← Paleta oscura
│   └── LightTheme.xaml        ← Paleta clara
├── ViewModels/
│   ├── BaseViewModel.cs       ← INotifyPropertyChanged base
│   ├── AppItemViewModel.cs    ← VM de cada app en la lista
│   └── MainViewModel.cs       ← Lógica principal de la UI
├── Views/
│   ├── MainWindow.xaml/.cs    ← Ventana principal
│   └── TagDialog.xaml/.cs     ← Diálogo de nueva etiqueta
├── App.xaml/.cs               ← Entry point + estilos globales
└── ApEn.csproj                ← .NET 8 WPF
```

---

## Privacidad

- **Sin internet:** ApEn no hace ninguna petición de red. Todo es local.
- **Sin analytics:** No hay telemetría ni recolección de datos.
- **Sin cuenta:** No necesitas registrarte en nada.
- Tu configuración se guarda únicamente en `Data/config.json` en tu PC.

---

## Sobre el .gitignore

El archivo `.gitignore` excluye carpetas que el compilador genera automáticamente y no deben versionarse:

| Carpeta/archivo | Por qué se ignora |
|---|---|
| `bin/` y `obj/` | Generados por `dotnet build`. Pesan 50-200 MB y cada usuario los genera en su PC |
| `Data/config.json` | Tu lista personal de apps y carpetas — no tiene sentido compartirla |
| `.vs/` | Configuración local de Visual Studio (rutas de tu PC) |
| `publish/` | El ejecutable compilado va en Releases, no en el código fuente |

---

<div align="center">

Construido con ❤️ en C# y WPF · Windows 10/11 · .NET 8 · Datos 100% tuyos

</div>
