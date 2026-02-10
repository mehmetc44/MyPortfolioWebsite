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

        });