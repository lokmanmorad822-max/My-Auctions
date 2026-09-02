/* =============================================================================
   modals.js — Generic confirmation modal wiring for admin actions.
   Supports data-* attributes:
     data-confirm-title
     data-confirm-message
     data-confirm-icon (bootstrap icon class, e.g. bi-stop-circle)
     data-confirm-variant (danger | gold | primary)
     data-confirm-action (url for form action)
     data-confirm-method (POST default)
     data-confirm-text (button label)
     data-confirm-require-text (optional: e.g. "حذف" forces user to type before confirm)
   ============================================================================= */

(function () {
  "use strict";

  document.addEventListener("DOMContentLoaded", function () {
    var triggerButtons = document.querySelectorAll("[data-confirm-action]");

    triggerButtons.forEach(function (btn) {
      btn.addEventListener("click", function (e) {
        e.preventDefault();
        openConfirmModal(btn);
      });
    });
  });

  function openConfirmModal(btn) {
    var existing = document.getElementById("confirmModal");
    if (existing) existing.remove();

    var title = btn.dataset.confirmTitle || "تأكيد الإجراء";
    var message = btn.dataset.confirmMessage || "هل أنت متأكد من تنفيذ هذا الإجراء؟";
    var icon = btn.dataset.confirmIcon || "bi-question-circle";
    var variant = btn.dataset.confirmVariant || "primary";
    var action = btn.dataset.confirmAction;
    var method = btn.dataset.confirmMethod || "POST";
    var confirmText = btn.dataset.confirmText || "تأكيد";
    var requireText = btn.dataset.confirmRequireText || "";

    var iconClass = "text-primary bg-soft-success";
    var btnClass = "btn-primary";
    if (variant === "danger") {
      iconClass = "text-danger bg-soft-danger";
      btnClass = "btn-danger";
    } else if (variant === "gold") {
      iconClass = "text-gold bg-soft-warning";
      btnClass = "btn-gold";
    }

    var modal = document.createElement("div");
    modal.id = "confirmModal";
    modal.className = "modal fade";
    modal.tabIndex = -1;
    modal.setAttribute("aria-hidden", "true");

    var requireFieldHtml = "";
    if (requireText) {
      requireFieldHtml =
        '<div class="mb-3">' +
        '<label class="form-label fs-8">اكتب "' + requireText + '" للتأكيد</label>' +
        '<input type="text" class="form-control confirm-required-input" placeholder="' + requireText + '" autocomplete="off">' +
        "</div>";
    }

    modal.innerHTML =
      '<div class="modal-dialog modal-dialog-centered">' +
      '<div class="modal-content rounded-4 border-0 shadow">' +
      '<div class="modal-body text-center p-4">' +
      '<div class="d-inline-grid place-items-center rounded-circle p-3 mb-3 ' + iconClass + '" style="width:4rem;height:4rem;font-size:1.6rem">' +
      '<i class="bi ' + icon + '"></i>' +
      "</div>" +
      "<h5 class='fw-900 mb-2'>" + title + "</h5>" +
      "<p class='text-muted fs-7 mb-4'>" + message + "</p>" +
      requireFieldHtml +
      '<form method="post" action="' + action + '" class="d-flex gap-2 justify-content-center confirm-form">' +
      '<input type="hidden" name="_method" value="' + method + '">' +
      '<button type="button" class="btn btn-soft-secondary flex-fill confirm-cancel">تراجع</button>' +
      '<button type="submit" class="btn ' + btnClass + ' flex-fill confirm-submit">' + confirmText + "</button>" +
      "</form>" +
      "</div>" +
      "</div>" +
      "</div>";

    document.body.appendChild(modal);

    var modalInstance = new bootstrap.Modal(modal);
    modalInstance.show();

    modal.addEventListener("hidden.bs.modal", function () {
      modal.remove();
    });

    // Cancel button
    modal.querySelector(".confirm-cancel").addEventListener("click", function () {
      modalInstance.hide();
    });

    // Require-text gate
    var submit = modal.querySelector(".confirm-submit");
    var reqInput = modal.querySelector(".confirm-required-input");
    if (reqInput) {
      submit.disabled = true;
      reqInput.addEventListener("input", function () {
        submit.disabled = reqInput.value.trim() !== requireText;
      });
    }
  }
})();

