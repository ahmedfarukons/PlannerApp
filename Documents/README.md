# Study Planner - Çalışma Planlayıcı

Modern ve profesyonel çalışma planlama uygulaması. WPF + MVVM + Clean Architecture ile geliştirilmiştir.

## 🎯 Özellikler

- ✅ **Modern UI/UX** - WPF ile modern arayüz
- ✅ **MVVM Pattern** - Temiz kod mimarisi
- ✅ **Dependency Injection** - Loosely coupled tasarım
- ✅ **Repository Pattern** - Veri erişim katmanı
- ✅ **SOLID Prensipleri** - Profesyonel kod yapısı
- ✅ **Clean Architecture** - Katmanlı mimari
- ✅ **XML Serialization** - Veri saklama
- ✅ **DevExpress Ready** - DevExpress entegrasyona hazır

## 🏗️ Proje Yapısı

```
StudyPlanner/
├── Models/              # Entity sınıfları
│   ├── BaseEntity.cs
│   └── StudyPlanItem.cs
├── ViewModels/          # MVVM ViewModels
│   ├── ViewModelBase.cs
│   ├── RelayCommand.cs
│   └── MainViewModel.cs
├── Views/               # WPF Views
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Services/            # Business logic
│   ├── XmlDataService.cs
│   └── DialogService.cs
├── Repositories/        # Data access
│   └── StudyPlanRepository.cs
├── Interfaces/          # Contracts
│   ├── IRepository.cs
│   ├── IDataService.cs
│   └── IDialogService.cs
├── App.xaml            # Application resources
└── App.xaml.cs         # DI Configuration
```

## 🔧 Teknolojiler

- **.NET 6.0** - Modern .NET framework
- **WPF** - Windows Presentation Foundation
- **MVVM** - Model-View-ViewModel pattern
- **Microsoft.Extensions.DependencyInjection** - DI Container
- **C# 10** - Latest C# features
- **XAML** - UI definition
- **XML Serialization** - Data persistence

## 📋 Gereksinimler

- Visual Studio 2022 veya üzeri
- .NET 6.0 SDK veya üzeri
- Windows 10/11

## 🚀 Kurulum

1. Projeyi klonlayın veya indirin
2. Visual Studio 2022 ile `StudyPlanner.sln` dosyasını açın
3. NuGet paketlerini restore edin
4. F5 ile çalıştırın

## 📦 DevExpress Kurulumu (Opsiyonel)

DevExpress kontrolleri kullanmak için:

1. DevExpress hesabınızdan lisans edinin
2. `StudyPlanner.csproj` dosyasında DevExpress paket referanslarını aktif edin
3. NuGet paketlerini restore edin
4. `App.xaml` dosyasında DevExpress temalarını ekleyin

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/DevExpress.Xpf.Themes.Office2019Colorful;component/Office2019Colorful.xaml"/>
</ResourceDictionary.MergedDictionaries>
```

## 🎨 SOLID Prensipleri

Proje, SOLID prensiplerini takip eder:

1. **Single Responsibility** - Her sınıf tek bir sorumluluğa sahip
2. **Open/Closed** - Genişletmeye açık, değişikliğe kapalı
3. **Liskov Substitution** - Base class yerine derived class kullanılabilir
4. **Interface Segregation** - Küçük ve spesifik interface'ler
5. **Dependency Inversion** - Üst seviye modüller interface'lere bağımlı

## 📖 Kullanım

### Çalışma Planı Ekleme

1. Sol panelde form alanlarını doldurun
2. "Ekle" butonuna tıklayın
3. Yeni plan sağ paneldeki listede görünür

### Kaydetme ve Yükleme

- **Kaydet**: Üst menüden "💾 Kaydet" butonuna tıklayın
- **Yükle**: Üst menüden "📂 Yükle" butonuna tıklayın

### Arama ve Filtreleme

- Arama kutusuna metin girin
- "Sadece Tamamlananlar" checkbox'ını işaretleyin

## 🧪 Test

Birim testler için:
```bash
dotnet test
```

## 📝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje eğitim amaçlıdır.

## 👥 İletişim

Sorularınız için issue açabilirsiniz.

## 🔄 Versiyon Geçmişi

### v2.0.0 (WPF + MVVM)
- WPF ile yeniden yazıldı
- MVVM pattern uygulandı
- Dependency Injection eklendi
- Clean Architecture
- SOLID prensipleri

### v1.0.0 (WinForms)
- İlk versiyon
- Windows Forms
- Temel CRUD işlemleri

---

**Not**: Bu proje .NET uygulama geliştirme standartlarına ve OOP prensiplerine uygun olarak geliştirilmiştir.



