// Give-AID Admin Console Scripts
document.addEventListener("DOMContentLoaded", function () {
    const wrapper = document.getElementById("wrapper");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const backdrop = document.getElementById("sidebarBackdrop");
    const sidebarNav = document.getElementById("sidebar-wrapper");

    // 1. Toggle Sidebar & Drawer
    if (sidebarToggle && wrapper) {
        sidebarToggle.addEventListener("click", function (event) {
            event.preventDefault();
            wrapper.classList.toggle("toggled");
        });
    }

    // 2. Close mobile drawer on backdrop click
    if (backdrop && wrapper) {
        backdrop.addEventListener("click", function () {
            wrapper.classList.remove("toggled");
        });
    }

    // 3. Auto-close mobile drawer when a navigation link is clicked (on mobile < 992px)
    if (sidebarNav && wrapper) {
        const links = sidebarNav.querySelectorAll("a.list-group-item");
        links.forEach(link => {
            link.addEventListener("click", function () {
                if (window.innerWidth < 992) {
                    wrapper.classList.remove("toggled");
                }
            });
        });
    }

    // 4. Auto-confirm delete handlers
    const deleteForms = document.querySelectorAll(".delete-confirm-form");
    deleteForms.forEach(form => {
        form.addEventListener("submit", function (e) {
            const itemName = this.getAttribute("data-item-name") || "this item";
            const confirmed = confirm(`Are you sure you want to delete ${itemName}? This action cannot be undone.`);
            if (!confirmed) {
                e.preventDefault();
            }
        });
    });

    // 5. Highlight active menu item based on current URL
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll("#sidebar-wrapper .list-group-item");
    navLinks.forEach(link => {
        const href = link.getAttribute("href")?.toLowerCase();
        if (href) {
            if (currentPath === href) {
                link.classList.add("active");
            } else if (href !== "/admin" && href !== "/admin/dashboard" && currentPath.startsWith(href)) {
                link.classList.add("active");
            }
        }
    });

    // 6. Reset drawer state if window is resized above 992px
    window.addEventListener("resize", function () {
        if (window.innerWidth >= 992 && wrapper && wrapper.classList.contains("toggled")) {
            wrapper.classList.remove("toggled");
        }
    });
});
