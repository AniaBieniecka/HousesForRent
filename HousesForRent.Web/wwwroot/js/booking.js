var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(status) {
    if ($.fn.DataTable.isDataTable('#tblBookings')) {
        $('#tblBookings').DataTable().destroy();
    }
    if (typeof status === 'undefined') {
        status = 'Pending';
    }

    dataTable = $('#tblBookings').dataTable({
        "ajax": {
            url: '/booking/getall?status=' + status,
        },
        "columns": [
            { data: 'id', width: "5%" },
            { data: 'userName', width: "10%" },
            { data: 'phone', width: "10%" },
            { data: 'userEmail', width: "15%" },
            { data: 'status', width: "10%" },
            { data: 'checkInDate', width: "12%" },
            { data: 'nightsQty', width: "5%" },
            { data: 'cost', width: "5%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group">
                        <a href="/booking/bookingDetails?bookingId=${data}" class="btn btn-primary">
                            <i class="bi bi-pencil"></i> Details                        
                            </a>
                            </div>`
                }
            }
        ]
    })
}

function filterTable(selectedStatus) {

    var selectedValue = selectedStatus.value;
    loadDataTable(selectedValue);

}