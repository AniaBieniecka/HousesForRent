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
            { data: 'id' },
            { data: 'house.name' },
            { data: 'userName' },
            { data: 'phone' },
            { data: 'status' },
            { data: 'checkInDate' },
            { data: 'nightsQty' },
            { data: 'cost' },
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