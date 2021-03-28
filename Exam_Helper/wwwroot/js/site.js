// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".card").css("border-color",localStorage.getItem("bc"));
$(".main-blocks").css("border-color",localStorage.getItem("bc"))
$(".block").css("border-color",localStorage.getItem("bc"))
$(".test-card").css("border-color",localStorage.getItem("bc"))
$(".alert").css("background-color",localStorage.getItem("bc"))

$(".action-button").css("background-color",localStorage.getItem("bc"))

$('.change-color').click(function () {
    // $(".card").addClass("border-info")
    let color = $(this).css("background-color").toString();
    console.log(color);
    $(".card").css("border-color",color)
    $(".main-blocks").css("border-color",color)
    $(".block").css("border-color",color)
    $(".action-button").css("background-color",color)
    $(".alert").css("background-color",color)
    
    $(".test-card").css("border-color",color)
    // $(".btn-outline-info").css("border-color",color)
    let bc = $(".card").css("border-color")
    console.log(bc);
    localStorage.setItem("bc",bc);
});
