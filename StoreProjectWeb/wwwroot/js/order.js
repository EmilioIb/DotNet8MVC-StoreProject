let dataTable;
$(document).ready(function () {
    const url = window.location.search;

    if (url != "") {
        let query = url.replace("?status=", "");
        loadDataTable(query);
    } else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    console.log(status)
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            {
                data: 'orderHeaderID',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-info mx-2"> <i class="bi bi-eye-fill"></i></a> 
                    </div>`;
                },
                "width": "10%"
            },
            { data: 'orderHeaderID', "width": "5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
        ]
    });
}
