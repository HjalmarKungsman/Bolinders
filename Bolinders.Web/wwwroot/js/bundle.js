$(document).ready(function () {

    $('.share').on('click', function (e) {
        e.preventDefault();

        var email = $('#email').val();
        var id = $('#vehicleId').val();
        var data = new FormData();
        var url = window.location.protocol + '//' + window.location.host;

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

    $('.equipment').select2({
        tags: true
    });
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
    var url = window.location.protocol + '//' + window.location.host;
    $.ajax({
        url: url + '/Bilar/RemoveImage',
        type: 'POST',
        data: {
            imageId: imgId,
            imagelink: imgUrl
        },
        error: function () {
            console.log("error");
        }
    });
}
$(document).ready(function () {
    // Instantiate the Bootstrap carousel
    $('.multi-item-carousel').carousel({
        interval: false
    });

    // for every slide in carousel, copy the next slide's item in the slide.
    // Do the same for the next, next item.
    $('.multi-item-carousel .item').each(function () {
        var next = $(this).next();
        if (!next.length) {
            next = $(this).siblings(':first');
        }
        next.children(':first-child').clone().appendTo($(this));

        if (next.next().length > 0) {
            next.next().children(':first-child').clone().appendTo($(this));
        } else {
            $(this).siblings(':first').children(':first-child').clone().appendTo($(this));
        }
    });
});
var modal = document.getElementById('imageModal');
var modalImg = document.getElementById("modalImage");

$('.myImg').on('click', function () {
    modal.style.display = "block";
    modalImg.src = this.src;
});

$(modal).on('click', function () {
    modal.style.display = "none";
});

$('close').on('click', function () {
    modal.style.display = "none";
});
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
$(document).ready(function () {
    function calculateCost() {

        var interest = 0.045;
        var length = parseInt($("#loanLenght").val());
        var price = parseFloat($("#exclVatPrice").val());
        var payment = parseFloat($("#downPayment").val());


        //Loan calculator
        var totalLoan = price - payment;
        var totalInterest = totalLoan * interest;
        var totalCost = totalLoan + totalInterest;
        var monthlyCost = (totalCost / length);
        var totalLoanCost = totalLoan + totalInterest + payment;

        //Update field
        $("span.monthlyPrice").text(Math.round(monthlyCost).toLocaleString('sv') + " kr");
        $("span.totalPrice").text(Math.round(totalLoanCost).toLocaleString('sv') + " kr");


    }
    calculateCost();
    $("#leasingCalculator").on("submit", function (e) {
        calculateCost();
        e.preventDefault();
    });
    $('#downPayment').on('input',function () {
        calculateCost();
    });
    $('#downPaymentRange').on('change', function () {
        calculateCost();
    });
    $('#loanLenght').change(function () {
        calculateCost();
    });

    //Range <=> Input
    //$('#downPaymentRange').on('input', function () {
    //    $('#downPayment').val($(this).val());
    //});
    //$('#downPayment').on('input', function () {
    //    $('#downPaymentRange').val($(this).val());
    //});
});
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

var result = [];
var articles = [];
var count = 0;
var hr;
//newsSettings.url + "?source=" + newsSettings.source + "&sortBy=" + newsSettings.sortBy + "&apiKey=" + newsSettings.apiKey
$.getJSON("https://newsapi.org/v1/articles?source=the-new-york-times&sortBy=top&apiKey=c28affb30c8842d78eedfd209f451423", function (data) {
    result = data;

    $.each(result.articles, function (i, ob) {

        var shortText = jQuery.trim(ob.description).substring(0, 50)
            .split(" ").slice(0, -1).join(" ") + "...";
        ob.description = shortText;

        var dateObj = new Date(ob.publishedAt);
        var month = ('0' + (dateObj.getMonth() + 1)).slice(-2);
        var date = ('0' + dateObj.getDate()).slice(-2);
        var year = dateObj.getFullYear();
        var shortDate = year + '-' + month + '-' + date;
        ob.publishedAt = shortDate;

        articles.push(ob);
    });

    articles.sort(function (a, b) {
        var c = new Date(a.publishedAt);
        var d = new Date(b.publishedAt);
        return d - c;

    });

    if (articles.length > 3) {
        articles.length = 3;
    }

    $.each(articles, function (i, obj) {
        count += 1;
        var new_html = "<h5> <strong>" + obj.title + "</h5> </strong>"
            + "<p>" + obj.description + "</p>"
            + '<span class="small">' + obj.publishedAt + '</span>'
            + '<a class="pull-right target=" _blank href="' + obj.url + '">' + "Läs mer!" + '</a>'
            + (count < 3 ? '<hr />' : '');

        $('#nyheter').append($('<ul class="list-unstyled" style="-webkit-padding-start: 0px; list-style: none;"><li></li></ul>').html(new_html));
    });
});

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