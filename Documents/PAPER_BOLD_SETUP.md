# Paper Bold AI Entegrasyonu - Kurulum Rehberi

## 📋 Genel Bakış

Paper Bold AI, StudyPlanner projesine entegre edilmiş bir akademik döküman analiz ve özetleme aracıdır. Google Gemini AI kullanarak PDF belgelerinizi analiz eder, özetler ve hakkında sorular cevaplayabilirsiniz.

## ✨ Özellikler

- 📄 **PDF Yükleme ve İşleme**: PDF dosyalarını yükleyip metin çıkarma
- 🤖 **AI Destekli Özetleme**: Google Gemini 1.5 Flash ile otomatik özet oluşturma
- 🔬 **Model/Algoritma Tespiti**: Akademik makalelerde kullanılan modellerin otomatik tespiti
- 💬 **Soru-Cevap**: Döküman içeriği hakkında doğal dil ile soru sorma
- 🎨 **Minimalist Arayüz**: DevExpress WPF kontrolleri ile modern ve temiz tasarım

## 🚀 Kurulum Adımları

### 1. Google API Anahtarı Alma

Paper Bold AI, Google Gemini API kullanır. API anahtarı almak için:

1. [Google AI Studio](https://makersuite.google.com/app/apikey) adresine gidin
2. Google hesabınızla giriş yapın
3. "Create API Key" butonuna tıklayın
4. API anahtarınızı kopyalayın

### 2. API Anahtarını Ayarlama

API anahtarınızı sisteme tanımlamak için iki yöntem:

#### Yöntem 1: Ortam Değişkeni (Önerilen)

Windows PowerShell'de:
```powershell
[System.Environment]::SetEnvironmentVariable('GOOGLE_API_KEY', 'YOUR_API_KEY_HERE', 'User')
```

Windows CMD'de:
```cmd
setx GOOGLE_API_KEY "YOUR_API_KEY_HERE"
```

#### Yöntem 2: Kod İçinde Değiştirme

`App.xaml.cs` dosyasını açın ve şu satırı bulun:
```csharp
var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? "YOUR_API_KEY_HERE";
```

`YOUR_API_KEY_HERE` yerine API anahtarınızı yazın:
```csharp
var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? "AIza...your-actual-key";
```

### 3. DevExpress Kurulumu

DevExpress WPF kontrolleri için lisans gereklidir. İki seçenek:

#### Seçenek A: DevExpress Lisansınız Varsa

DevExpress NuGet paketleri otomatik olarak yüklenecektir. Eğer özel bir NuGet feed kullanıyorsanız:

1. Visual Studio > Tools > NuGet Package Manager > Package Manager Settings
2. Package Sources'a DevExpress feed'inizi ekleyin
3. Projeyi restore edin: `dotnet restore`

#### Seçenek B: DevExpress Lisansınız Yoksa

Standart WPF kontrolleri ile çalışabilirsiniz. `StudyPlanner.csproj` dosyasında DevExpress paketlerini yorum satırına alın ve `Views/DocumentAnalyzerWindow.xaml` dosyasında DevExpress kontrollerini standart WPF kontrolleri ile değiştirin.

**NOT**: Bu durumda arayüz daha basit görünecektir ancak tüm fonksiyonlar çalışır.

### 4. Projeyi Build Etme

```bash
# Restore NuGet packages
dotnet restore

# Build project
dotnet build

# Run project
dotnet run
```

## 📖 Kullanım

### Ana Ekrandan Paper Bold'a Geçiş

1. Uygulamayı çalıştırın
2. Üst menüde **"📄 Döküman Analiz"** butonuna tıklayın
3. Paper Bold AI penceresi açılacaktır

### PDF Analizi

1. **"📂 PDF Yükle"** butonuna tıklayın
2. Analiz etmek istediğiniz PDF dosyasını seçin
3. AI otomatik olarak:
   - PDF'i okur
   - Özet çıkarır
   - Kullanılan modelleri/algoritmaları tespit eder
4. Sonuçlar sol panelde görüntülenir

### Soru Sorma

1. PDF yüklendikten sonra sağ taraftaki chat panelini kullanın
2. Sorunuzu yazın ve Enter'a basın veya "Gönder" butonuna tıklayın
3. AI, döküman içeriğine dayanarak cevap verir
4. Sohbet geçmişi korunur

## 🏗️ Mimari

### Katmanlar

```
StudyPlanner/
├── Models/                    # Domain modelleri
│   ├── DocumentSummary.cs     # PDF özet modeli
│   └── ChatMessage.cs         # Sohbet mesajı modeli
├── Interfaces/                # Servis arayüzleri
│   ├── IAiService.cs          # AI servisi
│   └── IPdfService.cs         # PDF işleme servisi
├── Services/                  # Servis implementasyonları
│   ├── GeminiAiService.cs     # Google Gemini entegrasyonu
│   └── PdfService.cs          # PDF okuma/işleme
├── ViewModels/                # MVVM ViewModels
│   └── DocumentAnalyzerViewModel.cs
└── Views/                     # UI katmanı
    ├── DocumentAnalyzerWindow.xaml
    └── DocumentAnalyzerWindow.xaml.cs
```

### Dependency Injection

Tüm servisler `App.xaml.cs` içinde DI container'a kaydedilir:

```csharp
services.AddSingleton<IAiService>(provider => new GeminiAiService(apiKey));
services.AddTransient<IPdfService, PdfService>();
services.AddTransient<DocumentAnalyzerViewModel>();
services.AddTransient<DocumentAnalyzerWindow>();
```

## 🎨 Tasarım Prensipleeri

### Minimalizm

- **Temiz renkler**: Mavi (#2196F3) ana renk, turuncu (#FF5722) vurgu rengi
- **Beyaz alan kullanımı**: İçerik nefes alabilir
- **Modern border radius**: 12-16px yuvarlatılmış köşeler
- **Subtle shadows**: Derinlik için hafif gölgeler

### Kullanıcı Deneyimi

- **Boş durum (Empty State)**: Kullanıcıyı yönlendiren açık mesajlar
- **Yükleme göstergeleri**: İşlem sürerken feedback
- **Responsive layout**: Farklı ekran boyutlarına uyum
- **Enter tuşu desteği**: Hızlı soru sorma

## 🔧 Konfigürasyon

### API Ayarları

`Services/GeminiAiService.cs`:
```csharp
private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
```

**Model**: Gemini 1.5 Flash (hızlı ve ekonomik)  
**Temperature**: 0.1 (tutarlı sonuçlar için düşük)  
**Max Output Tokens**: 2048

### PDF Ayarları

- **Desteklenen format**: .pdf
- **Maksimum boyut**: Sınırsız (ancak büyük dosyalar yavaş işlenebilir)
- **Encoding**: UTF-8

## 📊 Performans İpuçları

1. **PDF Boyutu**: 5MB'den küçük PDF'ler daha hızlı işlenir
2. **İnternet Bağlantısı**: API çağrıları için stabil internet gerekir
3. **API Limitleri**: Google Gemini API'nin rate limit'leri vardır

## 🐛 Sorun Giderme

### "API Key geçersiz" Hatası

- API anahtarınızın doğru girildiğinden emin olun
- Google AI Studio'da API anahtarının aktif olduğunu kontrol edin

### PDF Okunamıyor

- PDF'in şifreli olmadığından emin olun
- PDF'in metin içerdiğini kontrol edin (taranmış görüntüler desteklenmez)

### DevExpress Hataları

- DevExpress lisansınızın geçerli olduğundan emin olun
- Alternatif olarak standart WPF kontrollerini kullanın

## 🎯 Sunum İpuçları

### Demo Senaryosu

1. **Giriş**: StudyPlanner ana ekranını göster
2. **Geçiş**: Paper Bold butonuna tıkla
3. **PDF Yükle**: Örnek akademik makale yükle
4. **Özet**: Otomatik oluşturulan özeti göster
5. **Modeller**: Tespit edilen algoritmaları göster
6. **Soru-Cevap**: Canlı soru sor ve cevap al
7. **Vurgular**: 
   - Minimalist tasarım
   - Hız ve kullanım kolaylığı
   - AI entegrasyonu

### Vurgulanacak Noktalar

✅ **Clean Architecture**: Katmanlı mimari  
✅ **SOLID Principles**: Dependency Injection kullanımı  
✅ **Modern UI**: DevExpress ile profesyonel görünüm  
✅ **AI Integration**: Google Gemini RAG pipeline  
✅ **Minimize Code**: Maintainable ve temiz kod  

## 📚 Kaynaklar

- [Google Gemini API Docs](https://ai.google.dev/docs)
- [DevExpress WPF](https://docs.devexpress.com/WPF/7875/wpf-controls)
- [iText7 PDF Library](https://itextpdf.com/en/resources/documentation)
- [Original Paper Bold Project](https://github.com/enesmanan/paper-bold)

## 👨‍💻 Geliştirici Notları

### Gelecek Geliştirmeler

- [ ] Vector database (ChromaDB) entegrasyonu
- [ ] Çoklu dil desteği (TR/EN)
- [ ] Sohbet geçmişini kaydetme
- [ ] Birden fazla PDF'i karşılaştırma
- [ ] Export özellikleri (Word, Markdown)
- [ ] Batch processing

### Katkıda Bulunma

Projeye katkıda bulunmak için:
1. Fork yapın
2. Feature branch oluşturun
3. Değişikliklerinizi commit edin
4. Pull request açın

---

**Not**: Bu döküman sunum öncesi okunmalı ve tüm adımlar test edilmelidir!

