document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('skillsContainer');
    const addBtn = document.getElementById('addSkillBtn');
    addBtn.addEventListener('click', function () {
        const newRow = `
                <div class="skill-row d-flex align-items-center gap-3 mt-2">
                    <div class="flex-grow-1">
                        <input type="text" class="form-control form-control-sm fw-bold" placeholder="Yeni Yetenek" value="">
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex align-items-center gap-2">
                            <input type="range" class="skill-range form-range" min="0" max="100" value="50">
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
    });
    container.addEventListener('click', function (e) {
        if (e.target.closest('.delete-btn')) {
            e.target.closest('.skill-row').remove();
        }
    });

    container.addEventListener('input', function (e) {
        if (e.target.classList.contains('skill-range')) {
            const value = e.target.value;
            const badge = e.target.nextElementSibling;
            badge.innerText = value + '%';
        }
    });

    document.getElementById("heroPhoto").addEventListener("change", function (event) {
        const file = event.target.files[0];
        if (!file) return;
        const reader = new FileReader();

        reader.onload = function (e) {
            const previewBox = document.getElementById("heroPreview");
            const heroText = document.getElementById("heroText");

            previewBox.style.backgroundImage =
                `linear-gradient(rgba(0,0,0,0.6),rgba(0,0,0,0.6)), url('${e.target.result}')`;

            heroText.innerHTML = "Yeni Kapak Önizleme";
        };

        reader.readAsDataURL(file);
    });
    document.getElementById("profilePhoto").addEventListener("change", function (event) {
        const file = event.target.files[0];
        if (!file) return;
        const reader = new FileReader();

        reader.onload = function (e) {
            const previewBox = document.getElementById("profilePreview");
            const heroText = document.getElementById("heroText");

            previewBox.style.backgroundImage =
                `linear-gradient(rgba(0,0,0,0.6),rgba(0,0,0,0.6)), url('${e.target.result}')`;

            heroText.innerHTML = "Yeni Kapak Önizleme";
        };

        reader.readAsDataURL(file);
    });
    document.getElementById("profilePhoto").addEventListener("change", function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById("profilePreview").src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
});