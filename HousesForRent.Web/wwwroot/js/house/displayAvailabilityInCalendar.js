
$(document).ready(function () {
    loadBookingData();
});



function loadBookingData() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/House/GetCalendarBookingData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {


            displayCalendar("calendar", data);

            $(".chart-spinner").hide();
        }
    })
}


function displayCalendar(id, data) {
    $('#calendar').fullCalendar({
        timeZone: 'UTC',
            initialView: 'dayGridMonth',
                events: data.events
    });

}


