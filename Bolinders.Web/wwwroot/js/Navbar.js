$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() <= 25) {
            $('.navbar').addClass('transparent-header');
        }
        else {
            $('.navbar').removeClass('transparent-header');
        }
    });

    $('.navbar-toggle').on("click", function () {
        if ($('.navbar-collaspse').hasClass('in')) {
            $('.navbar').addClass('transparent-header');
        }
        else {
            $('.navbar').removeClass('transparent-header');
        }
    });

});