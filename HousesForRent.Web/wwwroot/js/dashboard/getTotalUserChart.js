
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

            var sectionCurrentCount = document.createElement("span");
            if (data.monthlyChange > 0) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = ' + ' + data.countCurrentMonth + '</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = ' ' + data.countCurrentMonth + '</span>';
            }

            document.querySelector("#sectionUserCount").append(sectionCurrentCount);
            document.querySelector("#sectionUserCount").append("since last month");

            loadRadialBarChart("totalUserRadialChart", data);

            $(".chart-spinner").hide();
        }
    })
}