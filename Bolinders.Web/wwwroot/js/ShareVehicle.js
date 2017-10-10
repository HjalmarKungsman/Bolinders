$(document).ready(function () {

    $('.share').on('click', function (e) {
        e.preventDefault;

        var email = $('#email').val();
        var id = $('#vehicleId').val();
        var data = new FormData();
        console.log(id);

        data.append("id", id);
        data.append("reciever", email);

        $.ajax({
            method: 'POST',
            url: '@Url.Action("ShareVehicle", "Vehicles")',
            data: data,
            contentType: false,
            processData: false
        })
            .done(function (response) {
                $('#statusMessage').html("<div class='alert alert-success'><strong>Skickat! </strong>" + response + "</div>");
            }).fail(function (response) {
                $('#statusMessage').html("<div class='alert alert-danger'><strong>Fel! </strong>" + response + "</div>");
            })
    });

    $('.shareButton').on("click", function () {
        $('#shareVehicle').toggleClass('hidden');

    });

    // Vad är detta?!?
    $('img').on('click', function () {

    });
});