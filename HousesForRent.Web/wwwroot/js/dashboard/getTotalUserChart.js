
$(document).ready(function () {
    loadTotalUserRadialChart();
});

function loadTotalUserRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalUserChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            document.querySelector("#spanTotalUserCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("strong");
            document.querySelector("#sectionUserCount").innerHTML += "This month new users: ";
            document.querySelector("#sectionUserCount").append(sectionCurrentCount);
            sectionCurrentCount.innerHTML = data.countCurrentMonth + ' </strong>';

            document.querySelector("#totalUserRadialChart").innerHTML += "Monthly change: ";
            loadRadialBarChart("totalUserRadialChart", data);

            $(".chart-spinner").hide();

        }
    })
}