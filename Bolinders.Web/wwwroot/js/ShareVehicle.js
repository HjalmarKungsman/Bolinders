$(document).ready(function () {

    $('.share').on('click', function (e) {
        e.preventDefault();

        var email = $('#email').val();
        var id = $('#vehicleId').val();
        var data = new FormData();
        var url = 'http://' + window.location.host;

        data.append("id", id);
        data.append("reciever", email);

        $.ajax({
            method: 'POST',
            url: url + '/Bilar/ShareVehicle/',
            data: data,
            contentType: false,
            processData: false
        })
            .done(function (response) {
                $('#statusMessage').html("<div class='alert alert-success'><strong>Skickat! </strong>" + response + "</div>");
            }).fail(function (response) {
                $('#statusMessage').html("<div class='alert alert-danger'><strong>Fel! </strong>" + response + "</div>");
            });
    });

    $('.shareButton').on("click", function (e) {
        e.preventDefault();
        $('#shareVehicle').toggleClass('hidden');

    });
});