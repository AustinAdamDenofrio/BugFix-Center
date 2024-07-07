export function refreshDataTables() {
    document.querySelectorAll('table.kt-datatable').forEach(el => {
        if (!DataTable.isDataTable(el) && !el.querySelector('[colspan]')) {
            new DataTable(el, {
                dom:
                    "<f>" +
                    "<'table-responsive'tr>" +
                    "<'row'" +
                    "<'col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start dt-toolbar'li>" +
                    "<'col-sm-12 col-md-7 d-flex align-items-center justify-content-center justify-content-md-end'p>" +
                    ">",
            });
        }
    });
}