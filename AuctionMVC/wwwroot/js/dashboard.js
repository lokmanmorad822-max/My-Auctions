/* =============================================================================
   dashboard.js — Dashboard-specific behavior
   - Auto-refresh of relative countdowns (countdown-* elements)
   - Chart.js / lightweight sparkline initialization (if present)
   - Status filter pills -> URL param navigation
   ============================================================================= */

(function () {
  "use strict";

  document.addEventListener("DOMContentLoaded", function () {
    initCountdowns();
    initFilterPills();
    initCharts();
  });

  /* ---------------- Countdowns ---------------- */
  function initCountdowns() {
    var els = document.querySelectorAll("[data-countdown]");
    if (!els.length) return;

    function pad(n) {
      return String(n).padStart(2, "0");
    }

    function tick() {
      els.forEach(function (el) {
        var end = new Date(el.dataset.countdown).getTime();
        var diff = end - Date.now();
        if (diff <= 0) {
          el.textContent = "منتهي";
          el.classList.add("text-muted");
          return;
        }
        var days = Math.floor(diff / 86400000);
        var hours = Math.floor((diff % 86400000) / 3600000);
        var mins = Math.floor((diff % 3600000) / 60000);
        var secs = Math.floor((diff % 60000) / 1000);
        var parts = [];
        if (days > 0) parts.push(days + " يوم");
        if (hours > 0 || days > 0) parts.push(pad(hours) + " س");
        parts.push(pad(mins) + " د");
        parts.push(pad(secs) + " ث");
        el.textContent = parts.join(" ");
      });
    }

    tick();
    setInterval(tick, 1000);
  }

  /* ---------------- Filter pills (data-filter-url) ---------------- */
  function initFilterPills() {
    document.querySelectorAll(".filter-pill[data-filter-url]").forEach(function (pill) {
      pill.addEventListener("click", function () {
        window.location.href = pill.dataset.filterUrl;
      });
    });
  }

  /* ---------------- Charts (Chart.js via CDN) ---------------- */
  function initCharts() {
    var monthlyCanvas = document.getElementById("monthlyAuctionsChart");
    if (!monthlyCanvas || typeof Chart === "undefined") return;

    var labels = JSON.parse(monthlyCanvas.dataset.labels || "[]");
    var data = JSON.parse(monthlyCanvas.dataset.values || "[]");

    new Chart(monthlyCanvas, {
      type: "bar",
      data: {
        labels: labels,
        datasets: [
          {
            label: "المزادات",
            data: data,
            backgroundColor: "rgba(14, 138, 86, 0.7)",
            borderRadius: 8,
            maxBarThickness: 36
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: { precision: 0 },
            grid: { color: "rgba(229,226,218,0.6)" }
          },
          x: {
            grid: { display: false }
          }
        }
      }
    });
  }
})();

