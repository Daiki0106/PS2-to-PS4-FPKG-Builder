# PS2-to-PS4-FPKG-Builder

![Plataforma](https://img.shields.io/badge/Plataforma-Windows-0078D6?logo=windows)
![Framework](https://img.shields.io/badge/.NET-4.0%2B-blue)
![Lenguaje](https://img.shields.io/badge/C%23-5.0-239120?logo=csharp)
![Compatibilidad](https://img.shields.io/badge/PlayStation%204-GoldHEN%20%7C%20FPKG-informational?logo=playstation)
![Versión](https://img.shields.io/badge/Estado-Beta-orange)

> **Herramienta para convertir juegos de PS2 (ISO, CHD, 7z) a paquetes instalables FPKG de PS4 con descarga automática de carátulas, editor de arte, parches y conversor por lotes.**

---

## ✨ Características Principales

* **Compatibilidad Multiformato Universal:**
  * Soporte nativo y directo para `.iso`, `.bin`, `.img`, `.mdf` y comprimidos en `.chd`.
  * Descompresión multihilo ultrarrápida al vuelo para archivos comprimidos `.zip`, `.7z` y `.rar` (con barra de progreso en tiempo real).
* **Corrección Automática de Discos CD (CD Fixer):**
  * Detección automática de discos en formato CD-ROM (modo 2352 bytes).
  * Conversión y parcheo de sectores a 2048 bytes (ISO estándar) para garantizar que los juegos en CD de PS2 inicien sin crasheos en el emulador de PS4.
* **Descarga Inteligente de Carátulas y Fondos:**
  * Descarga automática multi-espejo de carátulas frontales en alta resolución (2D y 3D).
  * Generación de fondos cinematográficos panorámicos 1080p con ambientación y desenfoque inteligente.
  * Alternancia con un solo clic entre la carátula frontal y el disco original (DVD/CD).
* **Editor de Arte Integrado (PS2 / PS4 Artwork Studio):**
  * Diseña portadas e iconos cuadrados (512x512) y fondos panorámicos (1920x1080) sin salir de la aplicación.
  * Pega directamente imágenes copiadas de internet con <kbd>Ctrl</kbd> + <kbd>V</kbd>.
* **Base de Datos y Parches Comunitarios:**
  * Reconocimiento de más de 10,000 títulos de PS2 por serial o nombre.
  * Detección y aplicación automática de archivos de configuración y parches optimizados (`.txt` / `.lua` para Widescreen 16:9, mejoras de 60 FPS y correcciones gráficas).
* **Modo por Lotes (Batch Mode):**
  * Convierte carpetas completas de juegos en cola de manera automatizada.
* **Instalador Remoto de Red (Send to PS4):**
  * Envío e instalación directa del paquete FPKG a tu consola PS4 conectada por red local (WiFi o cable) mediante *Remote Package Installer*.

---

## 🚀 Guía de Inicio Rápido

1. Descarga o clona este repositorio.
2. Abre **`PS2toPS4Builder.exe`**.
3. Arrastra tu archivo de juego (`.iso`, `.chd`, `.zip`, etc.) a la ventana principal (o presiona **Examinar...**).
4. El programa detectará automáticamente el Serial (ej. `SLUS-21678`), asignará el Title ID (`CUSAxxxxx`), buscará el nombre oficial y descargará el arte correspondiente.
5. *(Opcional)* Ajusta opciones adicionales (Widescreen 16:9, Carga rápida CDVD o abre **Crear Arte** para personalizar portadas).
6. Haz clic en **COMPILAR PKG DE PS4**.

---

## 🛠️ Herramientas Requeridas en la Carpeta `tools/`

Para compilar paquetes `.pkg` finales:
* **`orbis-pub-cmd.exe`**: Colócalo dentro de la subcarpeta `tools/` (herramienta estándar del SDK de publicación de la scene de PS4).
* **`7za.exe`**: Incluido en el proyecto para la extracción de archivos comprimidos.

---

## 💻 Recompilación Instantánea

El proyecto incluye el script **`compilar.bat`** que utiliza el compilador nativo de Windows (`csc.exe` de .NET Framework):
* No necesitas instalar Visual Studio ni programas pesados para compilar.
* Si realizas cambios en el código dentro de `src/`, simplemente haz doble clic en `compilar.bat` y se generará `PS2toPS4Builder.exe` con su icono nativo en menos de 2 segundos.

Si prefieres usar un IDE profesional, puedes abrir el proyecto en **Visual Studio 2022** o **VS Code** mediante el archivo `src/PS2toPS4Builder.csproj`.

---

## 📁 Estructura del Repositorio

```text
PS2-to-PS4-FPKG-Builder/
├── PS2toPS4Builder.exe          # Ejecutable principal
├── compilar.bat                 # Compilador automático nativo
├── app.ico                      # Icono oficial multirresolución
├── README.md                    # Documentación del proyecto
│
├── src/                         # Código fuente C# (.NET)
│   ├── Core/                    # Motor de conversión, parsers y servicios
│   │   ├── IsoReader.cs         # Extractor binario de SYSTEM.CNF y seriales
│   │   ├── CdFixer.cs           # Corrector LBA/sectores 2352 a 2048 para CD
│   │   ├── CoverDownloader.cs   # Descarga multi-espejo de arte y discos
│   │   ├── FormatHelper.cs      # Gestor de extensiones y descompresión 7z
│   │   ├── ConfigDatabase.cs    # Gestor de parches y configuraciones comunitarias
│   │   ├── TitleDatabase.cs     # Base de datos de títulos de PS2
│   │   ├── PkgBuilder.cs        # Orquestador del empaquetado GP4 y PKG
│   │   ├── RemoteInstaller.cs   # Protocolo de envío HTTP a PS4
│   │   └── StudioImageProcessor.cs # Procesamiento visual de arte
│   │
│   └── UI/                      # Formularios Windows Forms
│       ├── MainForm.cs          # Ventana principal
│       ├── BatchForm.cs         # Ventana del modo por lotes
│       ├── ArtStudioForm.cs     # Editor de arte y portadas
│       └── SendToPs4Form.cs     # Ventana de instalación remota
│
├── data/                        # Base de datos de títulos y parches
├── emus/                        # Plantillas de emuladores base (Jak v2, etc.)
└── tools/                       # Binarios de apoyo (orbis-pub-cmd, 7za, etc.)
```

---

## 📜 Licencia y Advertencia Legal

Este software es un proyecto de código abierto desarrollado para uso personal y educativo. No contiene software con derechos de autor de Sony Interactive Entertainment ni juegos comerciales protegidos. Todas las marcas registradas pertenecen a sus respectivos propietarios.
