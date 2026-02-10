        document.addEventListener('DOMContentLoaded', function () {

            // Global Değişken: Hangi satırı düzenliyoruz? (Null ise yeni ekleme modundayız)
            let editingRow = null;

            // Elementleri Seç
            const modalEl = document.getElementById('timelineModal');
            const modalTitle = modalEl.querySelector('.modal-title');
            const modal = new bootstrap.Modal(modalEl); // Bootstrap 5 Modal API

            const saveBtn = document.getElementById('saveTimelineBtn');
            const addNewBtn = document.getElementById('addNewBtn');
            const form = document.getElementById('timelineForm');
            const container = document.getElementById('timelineContainer');

            // Inputları Seç
            const inputType = document.getElementById('timelineType');
            const inputTitle = document.getElementById('timelineTitle');
            const inputCompany = document.getElementById('timelineCompany');
            const inputDate = document.getElementById('timelineDate');

            // ----------------------------------------------
            // 1. DİL EKLEME İŞLEMİ (MEVCUT)
            // ----------------------------------------------
            const langContainer = document.getElementById('langContainer');
            const addLangBtn = document.getElementById('addLangBtn');

            if (addLangBtn) { // Hata almamak için buton var mı kontrolü
                addLangBtn.addEventListener('click', function () {
                    const newRow = `
                <div class="row g-2 align-items-center mb-2 deletable-row">
                    <div class="col-5">
                        <input type="text" class="form-control form-control-sm" placeholder="Yeni Dil">
                    </div>
                    <div class="col-5">
                        <select class="form-select form-select-sm">
                            <option value="100">Native</option>
                            <option value="80">Advanced</option>
                            <option value="60">Intermediate</option>
                        </select>
                    </div>
                    <div class="col-2 text-end">
                        <button type="button" class="btn btn-sm btn-outline-danger border-0 delete-btn">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            `;
                    langContainer.insertAdjacentHTML('beforeend', newRow);
                });
            }

            // ---------------------------------------------------------
            // 1. MODALI SIFIRLAMA ("Yeni Ekle"ye basınca)
            // ---------------------------------------------------------
            addNewBtn.addEventListener('click', function () {
                editingRow = null; // Düzenleme modundan çık
                form.reset();      // Formu temizle
                modalTitle.innerText = "Yeni Kayıt Ekle"; // Başlığı düzelt
                saveBtn.innerText = "Listeye Ekle";       // Buton yazısını düzelt
            });

            // ---------------------------------------------------------
            // 2. DÜZENLEME MODUNA GEÇİŞ (Kalem butonuna basınca)
            // ---------------------------------------------------------
            document.body.addEventListener('click', function (e) {
                // Tıklanan element edit-btn mi?
                const editBtn = e.target.closest('.edit-btn');
                if (!editBtn) return;

                // Düzenlenecek satırı bul
                editingRow = editBtn.closest('.list-group-item');

                // Satırdaki verileri oku (HTML'den parse et)
                // Not: Gerçek projede verileri data-attribute olarak tutmak daha sağlıklıdır.
                const currentTitle = editingRow.querySelector('h5').innerText;
                const currentCompany = editingRow.querySelector('.text-muted').innerText.trim();
                const currentDate = editingRow.querySelector('small').innerText.trim();
                const currentBadge = editingRow.querySelector('.badge').innerText;

                // Verileri Modaldaki Inputlara Doldur
                inputTitle.value = currentTitle;
                inputCompany.value = currentCompany; // İkon metnini temizlemek gerekebilir ama şimdilik böyle
                inputDate.value = currentDate;

                // Selectbox'ı ayarla
                inputType.value = (currentBadge === 'Eğitim') ? 'Education' : 'Experience';

                // Modalı Düzenleme Modunda Aç
                modalTitle.innerText = "Kaydı Düzenle";
                saveBtn.innerText = "Değişiklikleri Kaydet";

                // Modalı Göster (Elle tetikliyoruz çünkü butonun data-bs-toggle'ı yok varsayıyoruz veya varsa da üstüne yazıyoruz)
                modal.show();
            });

            // ---------------------------------------------------------
            // 3. KAYDETME İŞLEMİ (Hem Ekleme Hem Güncelleme)
            // ---------------------------------------------------------
            saveBtn.addEventListener('click', function () {

                const type = inputType.value;
                const title = inputTitle.value;
                const company = inputCompany.value;
                const date = inputDate.value;

                if (title === "" || company === "") {
                    alert("Lütfen gerekli alanları doldurun.");
                    return;
                }

                let badgeClass = type === 'Experience' ? 'bg-primary' : 'bg-success';
                let badgeText = type === 'Experience' ? 'Deneyim' : 'Eğitim';
                let iconClass = type === 'Experience' ? 'bi-building' : 'bi-mortarboard';
                const rowContent = `
            <div class="d-flex justify-content-between align-items-start">
                <div>
                    <span class="badge ${badgeClass} mb-1">${badgeText}</span>
                    <h5 class="mb-1">${title}</h5>
                    <p class="mb-1 text-muted"><i class="bi ${iconClass}"></i> ${company}</p>
                    <small class="text-primary fw-bold"><i class="bi bi-calendar"></i> ${date}</small>
                </div>
                <div class="btn-group">
                    <button type="button" class="btn btn-sm btn-outline-secondary edit-btn"><i class="bi bi-pencil"></i></button>
                    <button type="button" class="btn btn-sm btn-outline-danger delete-btn"><i class="bi bi-trash"></i></button>
                </div>
            </div>
        `;

                if (editingRow) {
                    editingRow.innerHTML = rowContent;
                } else {
                    const newRowHtml = `<div class="list-group-item p-3 deletable-row">${rowContent}</div>`;
                    container.insertAdjacentHTML('afterbegin', newRowHtml); 
                }
                modal.hide();
                form.reset();
                editingRow = null;
            });
            document.body.addEventListener('click', function (e) {
                const deleteBtn = e.target.closest('.delete-btn');
                if (deleteBtn) {
                    if (confirm("Silmek istediğinize emin misiniz?")) {
                        const row = deleteBtn.closest('.list-group-item') || deleteBtn.closest('.deletable-row');
                        if (row) row.remove();
                    }
                }
            });

        });