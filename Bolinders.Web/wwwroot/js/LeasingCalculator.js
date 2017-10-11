$(document).ready(function () {
    function calculateCost() {
        var interest = 0.045;
        var length = parseInt($("#loanLenght").val());
        var price = parseFloat($("#inklVatPrice").val());
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