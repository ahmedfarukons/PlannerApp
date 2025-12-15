# 🚀 GitHub'a Push - Hızlı Rehber

## 1️⃣ GitHub'da Yeni Repository Oluştur

1. https://github.com/new adresine git
2. Repository adı: **`StudyPlanner-PaperBold`**
3. Description: `WPF Study Planner with AI-powered document analysis (Paper Bold integration)`
4. **Public** veya **Private** seç
5. ⚠️ **Initialize repository** seçeneklerini BOŞLUK BIRAK (README, .gitignore, license ekleme)
6. **Create repository** tıkla

## 2️⃣ Terminal'de Komutları Çalıştır

GitHub sana komutlar gösterecek. Şunları kullan:

```bash
cd "C:\Users\LENOVO\Documents\Visual Studio 2022\My Codes\StudyPlanner"

# Remote ekle (GITHUB_USERNAME yerine kendi kullanıcı adını yaz)
git remote add origin https://github.com/GITHUB_USERNAME/StudyPlanner-PaperBold.git

# Branch adını main yap
git branch -M main

# Push et
git push -u origin main
```

## 3️⃣ Veya Ben Push Edeyim

Aşağıdaki bilgileri ver:
- GitHub kullanıcı adın: `________`
- Repository adı: `________`

Ben komutları çalıştırayım!

---

## ✅ Local Commit Başarılı

```
[master c416918] Initial commit: Paper Bold AI integrated with StudyPlanner
 59 files changed, 7749 insertions(+)
```

Şu dosyalar commit edildi:
- ✅ Tüm kaynak kodlar
- ✅ Paper Bold entegrasyonu
- ✅ Konfigürasyon dosyaları
- ✅ Dokümantasyon
- ⚠️ `appsettings.Development.json` GİTTE DEĞİL (.gitignore ile korumalı)

---

## 🔐 Güvenlik Notu

API key'iniz güvende! `.gitignore` sayesinde `appsettings.Development.json` GitHub'a gitmedi.

**Not:** Sunum öncesi yeni bir commit yaparsak:
```bash
git add .
git commit -m "Yeni özellik: Kategori bazlı PDF yönetimi"
git push
```

