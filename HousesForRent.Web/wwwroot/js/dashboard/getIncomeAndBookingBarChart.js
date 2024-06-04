
$(document).ready(function () {
    loadIncomeAndBookingBarChartData();
});

function loadIncomeAndBookingBarChartData() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetIncomeAndBookingBarChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {


            loadBarChart("incomeAndBookingBarChartData", data);

            $(".chart-spinner").hide();
        }
    })
}

function loadBarChart(id, data) {
    var chartColors = getChartColorsArray(id);

    var seriesOptions = data.series.map(function (series) {
        return {
            name: series.name,
            type: series.type,
            data: series.data,
            unit: series.unit,
        };
    });


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
        yaxis: seriesOptions.map(function (series, index) {

            return {

                title: {
                    text: series.name,

                },
                opposite: index % 2 !== 0, // Naprzemienne osi y
                labels: {
                    formatter: function (value) {
                        if (series.unit == 'PLN') {
                            return value.toLocaleString('pl-PL', { style: 'currency', currency: 'PLN', minimumFractionDigits: 0 });

                        }
                        else return value
                    }
                },
            };

        })
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}