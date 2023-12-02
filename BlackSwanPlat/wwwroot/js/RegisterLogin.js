const container = document.getElementById('container');
const overlayCon = document.getElementById('overlaycon');
const overlayBtn = document.getElementById('overlayBtn');

overlayBtn.addEventListener("click", () => {
    container.classList.toggle('right-panel-active');
});


$(function () {


    $("#btnforget").click(function () {
        $("#selectrole").hide();
        $("#pass").hide();
        $(this).text("Please Enter Your Email");
        $(".btnsignin").hide();
        $("#gettoken").show();



    })

    $("#gettoken").click(function () {
        $.post("/Account/ResetPassword", { email: $("#useremail").val() }, function (value) {

            $("#datadiv").show();
            $("#tokeninp").val(value);
            $("#gettoken").hide();
            $("#btnreset").show();
            $("#btnforget").text("Please Enter New Password.")

        })

    })
    $("#btnreset").click(function () {

        if ($("#pass1").val() != $("#conpass").val()) {
            alert("New password and confirm password do not match.");
            $("#conpass").val(null);
            return;
        }

        $.post("/Account/ResetPasswordConfirm", { Email: $("#useremail").val(), Token: $("#tokeninp").val(), Password: $("#pass1").val() }, function (value) {
            alert(value);
            $(".btnsignin").val(null);
            location.reload();
        })
    })
})