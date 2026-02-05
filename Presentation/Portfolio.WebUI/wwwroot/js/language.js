
  const selected = document.getElementById('selected-lang');
  const menu = document.getElementById('lang-menu');

  // Menü aç/kapa
  selected.addEventListener('click', (e) => {
    e.preventDefault();
    menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
  });

  // Dil değiştir
  function setLang(name, code) {
    selected.innerHTML = `
      <img src="https://hatscripts.github.io/circle-flags/flags/${code}.svg" style="width: 16px; height: 16px; margin-right: 8px;">
      ${name}
      <i class="bi bi-chevron-down" style="margin-left: 6px;"></i>
    `;
    menu.style.display = 'none';
  }

  // Dışarı tıklanınca menüyü kapat
  document.addEventListener('click', (e) => {
    if (!e.target.closest('.dropdown')) {
      menu.style.display = 'none';
    }
  });
