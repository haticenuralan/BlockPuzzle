# Technical Debt / Known Limitations

## Grid & Game Logic
- `HasEmptyCell()` şu an sadece grid'de en az bir boş hücre olup olmadığını kontrol ediyor. Bu, yalnızca 1x1 (tek kare) bloklar için doğru bir Game Over kontrolü sağlıyor. Çok hücreli şekiller (L-şekli, çizgi vb.) eklendiğinde, bu fonksiyon "elimdeki hiçbir şekil hiçbir yere sığmıyor mu?" şeklinde genişletilmeli.

## DraggableBlock
- `DraggableBlock` şu an hem sürükleme davranışını hem de grid'e snap etme mantığını içeriyor. İleride bu ikisi ayrı sınıflara (örneğin `DragHandler` ve `GridSnapper`) bölünebilir, tek sorumluluk ilkesine daha uygun olur.

## Object Pooling
- Bloklar her yerleştirildiğinde/temizlendiğinde `Instantiate`/`Destroy` ile oluşturuluyor/yok ediliyor. Sprint 3'te Object Pool sistemine geçilecek, performans ve iyi pratik açısından.

## Ses ve Animasyon
- Şu an hiç ses efekti veya animasyon yok. Sprint 3-4'te eklenecek.

## Test Kodu
- `DebugGameOverTrigger.cs` dosyası, Game Over akışını test etmek için eklendi. Gerçek oyun mantığının bir parçası değil, geliştirme sürecinde kullanılan geçici bir araç.
