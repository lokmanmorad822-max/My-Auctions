/* =============================================================================
   site.js — Global admin dashboard behaviors
   - Mobile sidebar toggle
   - Auto-dismiss alerts
   - Confirm dialog helpers (used by modals.js)
   - Global fetch with bearer token forwarding
   ============================================================================= */

(function () {
  "use strict";

  document.addEventListener("DOMContentLoaded", function () {
    initSidebarToggle();
    initAlerts();
    initGlobalSearch();
  });

  /* ---------------- Sidebar (mobile) ---------------- */
  function initSidebarToggle() {
    var toggler = document.getElementById("sidebarToggle");
    var sidebar = document.getElementById("adminSidebar");
    var backdrop = document.getElementById("sidebarBackdrop");

    if (!toggler || !sidebar) return;

    function open() {
      sidebar.classList.add("open");
      if (backdrop) backdrop.classList.add("show");
      document.body.style.overflow = "hidden";
    }

    function close() {
      sidebar.classList.remove("open");
      if (backdrop) backdrop.classList.remove("show");
      document.body.style.overflow = "";
    }

    toggler.addEventListener("click", open);
    if (backdrop) backdrop.addEventListener("click", close);

    // Close on nav link click (mobile)
    sidebar.querySelectorAll(".sidebar-link").forEach(function (link) {
      link.addEventListener("click", function () {
        if (window.innerWidth <= 991.98) close();
      });
    });
  }

  /* ---------------- Alerts ---------------- */
  function initAlerts() {
    var alerts = document.querySelectorAll(".alert-dismissible");
    alerts.forEach(function (alert) {
      var delay = parseInt(alert.dataset.delay || "0", 10);
      if (delay > 0) {
        setTimeout(function () {
          bootstrap.Alert.getOrCreateInstance(alert).close();
        }, delay);
      }
    });
  }

  /* ---------------- Global search (topbar) ---------------- */
  function initGlobalSearch() {
    var input = document.getElementById("globalSearch");
    if (!input) return;

    input.addEventListener("keydown", function (e) {
      if (e.key === "Enter" && input.value.trim()) {
        window.location.href = "/Auctions/Index?search=" + encodeURIComponent(input.value.trim());
      }
    });
  }
})();

