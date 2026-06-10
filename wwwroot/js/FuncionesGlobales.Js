// ─── Helpers del carrito ────────────────────────────────────────────────────
// Si hay usuario autenticado → localStorage con clave por usuario (persistente).
// Si es invitado            → sessionStorage (se vacía al cerrar/reabrir el tab).

function getCartKey() {
    const user = (window.AURA_USER || '').trim();
    return user ? 'aura_cart_' + user : 'aura_cart_guest';
}

function getCartStorage() {
    return (window.AURA_USER || '').trim() ? localStorage : sessionStorage;
}

function getCart() {
    return JSON.parse(getCartStorage().getItem(getCartKey()) || '[]');
}

function saveCart(cart) {
    getCartStorage().setItem(getCartKey(), JSON.stringify(cart));
}
// ─────────────────────────────────────────────────────────────────────────────

let isCartOpen = false;

function toggleCart() {
    const sidebar = document.getElementById('cart-sidebar');
    const overlay = document.getElementById('cart-overlay');
    const fab = document.getElementById('cart-fab');
    if (!sidebar || !overlay) return;

    isCartOpen = !isCartOpen;
    if (isCartOpen) {
        sidebar.classList.remove('hidden');
        overlay.classList.remove('hidden');
        if (fab) fab.classList.add('scale-0');
        renderCart();
        requestAnimationFrame(() => {
            sidebar.classList.remove('translate-x-[calc(100%+2rem)]');
            overlay.classList.remove('opacity-0');
        });
    } else {
        sidebar.classList.add('translate-x-[calc(100%+2rem)]');
        overlay.classList.add('opacity-0');
        if (fab) fab.classList.remove('scale-0');
        setTimeout(() => {
            sidebar.classList.add('hidden');
            overlay.classList.add('hidden');
        }, 300);
    }
}

