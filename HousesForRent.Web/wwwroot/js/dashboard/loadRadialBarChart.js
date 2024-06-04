function loadRadialBarChart(id, data) {
    var chartColors = getChartColorsArray(id);
    var options = {
        fill: {
            colors: chartColors
        },
        chart: {
            height: 120,
            width: 120,
            type: "radialBar",
            sparkline: {
                enabled: true
            },
            offsetY: 0,
        },
        series: data.series,
        plotOptions: {
            radialBar: {
                hollow: {
                    margin: 15,
                    size: "40%"
                },
                dataLabels: {
                    value: {
                        offsetY: -10,
                    }
                }
            }
        },
        stroke: {
            lineCap: "round",
        },
        labels: [""]
    };
    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}

function getChartColorsArray(id) {
    if (document.getElementById(id) != null) {
        var colors = document.getElementById(id).getAttribute("data-colors");
        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;;
                }
            });
        }
    }
}