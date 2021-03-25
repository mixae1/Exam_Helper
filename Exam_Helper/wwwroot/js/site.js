// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.change-color').click(function () {
    // $(".card").addClass("border-info")
    let color = $(this).css("background-color").toString();
    console.log(color);
    $(".card").css("border-color",color)
    let bc = $(".card").css("border-color")
    console.log(bc);
});
