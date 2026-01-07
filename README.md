# Study Planner

Study Planner, öğrencilerin ve profesyonellerin çalışma süreçlerini organize etmelerine yardımcı olan modern bir WPF uygulamasıdır. .NET 8 ve MVVM mimarisi kullanılarak geliştirilmiştir.

## 🚀 Özellikler

- **Akıllı Odaklanma (Focus Zone)**: Pomodoro benzeri zamanlayıcı ile görev takibi.
- **İstatistikler**: Haftalık ve günlük çalışma analizleri.
- **Çalışma Planı**: Sürükle-bırak destekli görev yönetimi.
- **Kişiselleştirme**: Tema ve profil yönetimi.
- **Yapay Zeka Desteği**: Gemini API entegrasyonu ile akıllı öneriler.

## 🛠 Kullanılan Teknolojiler

- .NET 8 (WPF)
- MVVM Pattern
- MongoDB (Veritabanı)
- DevExpress WPF Controls
- Gemini AI API

## 📋 Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (Yerel kurulum veya Atlas bağlantısı)

## ⚙️ Kurulum ve Yapılandırma

1. **Repoyu Klonlayın**:
   ```bash
   git clone https://github.com/kullaniciadi/StudyPlanner.git
   cd StudyPlanner
   ```

2. **Veritabanı Ayarları**:
   MongoDB'nin yerel makinenizde `mongodb://localhost:27017` adresinde çalıştığından emin olun veya `appsettings.json` dosyasındaki bağlantı dizesini güncelleyin.

3. **API Anahtarı**:
   Yapay zeka özelliklerini kullanmak için `appsettings.json` dosyasına Google Gemini API anahtarınızı ekleyin:
   ```json
   "ApiSettings": {
     "GoogleApiKey": "BURAYA_API_ANAHTARINIZI_YAZIN"
   }
   ```

## ▶️ Uygulamayı Çalıştırma

Geliştirme modunda çalıştırmak için:
```bash
dotnet run
```

## 📦 Tek Dosya Executable (.exe) Oluşturma

Uygulamayı kurulum gerektirmeyen tek bir `.exe` dosyası olarak paketlemek için aşağıdaki komutu kullanın:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Bu işlem tamamlandığında, `.exe` dosyasını şu konumda bulabilirsiniz:
`\bin\Release\net8.0-windows\win-x64\publish\StudyPlanner.exe`

Bu dosyayı arkadaşınıza göndererek kurulum yapmadan kullanmasını sağlayabilirsiniz. (Not: Karşı tarafta MongoDB kurulu olması veya uygulamanın ulaşabileceği bir veritabanı olması gerekebilir).
