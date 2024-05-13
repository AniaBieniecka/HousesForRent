
$(document).ready(function () {
    loadCustomerAndBookingLineChartData();
});

function loadCustomerAndBookingLineChartData() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetCustomerAndBookingLineChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {


            loadLineChart("newCustomerAndBookingLineChart", data);

            $(".chart-spinner").hide();
        }
    })
}

function loadLineChart(id, data) {
    var chartColors = getChartColorsArray(id);

    var options = {
        series: data.series,

        colors: chartColors,
        chart: {
            type: 'line',
            width: "100%"
        },
        stroke: {
            width: 4
        },
        markers: {
            size: 6,
            hover: {
                sizeOffset: 4
            }
        },
        xaxis: {
            categories: data.categories,
        },
    }

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}