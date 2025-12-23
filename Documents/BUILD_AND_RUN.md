# StudyPlanner - Build ve Çalıştırma Rehberi

## 🚀 Hızlı Başlangıç

### Ön Gereksinimler

1. **Visual Studio 2022** (Community, Professional veya Enterprise)
   - Workload: `.NET desktop development`
   - Component: `.NET 6.0 Runtime`

2. **.NET 6.0 SDK**
   - İndirme: https://dotnet.microsoft.com/download/dotnet/6.0

3. **Git** (Opsiyonel)

### Adım 1: Projeyi Açma

#### Visual Studio ile:

1. `StudyPlanner.sln` dosyasına çift tıklayın
2. Visual Studio otomatik açılacaktır

#### Komut Satırı ile:

```bash
cd "C:\Users\LENOVO\Documents\Visual Studio 2022\My Codes\StudyPlanner"
start StudyPlanner.sln
```

### Adım 2: NuGet Paketlerini Restore Etme

#### Visual Studio'da:

1. Solution Explorer'da solution'a sağ tıklayın
2. **Restore NuGet Packages** seçeneğini seçin
3. Alternatif: `Tools > NuGet Package Manager > Package Manager Console`

```powershell
dotnet restore
```

#### Komut Satırı:

```bash
dotnet restore
```

### Adım 3: Projeyi Build Etme

#### Visual Studio'da:

- **Kısayol**: `Ctrl + Shift + B`
- **Menu**: `Build > Build Solution`

#### Komut Satırı:

```bash
# Debug Build
dotnet build

# Release Build
dotnet build --configuration Release
```

### Adım 4: Çalıştırma

#### Visual Studio'da:

- **Kısayol**: `F5` (Debug mode)
- **Kısayol**: `Ctrl + F5` (Without debugging)
- **Menu**: `Debug > Start Debugging`

#### Komut Satırı:

```bash
# Debug mode
dotnet run

# Release mode
dotnet run --configuration Release
```

#### Executable'dan Çalıştırma:

```bash
# Debug
.\bin\Debug\net6.0-windows\StudyPlanner.exe

# Release
.\bin\Release\net6.0-windows\StudyPlanner.exe
```

## 🔧 Build Configurations

### Debug Configuration

```bash
dotnet build --configuration Debug
```

**Özellikler**:
- Debug symbols dahil
- Optimizasyon yok
- Daha fazla log
- Development için ideal

### Release Configuration

```bash
dotnet build --configuration Release
```

**Özellikler**:
- Optimize edilmiş kod
- Debug symbols yok
- Daha küçük dosya boyutu
- Production için ideal

## 📦 Publish (Deployment)

### Self-Contained Deployment

Tüm .NET runtime ile birlikte:

```bash
# Windows x64
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish/win-x64

# Windows x86
dotnet publish --configuration Release --runtime win-x86 --self-contained true --output ./publish/win-x86
```

### Framework-Dependent Deployment

.NET runtime yüklü olması gerekir:

```bash
dotnet publish --configuration Release --self-contained false --output ./publish/framework-dependent
```

### Single File Publish

Tek bir executable dosya:

```bash
dotnet publish --configuration Release --runtime win-x64 --self-contained true /p:PublishSingleFile=true --output ./publish/single-file
```

## 🐛 Hata Giderme

### Build Hataları

#### "Project file is incomplete"

**Çözüm**: Clean solution ve rebuild

```bash
dotnet clean
dotnet build
```

#### "NuGet packages are not restored"

**Çözüm**:

```bash
dotnet restore --force
```

#### "Cannot find Microsoft.Extensions.DependencyInjection"

**Çözüm**:

```bash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet restore
```

### Runtime Hataları

#### "Could not load file or assembly"

**Çözüm**: Build klasörünü temizle

```bash
dotnet clean
rd /s /q bin
rd /s /q obj
dotnet build
```

#### "Main window not showing"

**Çözüm**: 
1. `App.xaml.cs` dosyasında `Application_Startup` metodunu kontrol edin
2. `StartupObject` property'sinin `StudyPlanner.App` olduğundan emin olun

### XAML Hataları

#### "Cannot locate resource"

**Çözüm**: Build action'ı kontrol edin
- XAML files: `Page` veya `ApplicationDefinition`
- Code-behind files: `Compile`

## 🔍 Debugging

### Visual Studio Debugging

1. Breakpoint eklemek için satır numarasının soluna tıklayın
2. `F5` ile debug mode'da çalıştırın
3. Breakpoint'te durduğunda:
   - `F10`: Step Over
   - `F11`: Step Into
   - `Shift + F11`: Step Out
   - `F5`: Continue

### Console Output

Debug bilgilerini görmek için:

```csharp
System.Diagnostics.Debug.WriteLine("Debug message");
```

Output Window'da görünecektir: `Debug > Windows > Output`

## 📊 Performance Profiling

### Visual Studio Profiler

1. `Debug > Performance Profiler`
2. Analiz türünü seçin:
   - CPU Usage
   - Memory Usage
   - .NET Object Allocation

### dotnet-trace

```bash
# Install
dotnet tool install --global dotnet-trace

# Collect trace
dotnet-trace collect --process-id <PID>
```

## 🧪 Testing

### Unit Test Ekleme

1. Test projesi oluştur:

```bash
dotnet new mstest -n StudyPlanner.Tests
dotnet sln add StudyPlanner.Tests\StudyPlanner.Tests.csproj
```

2. Reference ekle:

```bash
cd StudyPlanner.Tests
dotnet add reference ..\StudyPlanner.csproj
```

3. Testleri çalıştır:

```bash
dotnet test
```

## 📝 Önemli Notlar

### .NET Version

Proje **.NET 6.0** kullanır. Eğer farklı bir versiyon kullanmak isterseniz:

1. `StudyPlanner.csproj` dosyasında:
```xml
<TargetFramework>net7.0-windows</TargetFramework>
```

2. SDK'yı yükleyin ve restore edin:
```bash
dotnet restore
```

### DevExpress

DevExpress kullanmak için:
1. `StudyPlanner.csproj` dosyasında DevExpress paket referanslarını aktif edin
2. `DEVEXPRESS_SETUP.md` dosyasını okuyun
3. Restore ve rebuild yapın

## 🚀 CI/CD

### GitHub Actions

`.github/workflows/build.yml`:

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 6.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

## 🛠️ Development Tools

### Recommended Extensions

Visual Studio:
- ReSharper (Opsiyonel)
- Productivity Power Tools
- XAML Styler

VS Code:
- C# Extension
- XAML Extension
- .NET Core Test Explorer

## 📞 Yardım

Build ile ilgili sorunlar için:
1. `BUILD_ERRORS.log` dosyasını kontrol edin
2. GitHub Issues açın
3. [.NET Documentation](https://docs.microsoft.com/dotnet) 'a bakın

---

**Son Güncelleme**: 2024
**Versiyon**: 2.0.0



