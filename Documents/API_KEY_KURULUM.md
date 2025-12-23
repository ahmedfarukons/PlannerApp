# 🔑 API Key Kurulum Rehberi

## Hızlı Kurulum (3 Adım)

### 1️⃣ Google API Key Alın

1. **https://makersuite.google.com/app/apikey** adresine gidin
2. Google hesabınızla giriş yapın
3. **"Create API Key"** butonuna tıklayın
4. Oluşan key'i kopyalayın (örn: `AIzaSyC...`)

### 2️⃣ API Key'i Projeye Ekleyin

**Seçenek A: appsettings.Development.json (Önerilen)**

`appsettings.Development.json` dosyasını açın ve API key'inizi yapıştırın:

```json
{
  "ApiSettings": {
    "GoogleApiKey": "BURAYA_API_KEY_YAPISTIRIN",
    "ApiBaseUrl": "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent",
    "Temperature": 0.1,
    "MaxOutputTokens": 2048
  }
}
```

> Not: Bazı anahtarlarda `v1beta` endpoint’i `404 NOT_FOUND` döndürebiliyor. Bu durumda `v1` kullanın.
> Ayrıca `gemini-1.5-flash-001` gibi model adları bu endpointte desteklenmeyebilir ve `404 NOT_FOUND` döndürebilir.

**Seçenek B: .env dosyası**

Proje klasöründe `.env` dosyası oluşturun ve içine yazın:

```
GOOGLE_API_KEY=AIzaSyC_BURAYA_API_KEY_YAPISTIRIN
```

### 3️⃣ Projeyi Çalıştırın

```bash
dotnet restore
dotnet build
dotnet run
```

---

## 📋 Detaylı Açıklama

### Konfigürasyon Öncelik Sırası

Program API key'i şu sıraya göre arar:

1. **Environment Variable** (en yüksek öncelik)
   ```powershell
   $env:GOOGLE_API_KEY = "your_key"
   ```

2. **.env dosyası**
   ```
   GOOGLE_API_KEY=your_key
   ```

3. **appsettings.Development.json**
   ```json
   {
     "ApiSettings": {
       "GoogleApiKey": "your_key"
     }
   }
   ```

4. **appsettings.json** (varsayılan, boş bırakılmalı)

### Hangi Yöntemi Seçmeliyim?

| Yöntem | Avantaj | Dezavantaj | Ne Zaman Kullan? |
|--------|---------|------------|------------------|
| **appsettings.Development.json** | ✅ Kolay<br>✅ IDE desteği | ⚠️ Git'e eklenmemeli | **Önerilen** - Geliştirme için |
| **.env** | ✅ Standart<br>✅ Güvenli | ⚠️ Manuel oluşturma | Prodüksiyon benzeri |
| **Environment Variable** | ✅ En güvenli<br>✅ Kalıcı | ❌ Teknik bilgi gerekir | CI/CD, Production |

---

## 🔒 Güvenlik

### ⚠️ ÖNEMLİ: API Key'i Asla Git'e Eklemeyin!

`.gitignore` dosyası zaten şunları ekliyor:
```gitignore
.env
appsettings.Development.json
```

### ✅ Güvenli Pratikler

