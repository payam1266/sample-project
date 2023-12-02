$(function () {

    $(".editbtn").click(function () {
        $(".edit").prop("readonly", false);
        $(".editbtn").hide();
        $(".uodatebtn").fadeIn(400);
    });





    $(".uodatebtn").click(function () {
        let userid = $(this).data("userid");
        let username = $("#username").val();
        let firstname = $("#firstname").val();
        let lastname = $("#lastname").val();
        let phone = $("#phone").val();
        let city = $("#city").val();
        let age = $("#age").val();
        $.post("/Account/UpdateUserProfile", { id: userid, firstname: firstname, lastname: lastname, PhoneNumber: phone, city: city, age: age, username: username }, function (value) {
            $("#popup p").text("Edit Your Profile Successful.");

            $("#popup").fadeIn(500);

            $("#popup").delay(3000).fadeOut(500);

            $(".edit").prop("readonly", true);
            $(".uodatebtn").hide();
            $(".editbtn").show();
        });
    });
    $("#change").click(function (event) {

        $(".changepass").fadeIn(500);
    });

    $("#btnclose1").click(function (event) {

        $(".changepass").fadeOut(500);
    });



    $("#changePasswordForm").submit(function (event) {
        event.preventDefault();

        let currentPassword = $("#currentPassword").val();
        let newPassword = $("#newPassword").val();
        let confirmPassword = $("#confirmPassword").val();


        if (newPassword !== confirmPassword) {
            alert("New password and confirm password do not match.");
            $("#confirmPassword").val(null);
            return;
        }


        $.post("/Account/ChangePassword", { currentPassword: currentPassword, newPassword: newPassword }, function (value) {
            alert(value);
            $("#currentPassword").val(null);
            $("#newPassword").val(null);
            $("#confirmPassword").val(null);
        });
    });

    $("#btnclose5").click(function (event) {

        $("#divselect3").fadeOut(500);
    });




    $("#orders").click(function () {
        let i = 1;
        let userid = $(".uodatebtn").data("userid");
        $.post("/ShoppingCard/GetOrderUser", { userid: userid }, function (value) {

            let tbl = $("#tbl3");


            $("#tbl3").html(null);

            $("#divselect").fadeIn(500);

            let th = `<thead class="  text-light"style='background-color:#055160;height:40px;'><tr><th>#</th><th>FullNmae</th><th>Phone</th><th>Address</th><th>City</th><th>PaymentMeyhod</th><th>Date</th><th>Total</th><th>Details</th><th>Cancel</th><tr></thead>`
            tbl.append(th)
            let tbody = "<tbody></tbody>"

            tbl.append(tbody)

            value.forEach((p, index) => {


                let tr = `<tr><td>${i}</td>
                                                                                                                                                                <td>${p.fullname}</td>
                                                                                                                                                                <td>${p.phone}</td>
                                                                                                                                                              <td>${p.address}</td>
                                                                                                                                                                <td>${p.city}</td>
                                                                                                                                                                    <td>${p.paymentmethod}</td>

                                                                                                                                                                            <td>${p.dateTime}</td>
                                                                                                                                                                                <td>$${p.totalPrice}</td>


                                                                                                                                                                                              <td><button  class="btn  btn-outline-primary  btn-edite" style="width:150px"  onclick="F2(${p.id})">Details</button></td>
                                                                                                                                                                                              <td><button  class="btn  btn-outline-danger  btn-edite" style="width:150px"  onclick="F4(${p.id})">Cancel</button></td>
                                                                                                                                                           </tr>`
                tbl.append(tr)
                i++

            })


        });

    });
    $("#btnclose2").click(function (event) {

        $("#divselect").fadeOut(500);
    });


    $("#favorite").click(function () {
        $("#productContainer").html(null);
        let userId = $("#inpuid").val();
        $.post("/Home/GetProductsLikedByUser", { userId: userId }, function (value) {
            console.log(value);

            if (userId != null) {
                let pdoducthead = `<h2 class="text-center s col-6 offset-3" style="text-align:center;padding-bottom:5px;font-family: 'Work Sans', sans-serif;font-weight:700;font-size:32px;margin-top:40px;color:#212529"><span style="border-bottom:4px solid;border-color:#055160;padding:3px;color:#212529">Your</span> Favorite Product</h2>`
                $("#productContainer").append(pdoducthead);
            }

            $.each(value, function (index, product) {

                console.log(product.images[0].productid)
                console.log(product.name)

                let productCard = `

                                                            <div class="card h-100 shadow col-3 m-5 p-4 justify-content-evenly cardproduct">
                                                                        <img src="data:image/png;base64,${product.images[0].img}"  class="card-img-top img-fluid custum-bg" alt="${product.name}">
                                                        <div class="card-body p-2">
                                                                             <div class="card-title titleproduct">${product.name}</div>
                                                                                              <div class="card-title brandproduct">Brand : ${product.brand}</div>
                                         <div class="card-title detproduct">Color : ${product.color}</div>
                                                                            <div class="card-title detproduct ">Size : ${product.size}</div>
                                                                            <p class="card-text desproduct"> ${product.description}</p>


                                 <div class="card-footer custom-footer" style="padding:8px 6px">
                                                                    <div class="float-start priceproduct">
                                                                                <h2>$${product.price}</h2>

                                                                    </div>


                                                                             <div class="float-end">
                                                                                            <a  class="btn btnwatch flex-shrink-0"onclick="F5(this)">Watch Now</a>

                                                                    </div>


                                                        </div>
                                                    </div>

                                            `;


                $("#productContainer").append(productCard);

            });
        });


    });

});


function F2(id) {
    let k = 1;
    $.post("/ShoppingCard/GetOrderUserDetails", { id: id }, function (value) {


        let tbl2 = $("#tbl4");


        $("#tbl4").html(null);

        $("#divselect2").fadeIn(500);

        let th = `<thead class="  text-light"style='background-color:#9a1941;height:40px;'><tr><th>#</th><th>Name</th><th>Price</th><th>Quantity</th><th>Subtotal</th><tr></thead>`
        tbl2.append(th)
        let tbody2 = "<tbody style='font-family: 'Work Sans', sans-serif;'></tbody>"

        tbl2.append(tbody2)

        value.forEach((p, index) => {


            let tr2 = `<tr ><td>${k}</td>
                                                                                                                                                                    <td>${p.productName}</td>
                                                                                                                                                                <td>${p.productPrice}</td>
                                                                                                                                                                <td>${p.quantity}</td>
                                                                                                                                                                <td>${p.subtotal}</td>




                                                                                                                                                       </tr>`
            tbl2.append(tr2)
            k++

        })


    });


    $("#btnclose3").click(function (event) {

        $("#divselect2").fadeOut(500);
    });

}


function F4(id) {

    $.post("/ShoppingCard/CancelOrder", { id: id }, function (value) {

        if (value === "1") {
            alert("Your Order Is Canceled.")
        }

        if (value === "0") {
            alert("Cancel not successful.")
        }
    });

}


function F5() {
    window.location.href = "/Home/ProductPage";
}





