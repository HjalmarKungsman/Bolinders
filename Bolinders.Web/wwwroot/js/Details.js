﻿$(document).ready(function () {
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
        var totalCost = totalLoan + totalInterest + payment;

        //Update field
        $("span.monthlyPrice").text(Math.round(monthlyCost).toLocaleString('sv') + " kr");
        $("span.totalPrice").text(Math.round(totalCost).toLocaleString('sv') + " kr");
    }
    calculateCost();
    $("#leasingCalculator").on("submit", function (e) {
        calculateCost();
        e.preventDefault();
    });
    $('#downPayment').on('input',function () {
        calculateCost();
    });
    $('#loanLenght').change(function () {
        calculateCost();
    });
});