1. **Asla `appsettings.json`'a key yazmayın** (Git'te takip edilir)
2. **`appsettings.Development.json` kullanın** (Git'te takip edilmez)
3. **Team ile paylaşırken** örnek dosya paylaşın:
   ```
   appsettings.Development.json.example
   ```

---

## 🧪 Test Etme

### API Key Doğru mu?

Projeyi çalıştırın ve PDF yükleyin. Eğer hata alırsanız:

**Hata: "API Key not valid"**
- ✅ Key doğru kopyalandı mı kontrol edin
- ✅ Key etkin mi? (Google AI Studio'da kontrol)
- ✅ Dosya kaydedildi mi?

**Hata: "API Key bulunamadı"**
- ✅ Dosya adı doğru mu? (`appsettings.Development.json`)
- ✅ Dosya proje kök dizininde mi?
- ✅ JSON syntax'ı doğru mu?

---

## 🛠️ Sorun Giderme

### "API Key bulunamadı" Hatası

```csharp
// Hata mesajı:
Google API Key bulunamadı!

Lütfen aşağıdaki yöntemlerden birini kullanın:
1. .env dosyasına GOOGLE_API_KEY=your_key_here ekleyin
2. appsettings.Development.json içinde ApiSettings:GoogleApiKey ayarlayın
3. Environment variable olarak GOOGLE_API_KEY tanımlayın

API Key almak için: https://makersuite.google.com/app/apikey
```

**Çözüm:**
1. `appsettings.Development.json` dosyasını açın
2. `"GoogleApiKey": ""` satırını bulun
3. API key'inizi tırnak içine yapıştırın
4. Dosyayı kaydedin
5. Uygulamayı yeniden başlatın

### JSON Syntax Hatası

**Yanlış:**
```json
{
  "ApiSettings": {
    "GoogleApiKey": AIzaSyC123  // ❌ Tırnak yok
  }
}
```

**Doğru:**
```json
{
  "ApiSettings": {
    "GoogleApiKey": "AIzaSyC123"  // ✅ Tırnak var
  }
}
```

### Key Kopyalama Hatası

API key kopyalarken:
- ✅ Başında/sonunda boşluk olmamalı
- ✅ Tam key'i kopyalayın (genelde 39 karakter)
- ✅ Özel karakterler de dahil

---

## 📚 Ek Bilgiler

### Tüm Konfigürasyonlar

`appsettings.Development.json` içindeki tüm ayarlar:

```json
{
  "ApiSettings": {
    "GoogleApiKey": "your_key_here",           // Google Gemini API Key
    "ApiBaseUrl": "...",                        // API URL (değiştirmeyin)
    "Temperature": 0.1,                         // AI yaratıcılık (0-1)
    "MaxOutputTokens": 2048                     // Maksimum yanıt uzunluğu
  },
  "AppSettings": {
    "DefaultLanguage": "tr",                    // Türkçe/İngilizce (tr/en)
    "MaxPdfSizeMB": 10,                         // Max PDF boyutu
    "EnableLogging": true                       // Loglama açık/kapalı
  }
}
```

### Environment Variable Kalıcı Yapma

**Windows PowerShell (Kalıcı):**
```powershell
[System.Environment]::SetEnvironmentVariable('GOOGLE_API_KEY', 'YOUR_KEY', 'User')
```

**Windows CMD (Geçici):**
```cmd
set GOOGLE_API_KEY=YOUR_KEY
```

**Windows CMD (Kalıcı):**
```cmd
setx GOOGLE_API_KEY "YOUR_KEY"
```

---

## ✅ Kontrol Listesi

Başlamadan önce kontrol edin:

- [ ] Google AI Studio'dan API key aldım
- [ ] `appsettings.Development.json` dosyasını oluşturdum
- [ ] API key'i doğru yapıştırdım
- [ ] JSON syntax'ı doğru (tırnaklar, virgüller)
- [ ] Dosyayı kaydettim
- [ ] `.gitignore` dosyası mevcut
- [ ] Proje build alıyor

**Hazırsınız! 🚀**

---

## 🆘 Hala Çalışmıyor mu?

1. **Projeyi temizleyin:**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Dosya yapısını kontrol edin:**
   ```
   StudyPlanner/
   ├── StudyPlanner.csproj
   ├── appsettings.json          ✅ Git'te
   ├── appsettings.Development.json  ✅ Git'te değil (key burada)
   ├── App.xaml.cs
   └── ...
   ```

3. **Yeni API key deneyin:**
   - Eski key'i silin
   - Google AI Studio'dan yeni key alın
   - Tekrar deneyin

---

**İyi çalışmalar! 🎉**

*Son güncelleme: 28 Kasım 2025*

