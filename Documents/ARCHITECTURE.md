# StudyPlanner - Mimari Dokümantasyonu

Bu belge, StudyPlanner projesinin mimari yapısını, kullanılan tasarım desenlerini ve SOLID prensiplerini açıklar.

## 🏗️ Genel Mimari

Proje, **Clean Architecture** ve **MVVM (Model-View-ViewModel)** pattern'lerini takip eder.

```
┌─────────────────────────────────────────────┐
│              Presentation Layer             │
│         (Views + ViewModels)                │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│           Application Layer                 │
│      (Services + Business Logic)            │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│          Infrastructure Layer               │
│      (Repositories + Data Access)           │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│              Domain Layer                   │
│         (Models + Interfaces)               │
└─────────────────────────────────────────────┘
```

## 📂 Katmanlar

### 1. Domain Layer (Models + Interfaces)

**Sorumluluk**: İş mantığı kuralları ve entity tanımlamaları

```
Models/
├── BaseEntity.cs          # Tüm entity'ler için base class
└── StudyPlanItem.cs       # Çalışma planı entity

Interfaces/
├── IRepository.cs         # Generic repository contract
├── IDataService.cs        # Dosya işlemleri contract
└── IDialogService.cs      # UI dialog contract
```

**Özellikler**:
- ✅ Framework bağımsız
- ✅ Sadece business rules içerir
- ✅ Diğer katmanlara bağımlı değil

### 2. Infrastructure Layer (Repositories + Services)

**Sorumluluk**: Veri erişimi ve dış servis entegrasyonu

```
Repositories/
└── StudyPlanRepository.cs  # In-memory data storage

Services/
├── XmlDataService.cs       # XML serialization
└── DialogService.cs        # WPF dialogs
```

**Özellikler**:
- ✅ Domain layer'a bağımlı
- ✅ Interface'leri implement eder
- ✅ Concrete implementasyonlar

### 3. Application Layer (ViewModels)

**Sorumluluk**: Use case'ler ve application logic

```
ViewModels/
├── ViewModelBase.cs        # Base ViewModel
├── RelayCommand.cs         # Command pattern
└── MainViewModel.cs        # Ana ekran logic
```

**Özellikler**:
- ✅ MVVM pattern
- ✅ Data binding
- ✅ Command pattern
- ✅ INotifyPropertyChanged

### 4. Presentation Layer (Views)

**Sorumluluk**: UI ve kullanıcı etkileşimi

```
Views/
├── MainWindow.xaml         # UI definition
└── MainWindow.xaml.cs      # Code-behind (minimal)
```

**Özellikler**:
- ✅ XAML tabanlı
- ✅ Minimal code-behind
- ✅ Data binding
- ✅ DevExpress ready

## 🎯 SOLID Prensipleri

### Single Responsibility Principle (SRP)

Her sınıf tek bir sorumluluğa sahip:

```csharp
// ✅ İyi: Sadece veri tutma sorumluluğu
public class StudyPlanItem : BaseEntity
{
    public string Subject { get; set; }
    public DateTime Date { get; set; }
    // ...
}

// ✅ İyi: Sadece veri erişim sorumluluğu
public class StudyPlanRepository : IRepository<StudyPlanItem>
{
    public async Task<StudyPlanItem> GetByIdAsync(Guid id) { }
    // ...
}

// ✅ İyi: Sadece UI logic sorumluluğu
public class MainViewModel : ViewModelBase
{
    public ICommand AddCommand { get; }
    // ...
}
```

### Open/Closed Principle (OCP)

Genişletmeye açık, değişikliğe kapalı:

```csharp
// ✅ Base class - extension point
public abstract class ViewModelBase : INotifyPropertyChanged
{
    protected virtual void OnPropertyChanged(string propertyName) { }
    // ...
}

// ✅ Yeni ViewModel eklemek için base'i extend et
public class MainViewModel : ViewModelBase
{
    // Yeni özellikler ekle, base'i değiştirme
}
```

### Liskov Substitution Principle (LSP)

Derived class, base class yerine kullanılabilir:

```csharp
// ✅ Liskov'u takip eder
IRepository<StudyPlanItem> repository = new StudyPlanRepository();

// Repository interface'i her zaman StudyPlanRepository yerine kullanılabilir
```

### Interface Segregation Principle (ISP)

Küçük ve spesifik interface'ler:

```csharp
// ✅ İyi: Küçük, odaklanmış interface
public interface IDialogService
{
    void ShowMessage(string message);
    bool ShowConfirmation(string message);
}

// ❌ Kötü: Çok büyük interface
public interface IMegaService
{
    void ShowMessage();
    void SaveFile();
    void LoadFile();
    void SendEmail();
    // ... 50+ method
}
```

### Dependency Inversion Principle (DIP)

Üst seviye modüller, interface'lere bağımlı:

```csharp
// ✅ İyi: Interface'e bağımlı
public class MainViewModel
{
    private readonly IRepository<StudyPlanItem> _repository;
    private readonly IDataService<List<StudyPlanItem>> _dataService;
    
    public MainViewModel(
        IRepository<StudyPlanItem> repository,
        IDataService<List<StudyPlanItem>> dataService)
    {
        _repository = repository;
        _dataService = dataService;
    }
}

// ❌ Kötü: Concrete class'a bağımlı
public class MainViewModel
{
    private readonly StudyPlanRepository _repository;  // Tight coupling!
}
```

## 🔄 Tasarım Desenleri

### 1. Repository Pattern

**Amaç**: Veri erişim katmanını soyutlama

```csharp
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
}
```

