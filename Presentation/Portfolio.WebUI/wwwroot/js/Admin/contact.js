document.addEventListener('DOMContentLoaded', function() {
    
    // --- 1. MODAL AÇILIRKEN VERİLERİ DOLDURMA ---
    const messageModal = document.getElementById('messageModal');
    
    messageModal.addEventListener('show.bs.modal', function (event) {
        // Modalı tetikleyen satırı (tr) bul
        const button = event.relatedTarget; 
        
        // Satırın üzerindeki verileri (data-*) çek
        const sender = button.getAttribute('data-sender');
        const email = button.getAttribute('data-email');
        const subject = button.getAttribute('data-subject');
        const message = button.getAttribute('data-message');
        const date = button.getAttribute('data-date');

        // Modal içindeki alanlara yaz
        document.getElementById('m-sender').textContent = sender;
        document.getElementById('m-email').textContent = email;
        document.getElementById('m-subject').textContent = subject;
        document.getElementById('m-date').textContent = date;
        document.getElementById('m-message').textContent = message;
        document.getElementById('m-reply-btn').href = `mailto:${email}?subject=Re: ${subject}`;

        // Görsel olarak "Okundu" yap (Backend'e istek atmak buraya eklenebilir)
        if (button.classList.contains('unread')) {
            button.classList.remove('unread');
            
            // Zarf ikonunu açılmış zarf yap
            const icon = button.querySelector('.icon-status');
            if(icon) {
                icon.classList.remove('bi-envelope-fill', 'text-primary');
                icon.classList.add('bi-envelope-open', 'text-muted');
            }
            updateCounts();
        }
    });

    // --- 2. SİLME İŞLEMİ ---
    // Tablodaki tüm sil butonlarını dinle
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.stopPropagation(); // Satıra tıklanmış gibi modal açılmasın
            
            if(confirm('Bu mesajı silmek istediğinize emin misiniz?')) {
                this.closest('tr').remove(); // Satırı HTML'den sil
                updateCounts(); // Sayıları güncelle
            }
        });
    });

    // --- 3. SAYAÇLARI GÜNCELLEME ---
    function updateCounts() {
        const total = document.querySelectorAll('.message-row').length;
        const unread = document.querySelectorAll('.message-row.unread').length;
        
        document.getElementById('totalMsgCount').innerText = total;
        
        const badge = document.getElementById('unreadBadge');
        badge.innerText = unread;
        badge.style.display = unread > 0 ? 'inline-block' : 'none';
    }
    
    // Sayfa yüklenince sayıları bir kere hesapla
    updateCounts();
});