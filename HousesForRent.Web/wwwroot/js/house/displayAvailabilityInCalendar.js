
$(document).ready(function () {

    var selectedHouses = [];
    // Pobiera id zaznaczonych domów z formularza:
    $('input[name="selectedHouses"]:checked').each(function () {
        selectedHouses.push($(this).val());
    });
    loadBookingData(selectedHouses);

    //po kliknięciu i przesłaniu formularza ponowne załadowanie kalendarza:
    $('#submitForm').click(function () {
        selectedHouses = [];
        $('input[name="selectedHouses"]:checked').each(function () {
            selectedHouses.push($(this).val());
        });

        loadBookingData(selectedHouses);

    });

    var maxSelections = 8; //maksymalna liczba wyboru
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


            displayCalendar("calendar", data, selectedHouses);

            $(".chart-spinner").hide();
        }
    })
}
function displayCalendar(id, data, selectedHouses) {

    var $calendar = $('#calendar');

    if ($calendar.data('fullCalendar')) {
        $calendar.fullCalendar('destroy');
    }

    //Przypisanie kolorów do wybranych domów (selectedHouses) w houseColorMap
    var houseColorMap = {};
    for (var i = 0; i < selectedHouses.length; i++) {
        var houseId = selectedHouses[i];
        var colorIndex = i % Object.keys(resourceColors).length;
        var color = resourceColors[colorIndex + 1]; // Pobranie koloru z resourceColors
        houseColorMap[houseId] = color;
    }

    $('#calendar').fullCalendar({
        timeZone: 'UTC',
        initialView: 'dayGridMonth',
        events: data,
        eventRender: function (event, element) {
            if (event.resourceId) {
                var color = houseColorMap[event.resourceId];
                if (color) {
                    element.css('background-color', color);
                } else {
                    element.css('background-color', 'grey');
                }
            }
        },

        eventClick: function (info) {
            var eventId = info.id;
            window.location.href = '/booking/bookingDetails?bookingId=' + eventId;
        }
    });
}

//zestaw kolorów
var resourceColors = {
    '1': '#4682B4',
    '2': '#2E8B57',
    '3': '#708090',
    '4': '#FFA500',
    '5': '#9932CC',
    '6': '#FF69B4',
    '7': '#BC544B',
    '8': '#373737',
};


