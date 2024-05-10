
$(document).ready(function () {
    loadTotalIncomeRadialChart();
});

function loadTotalIncomeRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalIncomeChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector("#spanTotalIncome").innerHTML = formatCurrency(data.totalCount);

            var sectionCurrentCount = document.createElement("span");
            if (data.monthlyChange > 0) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = ' + ' + formatCurrency(data.countCurrentMonth)+ '</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = ' ' + formatCurrency(data.countCurrentMonth) + '</span>';
            }

            document.querySelector("#sectionIncome").append(sectionCurrentCount);
            document.querySelector("#sectionIncome").append("since last month");

            loadRadialBarChart("totalIncomeRadialChart", data);

            $(".chart-spinner").hide();
        }
    })
}


function formatCurrency(amount) {
    // Formatowanie liczby jako waluty
    return amount.toLocaleString('pl-PL', {maximumFractionDigits: 2 }) + ' PLN';
}