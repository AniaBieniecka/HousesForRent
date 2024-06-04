
$(document).ready(function () {
    loadTotalBookingRadialChart();
});

function loadTotalBookingRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalBookingChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector("#spanTotalBookingCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("strong");
            document.querySelector("#sectionBookingCount").innerHTML += "This month bookings: ";
            document.querySelector("#sectionBookingCount").append(sectionCurrentCount);
            sectionCurrentCount.innerHTML = data.countCurrentMonth + ' </strong>';

            document.querySelector("#totalBookingRadialChart").innerHTML += "Monthly change: ";
            loadRadialBarChart("totalBookingRadialChart", data);

            $(".chart-spinner").hide();
        }
    })
}