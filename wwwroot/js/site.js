// Sidebar responsive toggle
(function () {
    function ready(fn) {
        if (document.readyState !== "loading") fn();
        else document.addEventListener("DOMContentLoaded", fn);
    }

    ready(function () {
        var shell = document.querySelector(".app-shell");
        var sidebar = document.getElementById("appSidebar");
        var overlay = document.getElementById("sidebarOverlay");
        var openBtn = document.getElementById("menuToggle");
        var closeBtn = document.getElementById("sidebarClose");
        if (!shell || !sidebar) return;

        function openMenu() {
            shell.classList.add("sidebar-open");
            if (overlay) overlay.hidden = false;
            if (openBtn) openBtn.setAttribute("aria-expanded", "true");
            document.body.classList.add("no-scroll");
        }

        function closeMenu() {
            shell.classList.remove("sidebar-open");
            if (overlay) overlay.hidden = true;
            if (openBtn) openBtn.setAttribute("aria-expanded", "false");
            document.body.classList.remove("no-scroll");
        }

        function toggleMenu() {
            if (shell.classList.contains("sidebar-open")) closeMenu();
            else openMenu();
        }

        if (openBtn) openBtn.addEventListener("click", toggleMenu);
        if (closeBtn) closeBtn.addEventListener("click", closeMenu);
        if (overlay) overlay.addEventListener("click", closeMenu);

        sidebar.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function () {
                if (window.matchMedia("(max-width: 960px)").matches) closeMenu();
            });
        });

        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") closeMenu();
        });

        window.addEventListener("resize", function () {
            if (!window.matchMedia("(max-width: 960px)").matches) closeMenu();
        });
    });
})();
