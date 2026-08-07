/* ==========================================================================
   DudhDahi — Shared behaviour (API Integrated)
   ========================================================================== */

// Hardcoded array ki jagah empty array
let PRODUCTS = [];

// API Endpoint (Apne project ka port number Yahan Check/Change karein)
const API_URL = "/api/DairyProductsapi";

// API se Products fetch karne ka function
async function loadProductsFromAPI() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) {
            throw new Error(`HTTP Error Status: ${response.status}`);
        }
        PRODUCTS = await response.json();

        // Data fetch hone ke baad UI render karenge
        renderFeatured();
        renderAllProducts();
        Cart.refreshUI();
    } catch (error) {
        console.error("API se products fetch karne me problem aayi:", error);
    }
}

function productIcon(kind) {
    const stroke = 'stroke="var(--leaf-deep)" stroke-width="1.6" fill="none" stroke-linecap="round" stroke-linejoin="round"';
    switch (kind) {
        case "bottle":
            return `<svg viewBox="0 0 60 60" xmlns="http://www.w3.org/2000/svg"><path ${stroke} d="M24 6h12v8l4 6v32a2 2 0 0 1-2 2H22a2 2 0 0 1-2-2V20l4-6V6Z"/><path ${stroke} d="M20 26h20"/><path d="M21 27h18v19a1.5 1.5 0 0 1-1.5 1.5h-15A1.5 1.5 0 0 1 21 46V27Z" fill="var(--gold-soft)" opacity="0.55"/></svg>`;
        case "bowl":
            return `<svg viewBox="0 0 60 60" xmlns="http://www.w3.org/2000/svg"><ellipse ${stroke} cx="30" cy="24" rx="18" ry="6"/><path ${stroke} d="M12 24c0 10 4 22 18 22s18-12 18-22"/><path d="M13.5 25c1 8 5 17 16.5 17s15.5-9 16.5-17c-3 2-9 3.5-16.5 3.5S16.5 27 13.5 25Z" fill="var(--gold-soft)" opacity="0.5"/></svg>`;
        case "jar":
            return `<svg viewBox="0 0 60 60" xmlns="http://www.w3.org/2000/svg"><rect ${stroke} x="17" y="10" width="26" height="8" rx="2"/><path ${stroke} d="M20 18h20l3 6v24a4 4 0 0 1-4 4H21a4 4 0 0 1-4-4V24l3-6Z"/><path d="M17 30h26v14a4 4 0 0 1-4 4H21a4 4 0 0 1-4-4V30Z" fill="var(--clay)" opacity="0.35"/></svg>`;
        case "glass":
            return `<svg viewBox="0 0 60 60" xmlns="http://www.w3.org/2000/svg"><path ${stroke} d="M20 14h20l-3 34a3 3 0 0 1-3 3H26a3 3 0 0 1-3-3L20 14Z"/><path ${stroke} d="M18 14h24"/><path d="M20.6 25h18.8l-1.9 21.5a2 2 0 0 1-2 1.8H24.5a2 2 0 0 1-2-1.8L20.6 25Z" fill="var(--gold-soft)" opacity="0.5"/></svg>`;
        case "tub":
            return `<svg viewBox="0 0 60 60" xmlns="http://www.w3.org/2000/svg"><path ${stroke} d="M16 22h28l-3 24a3 3 0 0 1-3 3H22a3 3 0 0 1-3-3l-3-24Z"/><ellipse ${stroke} cx="30" cy="22" rx="14" ry="4.5"/><path d="M17.4 27h25.2l-2.3 18a2 2 0 0 1-2 1.8H21.7a2 2 0 0 1-2-1.8L17.4 27Z" fill="var(--clay)" opacity="0.3"/></svg>`;
        default:
            return "";
    }
}

