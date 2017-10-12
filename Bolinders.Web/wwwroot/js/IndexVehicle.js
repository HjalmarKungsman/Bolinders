 $(document).ready(function () {

        var content = '<p>Är du säker?</p> <a id="confirmRemove" ><i class="fa fa-check" style="font-size:36px;"></i></a><a id="noRemove" class="tab"><i class="fa fa-close" style="font-size:36px"></i></a>';

        $('#deleteButton').popover({
            animation: true,
            content: content,
            html: true,
            placement: 'auto right'
        });

        $('.selectedVehicles').on('change', function () {
            if ($('.selectedVehicles:checked').length > 0) {
                $('#deleteButton').removeClass('disabled');
            }
            else {
                $('#deleteButton').addClass('disabled');
                $('#deleteButton').popover('hide');
            }
        });
        
        $(document).on('click', '#confirmRemove', function () {

                var arrayId = new Array();
                var checkboxes = document.querySelectorAll('.selectedVehicles:checked');

                for (var i = 0; i < checkboxes.length; i++) {
                    arrayId.push(checkboxes[i].value);
                }
                $.ajax({
                    url: urlDelete,
                    type: 'POST',
                    traditional: true,
                    data: { selectedVehicles: arrayId },
                    error: function () {
                        console.log("error");
                    }
                }).done(function () {
                    location.reload();
                });

             });
        $(document).on('click', '#noRemove', function () {
            $('#deleteButton').popover('hide');
        });
    });