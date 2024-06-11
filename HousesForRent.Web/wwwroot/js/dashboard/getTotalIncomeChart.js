
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

            var sectionCurrentCount = document.createElement("strong");
            document.querySelector("#sectionIncome").innerHTML += "This month bookings: <br/>";
            document.querySelector("#sectionIncome").append(sectionCurrentCount);
            sectionCurrentCount.innerHTML = formatCurrency(data.countCurrentMonth) + ' </strong>';

            document.querySelector("#totalIncomeRadialChart").innerHTML += "Monthly change: ";
            loadRadialBarChart("totalIncomeRadialChart", data);

            $(".chart-spinner").hide();

        }
    })
}


function formatCurrency(amount) {
    // Formatowanie liczby jako waluty
    return amount.toLocaleString('pl-PL', {maximumFractionDigits: 2 }) + ' PLN';
}