const Cart = {
    KEY: "dudhdahi_cart",
    read() {
        try { return JSON.parse(localStorage.getItem(this.KEY)) || {}; }
        catch (e) { return {}; }
    },
    write(data) { localStorage.setItem(this.KEY, JSON.stringify(data)); },
    add(id, qty = 1) {
        const data = this.read();
        data[id] = (data[id] || 0) + qty;
        this.write(data);
        this.refreshUI();
    },
    setQty(id, qty) {
        const data = this.read();
        if (qty <= 0) delete data[id];
        else data[id] = qty;
        this.write(data);
        this.refreshUI();
    },
    remove(id) {
        const data = this.read();
        delete data[id];
        this.write(data);
        this.refreshUI();
    },
    count() {
        const data = this.read();
        return Object.values(data).reduce((a, b) => a + b, 0);
    },
    total() {
        const data = this.read();
        let sum = 0;
        for (const id in data) {
            const p = PRODUCTS.find((x) => x.id === id);
            if (p) sum += p.price * data[id];
        }
        return sum;
    },
    refreshUI() {
        document.querySelectorAll(".cart-count").forEach((el) => (el.textContent = this.count()));
        renderCartDrawer();
    },
};

function renderCartDrawer() {
    const itemsEl = document.getElementById("cartItems");
    const totalEl = document.getElementById("cartTotal");
    if (!itemsEl) return;
    const data = Cart.read();
    const ids = Object.keys(data);

    if (ids.length === 0) {
        itemsEl.innerHTML = `<div class="cart-empty">Your basket is empty.<br>Add some fresh dairy to get started.</div>`;
    } else {
        itemsEl.innerHTML = ids
            .map((id) => {
                const p = PRODUCTS.find((x) => x.id === id);
                if (!p) return "";
                const qty = data[id];
                return `
        <div class="cart-item" data-id="${p.id}">
          <div class="cart-item-media">${productIcon(p.icon)}</div>
          <div class="cart-item-info">
            <strong>${p.name}</strong>
            <span>₹${p.price} · ${p.unit}</span>
            <div class="cart-item-qty">
              <button class="qty-btn" data-action="dec" aria-label="Decrease quantity">−</button>
              <span>${qty}</span>
              <button class="qty-btn" data-action="inc" aria-label="Increase quantity">+</button>
              <button class="cart-item-remove" data-action="remove">Remove</button>
            </div>
          </div>
        </div>`;
            })
            .join("");
    }
    if (totalEl) totalEl.textContent = `₹${Cart.total()}`;
}

function bindCartDrawerEvents() {
    const itemsEl = document.getElementById("cartItems");
    if (!itemsEl) return;
    itemsEl.addEventListener("click", (e) => {
        const btn = e.target.closest("button");
        if (!btn) return;
        const row = e.target.closest(".cart-item");
        const id = row.dataset.id;
        const data = Cart.read();
        if (btn.dataset.action === "inc") Cart.setQty(id, (data[id] || 0) + 1);
        if (btn.dataset.action === "dec") Cart.setQty(id, (data[id] || 0) - 1);
        if (btn.dataset.action === "remove") Cart.remove(id);
    });
}

function openCart() {
    document.getElementById("cartOverlay")?.classList.add("open");
    document.getElementById("cartDrawer")?.classList.add("open");
    document.body.style.overflow = "hidden";
}
function closeCart() {
    document.getElementById("cartOverlay")?.classList.remove("open");
    document.getElementById("cartDrawer")?.classList.remove("open");
    document.body.style.overflow = "";
}

let toastTimer;
function showToast(msg) {
    const t = document.getElementById("toast");
    if (!t) return;
    t.innerHTML = `🥛 ${msg}`;
    t.classList.add("show");
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => t.classList.remove("show"), 2200);
}

function bindAddToCart(root = document) {
    root.addEventListener("click", (e) => {
        const btn = e.target.closest(".add-btn");
        if (!btn) return;
        const id = btn.dataset.id;
        const p = PRODUCTS.find((x) => x.id === id);
        if (!p) return;
        Cart.add(id, 1);
        btn.classList.add("added");
        btn.textContent = "✓";
        showToast(`${p.name} added to your basket`);
        setTimeout(() => {
            btn.classList.remove("added");
            btn.textContent = "+";
        }, 900);
    });
}

function initNav() {
    const nav = document.getElementById("mainNav");
    const toggle = document.getElementById("navToggle");
    if (!toggle) return;
    toggle.addEventListener("click", () => nav.classList.toggle("open"));
}

function initReveal() {
    const els = document.querySelectorAll("[data-reveal]");
    if (!("IntersectionObserver" in window) || els.length === 0) {
        els.forEach((el) => el.classList.add("is-visible"));
        return;
    }
    const io = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    io.unobserve(entry.target);
                }
            });
        },
        { threshold: 0.12 }
    );
    els.forEach((el) => io.observe(el));
}