function renderCart() {
    const cart = getCart();
    const container = document.getElementById('cart-items-container');
    const totalEl = document.getElementById('cart-total');
    if (!container || !totalEl) return;

    if (cart.length === 0) {
        container.innerHTML = `
            <div class="flex flex-col items-center justify-center h-full text-center text-neutral-400">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-16 h-16 mb-4 text-neutral-200" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15.75 10.5V6a3.75 3.75 0 1 0-7.5 0v4.5m11.356-1.993 1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 0 1-1.12-1.243l1.264-12A1.125 1.125 0 0 1 5.513 7.5h12.974c.576 0 1.059.435 1.119 1.007Z" /></svg>
                <p class="font-medium text-sm">Tu carrito esta vacio</p>
                <button onclick="toggleCart()" class="mt-4 text-amber-500 font-bold text-xs hover:underline">Continuar comprando</button>
            </div>
        `;
        totalEl.textContent = '$0.00';
        return;
    }

    let total = 0;
    container.innerHTML = cart.map(item => {
        total += item.price * (item.qty || 1);
        return `
            <div class="flex gap-4 bg-white border border-neutral-100 p-3 rounded-2xl shadow-sm relative group transition-all hover:shadow-md">
                <button onclick="removeCartItem(${item.id})" class="absolute -top-2 -right-2 bg-white border border-red-100 hover:bg-red-500 text-red-500 hover:text-white transition-colors rounded-full p-1.5 shadow-sm opacity-0 group-hover:opacity-100 focus:opacity-100">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke-width="2.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" /></svg>
                </button>
                <div class="w-20 h-20 rounded-xl overflow-hidden bg-neutral-100 flex-shrink-0">
                    <img src="${item.imageUrl}" class="w-full h-full object-cover" onerror="this.src='https://images.unsplash.com/photo-1521572267360-ee0c2909d518?q=80&w=200&auto=format&fit=crop'" />
                </div>
                <div class="flex flex-col justify-between flex-grow">
                    <div>
                        <h4 class="text-sm font-bold text-neutral-800 line-clamp-1">${item.name}</h4>
                        <p class="text-amber-500 font-black text-sm mt-0.5">$${item.price.toFixed(2)}</p>
                    </div>
                    <div class="flex items-center gap-3">
                        <div class="flex items-center border border-neutral-200 rounded-lg bg-neutral-50 overflow-hidden">
                            <button onclick="updateCartItemQty(${item.id}, -1)" class="px-2.5 py-1 text-neutral-500 hover:text-neutral-900 hover:bg-neutral-200 transition-colors">-</button>
                            <span class="px-2 text-xs font-bold text-neutral-900 w-6 text-center">${item.qty || 1}</span>
                            <button onclick="updateCartItemQty(${item.id}, 1)" class="px-2.5 py-1 text-neutral-500 hover:text-neutral-900 hover:bg-neutral-200 transition-colors">+</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }).join('');

    totalEl.textContent = '$' + total.toFixed(2);
}

function updateCartItemQty(id, delta) {
    const cart = getCart();
    const existing = cart.find(i => i.id === id);
    if (existing) {
        existing.qty = (existing.qty || 1) + delta;
        if (existing.qty < 1) existing.qty = 1;
        saveCart(cart);
        updateCartCount();
        if(isCartOpen) renderCart();
    }
}

function removeCartItem(id) {
    let cart = getCart();
    cart = cart.filter(i => i.id !== id);
    saveCart(cart);
    updateCartCount();
    if(isCartOpen) renderCart();
    showToast('Producto eliminado del carrito', 'error');
}

function addToCart(id, name, price, imageUrl) {
    const cart = getCart();
    const existing = cart.find(i => i.id === id);
    if (existing) { existing.qty = (existing.qty || 1) + 1; }
    else { cart.push({ id, name, price, imageUrl, qty: 1 }); }
    saveCart(cart);
    updateCartCount();
    if (!isCartOpen) {
        toggleCart();
    } else {
        renderCart();
    }
    showToast('Agregado al carrito!', 'success');
}

function updateCartCount() {
    const cart = getCart();
    const total = cart.reduce((s, i) => s + (i.qty || 1), 0);
    document.querySelectorAll('.cart-count').forEach(el => {
        el.textContent = total;
        // Trigger bounce animation
        el.classList.remove('badge-bounce');
        void el.offsetWidth; // Force reflow
        el.classList.add('badge-bounce');
    });
}

function showToast(msg, type) {
    let t = document.getElementById('toast');
    if (!t) t = document.getElementById('toast-home');
    if (!t) return;

    const textEl = t.querySelector('span');
    if (textEl) textEl.textContent = msg;

    const icon = document.getElementById('toast-icon');
    if (icon) {
        icon.className = type === 'error' ? 'bg-red-500 rounded-full p-1.5 flex-shrink-0 text-white' : 'bg-amber-400 rounded-full p-1.5 flex-shrink-0 text-neutral-900';
        icon.innerHTML = type === 'error' 
            ? `<svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke-width="3" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" /></svg>`
            : `<svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke-width="3" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="m4.5 12.75 6 6 9-13.5" /></svg>`;
    }

    t.classList.remove('translate-y-20','opacity-0');
    t.classList.add('translate-y-0','opacity-100');
    setTimeout(() => { 
        t.classList.add('translate-y-20','opacity-0'); 
        t.classList.remove('translate-y-0','opacity-100'); 
    }, 2800);
}

function openModal(id) {
    const overlay = document.getElementById(id);
    const box = document.getElementById(id + '-box');
    if(!overlay || !box) return;
    overlay.classList.remove('hidden');
    overlay.classList.add('flex');
    requestAnimationFrame(() => { box.classList.remove('scale-95','opacity-0'); box.classList.add('scale-100','opacity-100'); });
}

function closeModal(id) {
    const overlay = document.getElementById(id);
    const box = document.getElementById(id + '-box');
    if(!overlay || !box) return;
    box.classList.add('scale-95','opacity-0');
    box.classList.remove('scale-100','opacity-100');
    setTimeout(() => { overlay.classList.add('hidden'); overlay.classList.remove('flex'); }, 200);
}

function previewImage(inputId, imgId, phId) {
    const url = document.getElementById(inputId).value;
    const img = document.getElementById(imgId);
    if (url) { img.src = url; img.classList.remove('hidden'); if(phId) document.getElementById(phId).style.display='none'; }
    else { img.src=''; img.classList.add('hidden'); if(phId) document.getElementById(phId).style.display=''; }
}

function setButtonLoading(btnId, loading, defaultHtml) {
    const btn = document.getElementById(btnId);
    if (loading) { btn.disabled=true; btn.innerHTML='<svg class="animate-spin w-4 h-4 mr-1" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path></svg>Guardando...'; }
    else { btn.disabled=false; btn.innerHTML=defaultHtml; }
}

function openCreateModal() {
    document.getElementById('create-name').value='';
    document.getElementById('create-url').value='';
    document.getElementById('create-url2').value='';
    document.getElementById('create-price').value='';
    document.getElementById('create-preview').src='';
    document.getElementById('create-preview2').src='';
    document.getElementById('create-preview').classList.add('hidden');
    document.getElementById('create-preview2').classList.add('hidden');
    document.getElementById('create-ph').style.display='';
    document.getElementById('create-ph2').style.display='';
    document.getElementById('create-error').classList.add('hidden');
    document.getElementById('btn-save-create').innerHTML='<span>Guardar Producto</span>';
    openModal('modal-create');
}

async function saveCreate() {
    const name = document.getElementById('create-name').value.trim();
    const url = document.getElementById('create-url').value.trim();
    const url2 = document.getElementById('create-url2').value.trim() || null;
    const price = parseFloat(document.getElementById('create-price').value);
    const errDiv = document.getElementById('create-error');
    if (!name || !url || isNaN(price) || price < 0) { errDiv.textContent='Por favor completa los campos obligatorios (*) correctamente.'; errDiv.classList.remove('hidden'); return; }
    errDiv.classList.add('hidden');
    setButtonLoading('btn-save-create', true, '<span>Guardar Producto</span>');
    try {
        const res = await fetch('/Catalog/Create', { method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify({imageName:name,imageUrl:url,imageUrl2:url2,precio:price,activo:true}) });
        if (!res.ok) { const d = await res.json(); throw new Error(d.error||'Error al crear.'); }
        closeModal('modal-create'); showToast('Producto creado exitosamente','success'); setTimeout(()=>location.reload(),1200);
    } catch(e) { errDiv.textContent=e.message; errDiv.classList.remove('hidden'); setButtonLoading('btn-save-create',false,'<span>Guardar Producto</span>'); }
}

function openEditModal(id, name, url, url2, price, activo) {
    document.getElementById('edit-id').value=id;
    document.getElementById('edit-name').value=name;
    document.getElementById('edit-url').value=url;
    document.getElementById('edit-url2').value=url2 || '';
    document.getElementById('edit-price').value=price;
    document.getElementById('edit-activo').checked=(activo===true||activo==='true');
    
    document.getElementById('edit-preview').src=url;
    document.getElementById('edit-preview').classList.remove('hidden');
    
    if(url2) {
        document.getElementById('edit-preview2').src=url2;
        document.getElementById('edit-preview2').classList.remove('hidden');
        document.getElementById('edit-ph2').style.display='none';
    } else {
        document.getElementById('edit-preview2').src='';
        document.getElementById('edit-preview2').classList.add('hidden');
        document.getElementById('edit-ph2').style.display='';
    }

    document.getElementById('edit-error').classList.add('hidden');
    document.getElementById('btn-save-edit').innerHTML='<span>Guardar Cambios</span>';
    openModal('modal-edit');
}

async function saveEdit() {
    const id = parseInt(document.getElementById('edit-id').value);
    const name = document.getElementById('edit-name').value.trim();
    const url = document.getElementById('edit-url').value.trim();
    const url2 = document.getElementById('edit-url2').value.trim() || null;
    const price = parseFloat(document.getElementById('edit-price').value);
    const activo = document.getElementById('edit-activo').checked;
    const errDiv = document.getElementById('edit-error');
    if (!name || !url || isNaN(price) || price < 0) { errDiv.textContent='Por favor completa los campos obligatorios (*) correctamente.'; errDiv.classList.remove('hidden'); return; }
    errDiv.classList.add('hidden');
    setButtonLoading('btn-save-edit',true,'<span>Guardar Cambios</span>');
    try {
        const res = await fetch('/Catalog/Update', { method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify({id,imageName:name,imageUrl:url,imageUrl2:url2,precio:price,activo}) });
        if (!res.ok) { const d = await res.json(); throw new Error(d.error||'Error al actualizar.'); }
        closeModal('modal-edit'); showToast('Producto actualizado correctamente','success'); setTimeout(()=>location.reload(),1200);
    } catch(e) { errDiv.textContent=e.message; errDiv.classList.remove('hidden'); setButtonLoading('btn-save-edit',false,'<span>Guardar Cambios</span>'); }
}

let pendingDeleteId = null;
function deleteProduct(id, name) { pendingDeleteId=id; document.getElementById('delete-product-name').textContent='"'+name+'"'; openModal('modal-delete'); }
async function confirmDelete() {
    if (!pendingDeleteId) return;
    const btn = document.getElementById('btn-confirm-delete'); btn.textContent='Eliminando...'; btn.disabled=true;
    try {
        const res = await fetch('/Catalog/Delete', { method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify({id:pendingDeleteId}) });
        if (!res.ok) { const d = await res.json(); throw new Error(d.error||'Error al eliminar.'); }
        closeModal('modal-delete'); showToast('Producto eliminado correctamente','success'); setTimeout(()=>location.reload(),1200);
    } catch(e) { showToast(e.message,'error'); closeModal('modal-delete'); btn.textContent='Si, eliminar'; btn.disabled=false; }
}

function filterProducts() {
    const query = document.getElementById('search-input').value.toLowerCase();
    const cards = document.querySelectorAll('.product-card');
    let visible = 0;
    cards.forEach(card => { const show = (card.dataset.name||'').toLowerCase().includes(query); card.style.display=show?'':'none'; if(show)visible++; });
    document.getElementById('no-results').classList.toggle('hidden', visible > 0);
}

function sortProducts() {
    const val = document.getElementById('sort-select').value;
    const grid = document.getElementById('products-grid');
    const cards = Array.from(document.querySelectorAll('.product-card'));
    
    cards.sort((a,b) => {
        const pa=parseFloat(a.dataset.price), pb=parseFloat(b.dataset.price);
        const na=(a.dataset.name||'').toLowerCase(), nb=(b.dataset.name||'').toLowerCase();
        const ida=parseInt(a.dataset.id), idb=parseInt(b.dataset.id);
        
        if(val==='price-asc') return pa-pb;
        if(val==='price-desc') return pb-pa;
        if(val==='name-asc') return na.localeCompare(nb);
        return idb-ida;
    });
    cards.forEach(c => grid.appendChild(c));
}

// LOGICA MODAL DETALLE
let currentDetail = null;
let showingImg2 = false;

function openDetailModal(id, name, url, url2, price) {
    currentDetail = { id, name, url, url2, price };
    showingImg2 = false;
    
    document.getElementById('detail-name').textContent = name;
    document.getElementById('detail-price').textContent = '$' + price.toFixed(2);
    
    const img1 = document.getElementById('detail-image');
    const img2 = document.getElementById('detail-image-2');
    img1.src = url;
    
    img1.classList.remove('opacity-0');
    img2.classList.add('opacity-0');
    
    const hasSecondImage = url2 && url2.trim() !== '';
    const thumbnails = document.getElementById('detail-thumbnails');
    const navButtons = document.getElementById('detail-nav-buttons');
    
    if (hasSecondImage) {
        img2.src = url2;
        thumbnails.classList.remove('hidden');
        navButtons.classList.remove('hidden');
        
        document.getElementById('detail-thumb-1').src = url;
        document.getElementById('detail-thumb-2').src = url2;
        
        updateThumbnails();
    } else {
        thumbnails.classList.add('hidden');
        navButtons.classList.add('hidden');
    }
    
    openModal('modal-detail');
}

function setDetailImage(index) {
    if (!currentDetail || !currentDetail.url2) return;
    showingImg2 = (index === 1);
    
    const img1 = document.getElementById('detail-image');
    const img2 = document.getElementById('detail-image-2');
    
    if (showingImg2) {
        img1.classList.add('opacity-0');
        img2.classList.remove('opacity-0');
    } else {
        img1.classList.remove('opacity-0');
        img2.classList.add('opacity-0');
    }
    updateThumbnails();
}

function toggleDetailImage() {
    setDetailImage(showingImg2 ? 0 : 1);
}

function updateThumbnails() {
    const btn1 = document.getElementById('thumb-btn-1');
    const btn2 = document.getElementById('thumb-btn-2');
    if (showingImg2) {
        btn1.className = 'w-full aspect-[3/4] rounded-lg overflow-hidden border-2 border-transparent opacity-60 hover:opacity-100 transition-all focus:outline-none';
        btn2.className = 'w-full aspect-[3/4] rounded-lg overflow-hidden border-2 border-neutral-900 opacity-100 transition-all focus:outline-none';
    } else {
        btn1.className = 'w-full aspect-[3/4] rounded-lg overflow-hidden border-2 border-neutral-900 opacity-100 transition-all focus:outline-none';
        btn2.className = 'w-full aspect-[3/4] rounded-lg overflow-hidden border-2 border-transparent opacity-60 hover:opacity-100 transition-all focus:outline-none';
    }
}

function addDetailToCart() {
    if(!currentDetail) return;
    addToCart(currentDetail.id, currentDetail.name, currentDetail.price, currentDetail.url);
    closeModal('modal-detail');
}

document.addEventListener('DOMContentLoaded', () => {
    updateCartCount();
});

// --- Detección de doble-tap para móviles ---
let lastTap = 0;
let lastTapCard = null;

document.addEventListener('touchend', e => {
    const card = e.target.closest('.product-card');
    if (!card) return;
    if (e.target.closest('button') || e.target.closest('a')) return;

    const now = Date.now();
    const DOUBLE_TAP_DELAY = 350;
    const isSameCard = lastTapCard === card;

    if (isSameCard && now - lastTap < DOUBLE_TAP_DELAY) {
        // Doble tap confirmado — evitar zoom del navegador
        e.preventDefault();
        lastTap = 0;
        lastTapCard = null;

        const id = parseInt(card.dataset.id);
        const name = card.dataset.productName || card.dataset.name;
        const imageUrl = card.dataset.image;
        const imageUrl2 = card.dataset.image2;
        const price = parseFloat(card.dataset.price);

        // Efecto visual de flash al abrir
        card.classList.add('ring-2', 'ring-amber-400', 'ring-offset-2');
        setTimeout(() => card.classList.remove('ring-2', 'ring-amber-400', 'ring-offset-2'), 400);

        openDetailModal(id, name, imageUrl, imageUrl2, price);
    } else {
        lastTap = now;
        lastTapCard = card;
    }
}, { passive: false });

// Evitar el zoom nativo de doble-tap en tarjetas de producto
document.addEventListener('touchstart', e => {
    const card = e.target.closest('.product-card');
    if (!card) return;
    if (e.target.closest('button') || e.target.closest('a')) return;
    // Usamos touchend para la lógica, pero prevenimos el comportamiento
    // por defecto del segundo tap para evitar zoom
    const now = Date.now();
    if (lastTapCard === card && now - lastTap < 350) {
        e.preventDefault();
    }
}, { passive: false });

document.addEventListener('keydown', e => {
    if (e.key === 'Escape') { 
        if (isCartOpen) { toggleCart(); }
        if (isMobileMenuOpen) { toggleMobileMenu(); }
        ['modal-create','modal-edit','modal-delete','modal-detail'].forEach(id => { 
            let el = document.getElementById(id);
            if(el && !el.classList.contains('hidden')) closeModal(id); 
        }); 
    }
});
