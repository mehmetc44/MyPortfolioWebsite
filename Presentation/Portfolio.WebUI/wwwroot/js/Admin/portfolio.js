document.addEventListener('DOMContentLoaded', function () {

            // --- 1. ELEMANLAR VE DEĞİŞKENLER ---
            const tableBody = document.getElementById('projectsTableBody');
            const modalEl = document.getElementById('projectModal');
            const modal = new bootstrap.Modal(modalEl);

            const projectForm = document.getElementById('projectForm');
            const addNewBtn = document.getElementById('addNewProjectBtn');
            const saveBtn = document.getElementById('saveProjectBtn');

            // Form Inputları
            const pTitle = document.getElementById('pTitle');
            const pCategory = document.getElementById('pCategory');
            const pClient = document.getElementById('pClient');
            const pDate = document.getElementById('pDate');
            const pUrl = document.getElementById('pUrl');

            // Görsel Yönetimi
            const imagesInput = document.getElementById('projectImagesInput');
            const galleryContainer = document.getElementById('galleryContainer');
            const mainCoverPreview = document.getElementById('mainCoverPreview');

            // Durum (State)
            let editingRow = null;     
            let projectFiles = [];     // { source: File|String, isFile: boolean }
            let coverIndex = 0;        

            // Varsayılan Demo Resimleri
            const defaultImages = [
                "img/slider-3.jpeg",
                "img/slider-2.jpeg"
            ];


            // --- 2. RESİM YÜKLEME ---
            if (imagesInput) {
                imagesInput.addEventListener('change', function (e) {
                    const newFiles = Array.from(e.target.files);
                    
                    newFiles.forEach(file => {
                        if (file.type.match('image.*')) {
                            projectFiles.push({ source: file, isFile: true });
                        }
                    });

                    // Hiç resim yokken yenisi eklendiyse, ilki kapak olsun
                    if (projectFiles.length > 0 && projectFiles.length === newFiles.length) {
                        coverIndex = 0;
                    }

                    renderGallery();
                    imagesInput.value = ''; 
                });
            }


            // --- 3. GALERİYİ ÇİZME ---
            function renderGallery() {
                galleryContainer.innerHTML = '';
                
                // Güvenlik kontrolü: coverIndex sınır dışına çıkmasın
                if (coverIndex >= projectFiles.length) coverIndex = 0;

                if (projectFiles.length === 0) {
                    galleryContainer.innerHTML = '<small class="text-muted fst-italic w-100 text-center mt-2">Henüz görsel eklenmedi.</small>';
                    mainCoverPreview.innerHTML = '<div class="d-flex align-items-center justify-content-center h-100 text-muted bg-light"><i class="bi bi-image display-4"></i></div>';
                    return;
                }

                projectFiles.forEach((item, index) => {
                    let src = '';
                    if (item.isFile) {
                        src = URL.createObjectURL(item.source);
                    } else {
                        src = item.source; // String URL
                    }

                    const isCover = (index === coverIndex);

                    // Büyük önizlemeyi güncelle
                    if (isCover) {
                        mainCoverPreview.innerHTML = `<img src="${src}" style="width:100%; height:100%; object-fit:cover;">`;
                    }

                    const itemHtml = `
                        <div class="gallery-item ${isCover ? 'is-cover' : ''}">
                            <div class="cover-badge">KAPAK</div>
                            <img src="${src}">
                            <div class="overlay">
                                <button type="button" class="gallery-btn btn-make-cover" onclick="setCover(${index})" title="Kapak Yap">
                                    <i class="bi bi-star-fill"></i>
                                </button>
                                <button type="button" class="gallery-btn btn-delete-img" onclick="deleteImage(${index})" title="Sil">
                                    <i class="bi bi-trash-fill"></i>
                                </button>
                            </div>
                        </div>
                    `;
                    galleryContainer.insertAdjacentHTML('beforeend', itemHtml);
                });
            }


            // --- 4. GLOBAL FONKSİYONLAR ---
            window.setCover = function (index) {
                coverIndex = index;
                renderGallery();
            };

            window.deleteImage = function (index) {
                projectFiles.splice(index, 1);
                
                // Kapak silindiyse index'i düzelt
                if (index === coverIndex) {
                    coverIndex = 0; 
                } else if (index < coverIndex) {
                    coverIndex--; 
                }
                renderGallery();
            };


            // --- 5. "YENİ EKLE" BUTONU ---
            addNewBtn.addEventListener('click', function () {
                editingRow = null;
                projectForm.reset();
                
                // Demo için varsayılanları yükle
                projectFiles = defaultImages.map(url => ({ source: url, isFile: false }));
                coverIndex = 0;
                
                renderGallery();
                modalEl.querySelector('.modal-title').innerText = 'Yeni Proje Ekle';
            });


            // --- 6. DÜZENLEME MODU ---
            tableBody.addEventListener('click', function (e) {
                const editBtn = e.target.closest('.edit-project-btn');
                if (editBtn) {
                    editingRow = editBtn.closest('tr');

                    const title = editingRow.querySelector('.title-cell').innerText;
                    const client = editingRow.querySelectorAll('td')[3].innerText;
                    const date = editingRow.querySelectorAll('td')[4].innerText;
                    const category = editingRow.querySelector('.badge').innerText;
                    const imgUrl = editingRow.querySelector('img.table-img').src;

                    // Formu doldur
                    pTitle.value = title;
                    pClient.value = client;
                    pDate.value = date;
                    
                    // Select değerini bulmaya çalış
                    // (Eğer listedeki değer select options içinde yoksa boş gelebilir)
                    const options = Array.from(pCategory.options);
                    const optionToSelect = options.find(item => item.text === category || item.value === category);
                    if(optionToSelect) pCategory.value = optionToSelect.value;

                    // Galeri: Kapak resmini başa ekle, varsayılanları arkasına ekle
                    projectFiles = [
                        { source: imgUrl, isFile: false }, 
                        ...defaultImages.map(url => ({ source: url, isFile: false }))
                    ];
                    coverIndex = 0;

                    renderGallery();
                    modalEl.querySelector('.modal-title').innerText = 'Projeyi Düzenle';
                    modal.show();
                }
            });


            // --- 7. KAYDETME ---
            saveBtn.addEventListener('click', function () {
                // Basit Validasyon
                if (pTitle.value.trim() === "") { 
                    alert("Proje başlığı zorunludur!"); 
                    return; 
                }

                const title = pTitle.value;
                // Kategori seçilmediyse 'Genel' yap
                const category = pCategory.value || "Genel";
                const client = pClient.value || "-";
                const date = pDate.value || new Date().toISOString().split('T')[0];
                const url = pUrl.value || "#";

                // Kapak Resmi Belirle
                let imgSrc = "https://via.placeholder.com/100";
                if (projectFiles.length > 0 && projectFiles[coverIndex]) {
                    const item = projectFiles[coverIndex];
                    if (item.isFile) {
                        imgSrc = URL.createObjectURL(item.source);
                    } else {
                        imgSrc = item.source;
                    }
                }

                const rowContent = `
                    <td class="ps-4">
                        <img src="${imgSrc}" class="table-img" alt="Proje" style="width:60px; height:60px; object-fit:cover; border-radius:6px;">
                    </td>
                    <td>
                        <h6 class="mb-0 fw-bold title-cell">${title}</h6>
                        <small class="text-muted"><i class="bi bi-link-45deg"></i> ${url}</small>
                    </td>
                    <td><span class="badge bg-primary bg-opacity-10 text-primary px-3">${category}</span></td>
                    <td>${client}</td>
                    <td>${date}</td>
                    <td class="text-end pe-4">
                        <div class="btn-group">
                            <button type="button" class="btn btn-sm btn-outline-secondary edit-project-btn"><i class="bi bi-pencil"></i></button>
                            <button type="button" class="btn btn-sm btn-outline-danger delete-btn"><i class="bi bi-trash"></i></button>
                        </div>
                    </td>
                `;

                if (editingRow) {
                    editingRow.innerHTML = rowContent;
                } else {
                    const newRow = `<tr class="project-row" data-id="${Date.now()}">${rowContent}</tr>`;
                    tableBody.insertAdjacentHTML('afterbegin', newRow);
                }

                modal.hide();
            });


            // --- 8. SİLME ---
            tableBody.addEventListener('click', function (e) {
                const deleteBtn = e.target.closest('.delete-btn');
                if (deleteBtn && confirm("Projeyi silmek istediğinize emin misiniz?")) {
                    deleteBtn.closest('tr').remove();
                }
            });

        });