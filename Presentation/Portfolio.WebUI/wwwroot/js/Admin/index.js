document.addEventListener('DOMContentLoaded', function () {
    
    // ==========================================
    // 1. FORMU DİNLEME VE KAYDET BUTONUNU AKTİF ETME
    // ==========================================
    const form = document.getElementById('aboutMeForm');
    const saveBtn = document.getElementById('saveChangesBtn');

    if (form && saveBtn) {
        // Herhangi bir input'a yazı yazıldığında
        form.addEventListener('input', function () {
            saveBtn.disabled = false;
        });

        // Range slider veya select box gibi elemanlar değiştiğinde
        form.addEventListener('change', function () {
            saveBtn.disabled = false;
        });
    }

    // ==========================================
    // 2. YETENEKLER (SKILLS) KISMI
    // ==========================================
    const container = document.getElementById('skillsContainer');
    const addBtn = document.getElementById('addSkillBtn');
    let skillIndex = 999; // Backend'de çakışma olmaması için yüksek bir index veriyoruz
    
    if(addBtn && container) {
        addBtn.addEventListener('click', function () {
            // DİKKAT: name="..." nitelikleri eklendi!
            const newRow = `
                    <div class="skill-row d-flex align-items-center gap-3 mt-2">
                        <div class="flex-grow-1">
                            <input type="text" name="Skills[${skillIndex}].Name" class="form-control form-control-sm fw-bold" placeholder="Yeni Yetenek" value="">
                        </div>
                        <div class="flex-grow-1">
                            <div class="d-flex align-items-center gap-2">
                                <input type="range" name="Skills[${skillIndex}].Percentage" class="skill-range form-range" min="0" max="100" value="50">
                                <span class="skill-val badge bg-primary rounded-pill" style="width: 50px;">50%</span>
                            </div>
                        </div>
                        <div class="align-self-end pb-1">
                            <button type="button" class="btn btn-sm btn-outline-danger border-0 delete-btn">
                                <i class="bi bi-trash-fill"></i>
                            </button>
                        </div>
                    </div>
                `;
            container.insertAdjacentHTML('beforeend', newRow);
            skillIndex++;

            // Yeni yetenek eklendiğinde de Kaydet butonunu aktif edelim
            if(saveBtn) saveBtn.disabled = false;
        });

        container.addEventListener('click', function (e) {
            if (e.target.closest('.delete-btn')) {
                e.target.closest('.skill-row').remove();
                // Bir yetenek silindiğinde de form değişmiş sayılır, butonu aktif edelim
                if(saveBtn) saveBtn.disabled = false;
            }
        });

        container.addEventListener('input', function (e) {
            if (e.target.classList.contains('skill-range')) {
                const value = e.target.value;
                const badge = e.target.nextElementSibling;
                badge.innerText = value + '%';
            }
        });
    }

    // ==========================================
    // 3. PROFİL FOTOĞRAFI MODALI KISMI
    // ==========================================
   const profileModalInput = document.getElementById("profileModalInput");
const profileModalPreview = document.getElementById("profileModalPreview");
const saveProfileBtn = document.getElementById("saveProfileBtn");
const profilePreviewBox = document.getElementById("profilePreview");
const profileModalEl = document.getElementById('profileModal');

if(profileModalInput && saveProfileBtn) {
profileModalInput.addEventListener("change", function (e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (event) {
            profileModalPreview.src = event.target.result;
        }
        reader.readAsDataURL(file);
    }
});
}

    // ==========================================
    // 4. KAPAK FOTOĞRAFI (HERO IMAGE) KISMI
    // ==========================================
    const heroModalInput = document.getElementById("heroModalInput");
    const heroModalPreview = document.getElementById("heroModalPreview");
    const saveHeroBtn = document.getElementById("saveHeroBtn");
    const heroPreviewBox = document.getElementById("heroPreview");
    const heroModalEl = document.getElementById('heroModal');

    if(heroModalInput && saveHeroBtn) {
        heroModalInput.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (event) {
                    heroModalPreview.src = event.target.result;
                    saveHeroBtn.disabled = false; 
                }
                reader.readAsDataURL(file);
            } else {
                saveHeroBtn.disabled = true;
            }
        });

        saveHeroBtn.addEventListener("click", function () {
            const newImageSrc = heroModalPreview.src;
            heroPreviewBox.style.backgroundImage = `linear-gradient(rgba(0,0,0,0.6),rgba(0,0,0,0.6)), url('${newImageSrc}')`;
            const modalInstance = bootstrap.Modal.getInstance(heroModalEl);
            if(modalInstance) modalInstance.hide();
            saveHeroBtn.disabled = true;
        });
    }
});