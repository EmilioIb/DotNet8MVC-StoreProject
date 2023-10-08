let dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            {
                data: { id: "id", lockoutEnd: "lockoutEnd"},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `<div class="text-ceter">
                            <a onClick="LockUnlock('${data.id}')" class="btn btn-success text-white" style="cursor: pointer; width: 100px;">
                                <i class="bi bi-unlock-fill"></i>
                            </a>
                            <a href="/admin/user/RoleManagement?id=${data.id}" class="btn btn-warning text-white" style="cursor: pointer; width: 100px;">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                    } else {
                        return `<div class="text-ceter">
                            <a onClick="LockUnlock('${data.id}')" class="btn btn-danger text-white" style="cursor: pointer; width: 100px;">
                                <i class="bi bi-lock-fill"></i>
                            </a>
                            <a href="/admin/user/RoleManagement?id=${data.id}" class="btn btn-warning text-white" style="cursor: pointer; width: 100px;">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                    }
                },
                "width": "20%"
            },
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'role', "width": "15%" },
            { data: 'company.name', "width": "15%" },
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }


    })
}
