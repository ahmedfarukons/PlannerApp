# DevExpress Kurulum ve Entegrasyon Rehberi

Bu belge, StudyPlanner projesine DevExpress WPF kontrollerinin nasıl entegre edileceğini açıklar.

## 📋 Gereksinimler

1. DevExpress Universal Subscription veya WPF Subscription
2. DevExpress hesabı ve lisans anahtarı
3. Visual Studio 2022
4. .NET 6.0 SDK

## 🚀 Adım 1: DevExpress Kurulumu

### NuGet Paketlerini Yükleme

1. Visual Studio'da projeyi açın
2. **Tools > NuGet Package Manager > Manage NuGet Packages for Solution**
3. **Browse** sekmesinde DevExpress paketlerini arayın
4. Aşağıdaki paketleri yükleyin:

```xml
<PackageReference Include="DevExpress.Wpf.Core" Version="23.2.3" />
<PackageReference Include="DevExpress.Wpf.Grid" Version="23.2.3" />
<PackageReference Include="DevExpress.Wpf.Themes.All" Version="23.2.3" />
<PackageReference Include="DevExpress.Wpf.Controls" Version="23.2.3" />
```

### Alternatif: Package Manager Console

```powershell
Install-Package DevExpress.Wpf.Core
Install-Package DevExpress.Wpf.Grid
Install-Package DevExpress.Wpf.Themes.All
Install-Package DevExpress.Wpf.Controls
```

## 🎨 Adım 2: Tema Ekleme

### App.xaml'i Güncelleme

`App.xaml` dosyasını açın ve aşağıdaki değişiklikleri yapın:

```xml
<Application x:Class="StudyPlanner.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:dxc="http://schemas.devexpress.com/winfx/2008/xaml/core"
             Startup="Application_Startup">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- DevExpress Themes -->
                <ResourceDictionary Source="pack://application:,,,/DevExpress.Xpf.Themes.Office2019Colorful.v23.2;component/Themes/Generic.xaml"/>
                
                <!-- Alternatif Temalar:
                Office2019White: Beyaz tema
                Office2019Black: Siyah tema
                Office2019HighContrast: Yüksek kontrast
                VS2019Light: Visual Studio açık tema
                VS2019Dark: Visual Studio koyu tema
                -->
            </ResourceDictionary.MergedDictionaries>
            
            <!-- Diğer kaynaklar -->
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## 📝 Adım 3: MainWindow'u DevExpress ile Güncelleme

### Namespace Ekleme

`Views/MainWindow.xaml` dosyasında namespace'leri ekleyin:

```xml
<Window x:Class="StudyPlanner.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
        xmlns:models="clr-namespace:StudyPlanner.Models"
        xmlns:helpers="clr-namespace:StudyPlanner.Helpers">
```

### DataGrid'i GridControl ile Değiştirme

Mevcut DataGrid yerine:

```xml
<!-- Eski DataGrid -->
<DataGrid ItemsSource="{Binding StudyPlans}"
          SelectedItem="{Binding SelectedItem}"
          ...>
</DataGrid>
```

DevExpress GridControl kullanın:

```xml
<!-- DevExpress GridControl -->
<dxg:GridControl ItemsSource="{Binding StudyPlans}"
                 SelectedItem="{Binding SelectedItem}"
                 AutoGenerateColumns="None">
    <dxg:GridControl.View>
        <dxg:TableView AllowEditing="False"
                      ShowGroupPanel="False"
                      NavigationStyle="Row"
                      AutoWidth="True"/>
    </dxg:GridControl.View>
    
    <dxg:GridControl.Columns>
        <dxg:GridColumn FieldName="IsCompleted" 
                       Header="✓" 
                       Width="50">
            <dxg:GridColumn.CellTemplate>
                <DataTemplate>
                    <CheckBox IsChecked="{Binding Row.IsCompleted, Mode=TwoWay}"
                             HorizontalAlignment="Center"/>
                </DataTemplate>
            </dxg:GridColumn.CellTemplate>
        </dxg:GridColumn>
        
        <dxg:GridColumn FieldName="Subject" 
                       Header="Ders/Konu"/>
        
        <dxg:GridColumn FieldName="Category" 
                       Header="Kategori" 
                       Width="100"/>
        
        <dxg:GridColumn FieldName="DateDisplay" 
                       Header="Tarih" 
                       Width="150"/>
        
        <dxg:GridColumn FieldName="DurationDisplay" 
                       Header="Süre" 
                       Width="100"/>
        
        <dxg:GridColumn FieldName="Priority" 
                       Header="Öncelik" 
                       Width="80"/>
    </dxg:GridControl.Columns>
</dxg:GridControl>
```

### TextBox'ları DevExpress TextEdit ile Değiştirme

```xml
<!-- Eski TextBox -->
<TextBox Text="{Binding CurrentItem.Subject}"/>

