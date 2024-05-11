
$(document).ready(function () {
    loadCustomerBookingPieChartData();
});

function loadCustomerBookingPieChartData() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetCustomerBookingPieChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {


            loadPieChart("customerBookingPieChart", data);

            $(".chart-spinner").hide();
        }
    })
}

function loadPieChart(id, data) {
    var chartColors = getChartColorsArray(id);

    var options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: 'pie',
            width: 380
        }
    }

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}