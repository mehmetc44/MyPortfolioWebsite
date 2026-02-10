document.addEventListener('DOMContentLoaded', function() {

        // --- ELEMANLAR ---
        const tableBody = document.getElementById('testimonialTableBody');
        const modalEl = document.getElementById('testimonialModal');
        const modal = new bootstrap.Modal(modalEl);
        
        const addNewBtn = document.getElementById('addNewBtn');
        const saveBtn = document.getElementById('saveBtn');
        const testimonialForm = document.getElementById('testimonialForm');

        // Inputs
        const tName = document.getElementById('tName');
        const tTitle = document.getElementById('tTitle');
        const tRating = document.getElementById('tRating'); // Gizli input
        const tImage = document.getElementById('tImage');
        const avatarPreview = document.getElementById('avatarPreview');

        // Yorumlar
        const tCommentTr = document.getElementById('tCommentTr');
        const tCommentEn = document.getElementById('tCommentEn');
        const tCommentDe = document.getElementById('tCommentDe');

        // Yıldızlar
        const stars = document.querySelectorAll('#starContainer i');

        // State
        let editingRow = null;
        let currentRating = 5; // Varsayılan puan
        const defaultAvatarHtml = '<div class="text-muted small"><i class="bi bi-person-bounding-box fs-1"></i><br>Foto Seç</div>';


        // --- 1. YILDIZ OYLAMA MANTIĞI ---
        stars.forEach(star => {
            star.addEventListener('click', function() {
                const val = parseInt(this.getAttribute('data-value'));
                setRating(val);
            });
        });

        function setRating(val) {
            currentRating = val;
            tRating.value = val;
            
            // Görseli güncelle
            stars.forEach(star => {
                const starVal = parseInt(star.getAttribute('data-value'));
                if (starVal <= val) {
                    star.classList.add('active'); // Sarı yap
                    star.classList.remove('bi-star');
                    star.classList.add('bi-star-fill');
                } else {
                    star.classList.remove('active'); // Gri yap
                    star.classList.remove('bi-star-fill');
                    star.classList.add('bi-star');
                }
            });
        }


        // --- 2. RESİM ÖNİZLEME ---
        tImage.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    avatarPreview.innerHTML = `<img src="${e.target.result}">`;
                }
                reader.readAsDataURL(file);
            }
        });


        // --- 3. YENİ EKLE BUTONU ---
        addNewBtn.addEventListener('click', function() {
            editingRow = null;
            testimonialForm.reset();
            avatarPreview.innerHTML = defaultAvatarHtml;
            setRating(5); // Puanı 5'e sıfırla
            modalEl.querySelector('.modal-title').innerText = "Referans Ekle";
            modal.show();
        });


        // --- 4. DÜZENLEME MODU ---
        tableBody.addEventListener('click', function(e) {
            const editBtn = e.target.closest('.edit-btn');
            if (editBtn) {
                editingRow = editBtn.closest('tr');

                const imgEl = editingRow.querySelector('.table-avatar');
                const imgUrl = imgEl ? imgEl.src : '';
                const name = editingRow.querySelector('.t-name').innerText;
                const title = editingRow.querySelector('.t-title').innerText;
                
                // Formu Doldur
                tName.value = name;
                tTitle.value = title;
                
                // Resim
                avatarPreview.innerHTML = `<img src="${imgUrl}">`;

                // Puanı Bul
                const ratingCount = editingRow.querySelectorAll('.bi-star-fill.text-warning').length;
                setRating(ratingCount);

                modalEl.querySelector('.modal-title').innerText = "Referans Düzenle";
                modal.show();
            }
        });


        // --- 5. KAYDETME ---
        saveBtn.addEventListener('click', function() {
            if (tName.value.trim() === "") { alert("İsim zorunludur!"); return; }

            const name = tName.value;
            const title = tTitle.value;
            const comment = tCommentTr.value || "Yorum yok..."; 
            
            let imgSrc = "img/1.jpeg";
            if (avatarPreview.querySelector('img')) {
                imgSrc = avatarPreview.querySelector('img').src;
            }

            let starsHtml = '';
            for(let i=1; i<=5; i++) {
                if(i <= currentRating) {
                    starsHtml += '<i class="bi bi-star-fill text-warning"></i> ';
                } else {
                    starsHtml += '<i class="bi bi-star text-muted"></i> ';
                }
            }

            const rowContent = `
                <td class="ps-4">
                    <img src="${imgSrc}" class="table-avatar" alt="User">
                </td>
                <td>
                    <div class="fw-bold t-name">${name}</div>
                    <div class="small text-muted t-title">${title}</div>
                </td>
                <td>
                    <div class="small">${starsHtml}</div>
                </td>
                <td>
                    <div class="text-muted small text-truncate" style="max-width: 250px;">
                        ${comment}
                    </div>
                </td>
                <td class="text-end pe-4">
                    <div class="btn-group">
                        <button type="button" class="btn btn-sm btn-outline-secondary edit-btn"><i class="bi bi-pencil"></i></button>
                        <button type="button" class="btn btn-sm btn-outline-danger delete-btn"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            `;

            if (editingRow) {
                editingRow.innerHTML = rowContent;
            } else {
                const newRow = `<tr>${rowContent}</tr>`;
                tableBody.insertAdjacentHTML('afterbegin', newRow);
            }

            modal.hide();
        });


        // --- 6. SİLME ---
        tableBody.addEventListener('click', function(e) {
            const deleteBtn = e.target.closest('.delete-btn');
            if (deleteBtn && confirm("Bu yorumu silmek istiyor musunuz?")) {
                deleteBtn.closest('tr').remove();
            }
        });

    });