<!-- DevExpress TextEdit -->
<dxe:TextEdit EditValue="{Binding CurrentItem.Subject, UpdateSourceTrigger=PropertyChanged}"/>
```

### ComboBox'ı DevExpress ComboBoxEdit ile Değiştirme

```xml
<!-- DevExpress ComboBoxEdit -->
<dxe:ComboBoxEdit EditValue="{Binding CurrentItem.Priority}"
                  ItemsSource="{Binding Source={x:Static models:PriorityLevel}, 
                                        Converter={helpers:EnumToCollectionConverter}}"/>
```

### DatePicker'ı DevExpress DateEdit ile Değiştirme

```xml
<!-- DevExpress DateEdit -->
<dxe:DateEdit EditValue="{Binding CurrentItem.Date}"
              Mask="dd.MM.yyyy HH:mm"
              MaskType="DateTime"/>
```

### Button'ları DevExpress SimpleButton ile Değiştirme

```xml
<!-- DevExpress SimpleButton -->
<dx:SimpleButton Content="➕ Ekle"
                Command="{Binding AddCommand}"
                Glyph="{dx:DXImage Image=Add_16x16.png}"
                Width="120"
                Height="35"/>
```

## 🎨 Adım 4: Gelişmiş Özellikler

### Ribbon UI Ekleme

```xml
<dxr:RibbonControl>
    <dxr:RibbonDefaultPageCategory>
        <dxr:RibbonPage Caption="Ana Sayfa">
            <dxr:RibbonPageGroup Caption="Dosya">
                <dxb:BarButtonItem Content="Kaydet" 
                                  LargeGlyph="{dx:DXImage Image=Save_32x32.png}"
                                  Command="{Binding SaveCommand}"/>
                <dxb:BarButtonItem Content="Yükle" 
                                  LargeGlyph="{dx:DXImage Image=Open_32x32.png}"
                                  Command="{Binding LoadCommand}"/>
            </dxr:RibbonPageGroup>
            
            <dxr:RibbonPageGroup Caption="Düzenle">
                <dxb:BarButtonItem Content="Ekle" 
                                  LargeGlyph="{dx:DXImage Image=Add_32x32.png}"
                                  Command="{Binding AddCommand}"/>
                <dxb:BarButtonItem Content="Sil" 
                                  LargeGlyph="{dx:DXImage Image=Delete_32x32.png}"
                                  Command="{Binding DeleteCommand}"/>
            </dxr:RibbonPageGroup>
        </dxr:RibbonPage>
    </dxr:RibbonDefaultPageCategory>
</dxr:RibbonControl>
```

### Kartlar için TileControl

```xml
<dxlc:TileLayoutControl ItemsSource="{Binding StudyPlans}">
    <dxlc:TileLayoutControl.ItemTemplate>
        <DataTemplate>
            <dxlc:Tile Header="{Binding Subject}">
                <StackPanel>
                    <TextBlock Text="{Binding DateDisplay}"/>
                    <TextBlock Text="{Binding DurationDisplay}"/>
                    <TextBlock Text="{Binding Notes}"/>
                </StackPanel>
            </dxlc:Tile>
        </DataTemplate>
    </dxlc:TileLayoutControl.ItemTemplate>
</dxlc:TileLayoutControl>
```

### Animasyonlu Dialog'lar

```xml
<dx:ThemedWindow ...>
    <!-- Window content -->
</dx:ThemedWindow>
```

DialogService'de:

```csharp
DXMessageBox.Show("Mesaj", "Başlık", MessageBoxButton.OK, MessageBoxImage.Information);
```

## 📚 Faydalı Kaynaklar

- [DevExpress WPF Documentation](https://docs.devexpress.com/WPF/6178/wpf-controls)
- [DevExpress Examples](https://github.com/DevExpress-Examples)
- [DevExpress Support Center](https://supportcenter.devexpress.com/)
- [DevExpress YouTube Channel](https://www.youtube.com/user/DevExpressInc)

## 🔧 Sorun Giderme

### Lisans Hatası

Eğer lisans hatası alıyorsanız:

1. DevExpress License Manager'ı açın
2. Lisans anahtarınızı girin
3. Projeyi temizleyin ve yeniden derleyin

### Tema Görünmüyor

1. `App.xaml`'de tema yolunu kontrol edin
2. DevExpress paket sürümünü kontrol edin
3. Clean Solution > Rebuild Solution yapın

### GridControl Veri Göstermiyor

1. `AutoGenerateColumns` özelliğini kontrol edin
2. `ItemsSource` binding'ini kontrol edin
3. ViewModel'de `StudyPlans` ObservableCollection olduğundan emin olun

## 💡 İpuçları

1. **Theme Switcher**: Kullanıcının tema değiştirmesine izin verin
2. **Localization**: DevExpress çoklu dil desteği sunar
3. **Export**: GridControl Excel/PDF export özelliği vardır
4. **Validation**: DevExpress validation framework'ü kullanın
5. **Touch Support**: DevExpress touch-friendly kontroller sunar

---

**Not**: Bu rehber DevExpress v23.2 için hazırlanmıştır. Farklı sürümler için dokümantasyonu kontrol edin.



