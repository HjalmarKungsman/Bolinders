$(document).ready(function () {
    $('.toggle-searchOptions').on('click', function () {

        var up = 'fa-angle-up';
        var down = 'fa-angle-down';
        var span = $('.toggle-searchOptions span').first();
 
        $('#moreSearchOptions').toggleClass('hidden');
        if (span.hasClass(down)) {
            span.removeClass(down);
            span.addClass(up);
        }
        else {
            span.removeClass(up);
            span.addClass(down);
        }
    });
});
