// OrderHub Lab — checkbox persistence + active TOC highlighting

const STORAGE_KEY = 'orderhub-lab-progress-v1';

function loadState() {
  try { return JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}'); }
  catch { return {}; }
}

function saveState(state) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

function updateProgress() {
  const all = document.querySelectorAll('input[type="checkbox"][data-key]');
  const done = Array.from(all).filter(cb => cb.checked).length;
  const total = all.length;
  const pct = total ? Math.round(done / total * 100) : 0;

  const counter = document.querySelector('.progress-count');
  const bar = document.querySelector('.progress-bar-fill');
  if (counter) counter.textContent = `${done} / ${total}`;
  if (bar) bar.style.width = `${pct}%`;
}

function initCheckboxes() {
  const state = loadState();
  document.querySelectorAll('input[type="checkbox"][data-key]').forEach(cb => {
    const key = cb.dataset.key;
    if (state[key]) cb.checked = true;
    cb.addEventListener('change', () => {
      state[key] = cb.checked;
      saveState(state);
      updateProgress();
    });
  });
  updateProgress();
}

function initActiveTOC() {
  const links = document.querySelectorAll('.toc a');
  const linkByHref = new Map();
  links.forEach(a => {
    const id = a.getAttribute('href').slice(1);
    linkByHref.set(id, a);
  });

  const sections = Array.from(document.querySelectorAll('section[id], article[id]'));
  if (sections.length === 0) return;

  const observer = new IntersectionObserver(entries => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        links.forEach(l => l.classList.remove('active'));
        const link = linkByHref.get(entry.target.id);
        if (link) link.classList.add('active');
      }
    });
  }, {
    rootMargin: '-30% 0px -60% 0px',
    threshold: 0
  });

  sections.forEach(s => observer.observe(s));
}

document.addEventListener('DOMContentLoaded', () => {
  initCheckboxes();
  initActiveTOC();
});