**Faydalar**:
- Veri kaynağı değişikliklerine karşı esnek
- Test edilebilir (mock edilebilir)
- Centralized data access logic

### 2. MVVM Pattern

**Katmanlar**:
```
View (XAML) ←→ ViewModel ←→ Model
```

**Data Flow**:
```
User Action → View → Command → ViewModel → Repository → Data
Data → Repository → ViewModel → INotifyPropertyChanged → View
```

### 3. Dependency Injection Pattern

**Container Setup** (App.xaml.cs):

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Interfaces → Implementations
    services.AddSingleton<IRepository<StudyPlanItem>, StudyPlanRepository>();
    services.AddSingleton<IDataService<List<StudyPlanItem>>, XmlDataService>();
    services.AddTransient<MainViewModel>();
    services.AddTransient<MainWindow>();
}
```

### 4. Command Pattern

**Implementation**:

```csharp
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
    
    public void Execute(object parameter) => _execute(parameter);
    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
}
```

### 5. Observer Pattern

**INotifyPropertyChanged Implementation**:

```csharp
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## 📊 Data Flow

### Create Operation

```
1. User fills form → CurrentItem (ViewModel)
2. User clicks "Add" button
3. AddCommand.Execute() → ViewModel
4. Validation (ViewModel)
5. _repository.AddAsync() → Repository
6. StudyPlans.Add() → ObservableCollection
7. View updates automatically (INotifyPropertyChanged)
```

### Read Operation

```
1. Application starts
2. LoadDataAsync() → ViewModel
3. _dataService.LoadAsync() → XmlDataService
4. XML Deserialization
5. LoadItemsToRepository() → ViewModel
6. Repository.AddAsync() → Repository
7. StudyPlans collection updated
8. View reflects data (Data Binding)
```

### Update Operation

```
1. User selects item → SelectedItem (ViewModel)
2. User modifies → CurrentItem updates
3. UpdateCommand.Execute()
4. _repository.UpdateAsync() → Repository
5. INotifyPropertyChanged triggers
6. View updates
```

### Delete Operation

```
1. User selects item → SelectedItem
2. User clicks "Delete" button
3. Confirmation dialog → DialogService
4. DeleteCommand.Execute()
5. _repository.DeleteAsync() → Repository
6. StudyPlans.Remove()
7. View updates
```

## 🧪 Testing Strategy

### Unit Tests

```csharp
[TestClass]
public class StudyPlanRepositoryTests
{
    [TestMethod]
    public async Task AddAsync_ShouldAddItem()
    {
        // Arrange
        var repository = new StudyPlanRepository();
        var item = new StudyPlanItem { Subject = "Test" };
        
        // Act
        await repository.AddAsync(item);
        var result = await repository.GetByIdAsync(item.Id);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Subject);
    }
}
```

### Integration Tests

```csharp
[TestClass]
public class XmlDataServiceTests
{
    [TestMethod]
    public async Task SaveAndLoad_ShouldPersistData()
    {
        // Arrange
        var service = new XmlDataService();
        var items = new List<StudyPlanItem> { new StudyPlanItem() };
        
        // Act
        await service.SaveAsync(items);
        var loaded = await service.LoadAsync();
        
        // Assert
        Assert.AreEqual(items.Count, loaded.Count);
    }
}
```

### ViewModel Tests (with Mocking)

```csharp
[TestClass]
public class MainViewModelTests
{
    [TestMethod]
    public async Task AddCommand_WithValidData_ShouldAddItem()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<StudyPlanItem>>();
        var mockDataService = new Mock<IDataService<List<StudyPlanItem>>>();
        var mockDialogService = new Mock<IDialogService>();
        
        var viewModel = new MainViewModel(
            mockRepo.Object, 
            mockDataService.Object, 
            mockDialogService.Object);
        
        viewModel.CurrentItem = new StudyPlanItem { Subject = "Test" };
        
        // Act
        viewModel.AddCommand.Execute(null);
        
        // Assert
        mockRepo.Verify(x => x.AddAsync(It.IsAny<StudyPlanItem>()), Times.Once);
    }
}
```

## 🚀 Extension Points

Proje aşağıdaki alanlarda kolayca genişletilebilir:

### 1. Yeni Repository Implementasyonu

```csharp
// Database repository
public class DatabaseStudyPlanRepository : IRepository<StudyPlanItem>
{
    private readonly DbContext _context;
    
    public DatabaseStudyPlanRepository(DbContext context)
    {
        _context = context;
    }
    
    // Implement interface methods with EF Core
}

// Dependency Injection'da değiştir:
services.AddSingleton<IRepository<StudyPlanItem>, DatabaseStudyPlanRepository>();
```

### 2. Yeni Data Service

```csharp
// JSON data service
public class JsonDataService : IDataService<List<StudyPlanItem>>
{
    public async Task<List<StudyPlanItem>> LoadAsync()
    {
        // JSON deserialization
    }
    
    public async Task<bool> SaveAsync(List<StudyPlanItem> data)
    {
        // JSON serialization
    }
}
```

### 3. Yeni ViewModel

```csharp
public class StatisticsViewModel : ViewModelBase
{
    private readonly IRepository<StudyPlanItem> _repository;
    
    public StatisticsViewModel(IRepository<StudyPlanItem> repository)
    {
        _repository = repository;
    }
    
    // Statistics logic
}
```

### 4. Yeni View

```xml
<Window x:Class="StudyPlanner.Views.StatisticsWindow">
    <!-- Statistics UI -->
</Window>
```

## 📚 Kaynaklar

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [MVVM Pattern - Microsoft](https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

**Son Güncelleme**: 2024
**Versiyon**: 2.0.0



