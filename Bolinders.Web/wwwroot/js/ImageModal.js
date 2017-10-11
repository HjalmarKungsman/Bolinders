var modal = document.getElementById('imageModal');
var modalImg = document.getElementById("modalImage");

$('.myImg').on('click', function () {
    modal.style.display = "block";
    modalImg.src = this.src;
})

$(modal).on('click', function () {
    modal.style.display = "none";
});

$('close').on('click', function () {
    modal.style.display = "none";
});