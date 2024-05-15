
$(document).ready(function () {

    $('#submitForm').click(function () {
        // Pobiera wartości wybranych zasobów z formularza
        var selectedHouses = [];
        $('input[name="selectedHouses"]:checked').each(function () {
            selectedHouses.push($(this).val());
        });

        loadBookingData(selectedHouses);

    });

    var maxSelections = 5; //maksymalna liczba wyboru
    var $checkboxes = $('input[name="selectedHouses"]');
    $checkboxes.change(function () {
        var numChecked = $checkboxes.filter(':checked').length;
        if (numChecked >= maxSelections) {
            $checkboxes.not(':checked').prop('disabled', numChecked >= maxSelections);
        } else {
            $checkboxes.prop('disabled', false);
        }
    });
});

function loadBookingData(selectedHouses) {
    $(".chart-spinner").show();

    $.ajax({
        url: "/House/GetCalendarBookingData",
        type: 'GET',
        dataType: 'json',
        data: { selectedHouses: JSON.stringify(selectedHouses) }, // Przekazuje wybrane zasoby jako parametr

        success: function (data) {


            displayCalendar("calendar", data);

            $(".chart-spinner").hide();
        }
    })
}
function displayCalendar(id, data) {

    var $calendar = $('#calendar');

    if ($calendar.data('fullCalendar')) {
        $calendar.fullCalendar('destroy');
    }

    $('#calendar').fullCalendar({
        timeZone: 'UTC',
        initialView: 'dayGridMonth',
        events: data,
        outerWidth: "80%",
        eventRender: function (event, element) {
            if (event.resourceId && resourceColors[event.resourceId]) {
                element.css('background-color', resourceColors[event.resourceId]);
            } 
        }
    });

}

var resourceColors = {
    '1': 'red',
    '2': 'blue',
    '3': 'green',
};


