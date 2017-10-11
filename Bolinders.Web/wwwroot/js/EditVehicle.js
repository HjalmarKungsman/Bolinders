$(document).ready(function () {
    //$('.equipment-list-item').on('click', function () {
    //    this.remove(this);
    //});

    //$('.newEquipmentItem').on('blur', function () {
    //    //TODO Fix or remove
    //    $('.newEquipment').val($('.newEquipmentItem').val());
    //    $('.newEquipment').val();

    //});

});
$('.equipment').select2({
    tags: true
});

var test = true;
$('.imgDeleteBox').on('click', function () {
    if (test) {
        $('.noRemove', this).removeClass('hidden');
        $('.yesRemove', this).removeClass('hidden');
        $('.fa-trash', this).addClass('hidden');
        $('.imgDeleteBox').addClass('red');
        test = false;
    }
});
$('.imgDeleteBox').hover(function () {
    $('.noRemove', this).addClass('hidden');
    $('.yesRemove', this).addClass('hidden');
    $('.fa-trash', this).removeClass('hidden');
    $('.imgDeleteBox').removeClass('red');
    test = true;
});
$('.noRemove').on('click', function () {
    var thiss = this.parentElement;
    $('.noRemove', thiss).addClass('hidden');
    $('.yesRemove', thiss).addClass('hidden');
    $('.fa-trash').removeClass('hidden');
    $('.imgDeleteBox').removeClass('red');
});

function deletepic(imgId, imgUrl, thiss, events) {
    $(events.path[5]).addClass('hidden');

    $.ajax({
        url: '@Url.Action("RemoveImage", "Vehicles")',
        type: 'POST',
        data: {
            imageId: imgId,
            imagelink: imgUrl
        },
        error: function () {
            console.log("error");
        }
    })
};