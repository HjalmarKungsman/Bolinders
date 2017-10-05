$(document).ready(function () {
    function calculateCost() {
        const length = parseInt($(".col-lg-6 > .LeaseCalculator form select").val());
        const price = parseFloat($(".col-lg-6 > .LeaseCalculator form input[type=hidden]").val());
        const payment = parseFloat($(".col-lg-6 > .LeaseCalculator form input[type=number]").val());
        const monthlycost = Math.round((price - payment) / length * 12 * 1.045 / 12);
        const totalcost = monthlycost * length;
        $("span.monthlyPrice").text(monthlycost + " kr");
        $("span.totalPrice").text(totalcost + " kr");
    }
    $(".col-lg-6 > .LeaseCalculator form").on("submit", function (e) {
        calculateCost();
        e.preventDefault();
    });
});