function initFaq() {
    document.querySelectorAll(".faq-item").forEach((item) => {
        const q = item.querySelector(".faq-q");
        const a = item.querySelector(".faq-a");
        if (!q) return;
        q.addEventListener("click", () => {
            const isOpen = item.classList.contains("open");
            document.querySelectorAll(".faq-item.open").forEach((other) => {
                other.classList.remove("open");
                other.querySelector(".faq-a").style.maxHeight = null;
            });
            if (!isOpen) {
                item.classList.add("open");
                a.style.maxHeight = a.scrollHeight + "px";
            }
        });
    });
}

function initContactForm() {
    const form = document.getElementById("contactForm");
    if (!form) return;
    form.addEventListener("submit", (e) => {
        e.preventDefault();
        let valid = true;
        const fields = [
            { id: "cf-name", test: (v) => v.trim().length > 1, msg: "Please enter your name." },
            { id: "cf-phone", test: (v) => /^[0-9]{10}$/.test(v.trim()), msg: "Enter a valid 10-digit phone number." },
            { id: "cf-email", test: (v) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v.trim()), msg: "Enter a valid email address." },
            { id: "cf-message", test: (v) => v.trim().length > 5, msg: "Tell us a little more (min. 6 characters)." },
        ];
        fields.forEach(({ id, test, msg }) => {
            const input = document.getElementById(id);
            if (!input) return;
            const field = input.closest(".field");
            const errorEl = field.querySelector(".field-error");
            if (!test(input.value)) {
                valid = false;
                field.classList.add("has-error");
                errorEl.textContent = msg;
            } else {
                field.classList.remove("has-error");
            }
        });
        const msgBox = document.getElementById("formMsg");
        if (valid) {
            form.reset();
            msgBox.textContent = "Thank you! Your message has been noted — our team will call you back within a day.";
            msgBox.classList.add("show");
            setTimeout(() => msgBox.classList.remove("show"), 5000);
        } else {
            msgBox.classList.remove("show");
        }
    });
}

function productCardHTML(p) {
    return `
  <article class="product-card" data-reveal data-cat="${p.cat}">
    <div class="product-media">
      <span class="product-tag ${p.tag === "Premium" ? "tag-gold" : ""}">${p.tag}</span>
      ${productIcon(p.icon)}
    </div>
    <div class="product-name">${p.name}</div>
    <div class="product-desc">${p.desc}</div>
    <div class="product-foot">
      <div class="product-price">₹${p.price} <span>/ ${p.unit}</span></div>
      <button class="add-btn" data-id="${p.id}" aria-label="Add ${p.name} to cart">+</button>
    </div>
  </article>`;
}

function renderFeatured() {
    const grid = document.getElementById("featuredGrid");
    if (!grid) return;
    const featured = PRODUCTS.filter((p) => ["milk-full", "dahi-classic", "ghee-desi", "chach-masala"].includes(p.id));
    grid.innerHTML = featured.map(productCardHTML).join("");
}

function renderAllProducts(filter = "all") {
    const grid = document.getElementById("productGrid");
    if (!grid) return;
    const list = filter === "all" ? PRODUCTS : PRODUCTS.filter((p) => p.cat === filter);
    grid.innerHTML = list.map(productCardHTML).join("");
    initReveal();
}

function initProductFilters() {
    const pills = document.querySelectorAll(".pill[data-cat]");
    if (pills.length === 0) return;
    pills.forEach((pill) => {
        pill.addEventListener("click", () => {
            pills.forEach((p) => p.classList.remove("active"));
            pill.classList.add("active");
            renderAllProducts(pill.dataset.cat);
        });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    initNav();
    bindAddToCart(document);
    bindCartDrawerEvents();
    initReveal();
    initFaq();
    initContactForm();
    initProductFilters();

    // API Call Triggering
    loadProductsFromAPI();

    document.getElementById("cartToggle")?.addEventListener("click", openCart);
    document.getElementById("cartToggleMobile")?.addEventListener("click", openCart);
    document.getElementById("cartClose")?.addEventListener("click", closeCart);
    document.getElementById("cartOverlay")?.addEventListener("click", closeCart);
});