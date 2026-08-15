/* =============================================================================
   datatables.init.js — DataTables Arabic localization + admin table presets
   Requires: DataTables core + buttons + dataTables.bootstrap5 + Bootstrap Icons
   ============================================================================= */

(function () {
  "use strict";

  // Arabic localization dictionary
  var arabic = {
    sEmptyTable: "لا توجد بيانات متاحة في الجدول",
    sInfo: "عرض _START_ إلى _END_ من أصل _TOTAL_ سجل",
    sInfoEmpty: "عرض 0 إلى 0 من أصل 0 سجل",
    sInfoFiltered: "(تم تصفية من إجمالي _MAX_ سجل)",
    sInfoThousands: ",",
    sLengthMenu: "عرض _MENU_ سجل",
    sLoadingRecords: "جارٍ التحميل...",
    sProcessing: "جارٍ المعالجة...",
    sSearch: "بحث:",
    sZeroRecords: "لم يتم العثور على سجلات مطابقة",
    oPaginate: {
      sFirst: "<<",
      sLast: ">>",
      sNext: "التالي",
      sPrevious: "السابق"
    },
    oAria: {
      sSortAscending: ": تفعيل لترتيب العمود تصاعدياً",
      sSortDescending: ": تفعيل لترتيب العمود تنازلياً"
    }
  };

  window.AuctionDataTables = {
    arabic: arabic,

    defaultConfig: function (options) {
      var base = {
        language: arabic,
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [],
        dom:
          "<'row mb-3'<'col-12 col-md-6'f><'col-12 col-md-6'<'d-flex justify-content-md-end'B>>>" +
          "<'row'<'col-12'tr>>" +
          "<'row'<'col-12 col-md-5'i><'col-12 col-md-7'p>>",
        buttons: [
          {
            extend: "copyHtml5",
            text: '<i class="bi bi-copy"></i> نسخ',
            className: "btn btn-soft-secondary btn-sm",
            titleAttr: "نسخ البيانات"
          },
          {
            extend: "csvHtml5",
            text: '<i class="bi bi-filetype-csv"></i> CSV',
            className: "btn btn-soft-secondary btn-sm",
            titleAttr: "تصدير CSV"
          },
          {
            extend: "excelHtml5",
            text: '<i class="bi bi-file-earmark-excel"></i> Excel',
            className: "btn btn-soft-secondary btn-sm",
            titleAttr: "تصدير Excel"
          },
          {
            extend: "print",
            text: '<i class="bi bi-printer"></i> طباعة',
            className: "btn btn-soft-secondary btn-sm",
            titleAttr: "طباعة"
          }
        ],
        initComplete: function () {
          // Style the search input container to match the dashboard look
          var wrapper = this.api().table().container();
          var filter = wrapper.querySelector(".dataTables_filter");
          if (filter) {
            filter.classList.add("mb-0");
          }
        }
      };

      return Object.assign({}, base, options || {});
    },

    init: function (selector, options) {
      var config = this.defaultConfig(options);
      var table = $(selector).DataTable(config);
      return table;
    }
  };
})();

