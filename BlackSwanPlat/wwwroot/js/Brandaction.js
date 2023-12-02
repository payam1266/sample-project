$(function () {

    Select()


    let btnAdd = $("#btnAdd")
    btnAdd.click(function () {

        $.post('/Admin/BrandInsertConfirm', { name: $("#name").val(), date: $("#date").val(), description: $("#description").val() }, function (value) {
            $("#name").val(null)
            $("#date").val(null)
            $("#description").val(null)

            $("#spancityupdate").attr("hidden", true)
            $("#spancitydellet").attr("hidden", true)
            $("#spancityinsert").attr("hidden", false)
            $("input").click(function () {
                $("#spancityinsert").attr("hidden", true)
            })
            Select()
        })

    })

})
let i = 1
let tbl = $("#tbl")
function Select() {
    $("#tbl").html(null)

    $.post("/Admin/BrandSelect", function (value) {
        let th = `<thead  class=" bg-info text-light"> <tr ><th>#</th><th>NAME</th><th>DATE CREATED</th><th>DESCRIPTION</th><th>Delete</th><th>Edit</th><tr></thead>`
        tbl.append(th)
        let tbody = "<tbody></tbody>"

        tbl.append(tbody)
        $("#countNumber").text(value.length);

        value.forEach((brand, index) => {
            let tr = `<tr><td>${i}</td>
                            <td>${brand.name}</td>
                            <td>${brand.date}</td>
                            <td>${brand.description}</td>
                                      <td><button class="btn btn-outline-danger" style="border-radius:7px;font-size:12px;width:100px" onclick="F1(${brand.id})">Delete</button></td>
                                          <td><button class="btn btn-outline-secondary" style="border-radius:7px;font-size:12px;width:100px" onclick="F2(${brand.id})">Edit</button></td>
                           </tr>`
            tbl.append(tr)
            i++
        })
    })

}
function F1(id) {
    $.post("/Admin/BrandDelete", { id: id }, function (value) {
        $("#spancityinsert").attr("hidden", true)
        $("#spancityupdate").attr("hidden", true)
        $("#spancitydellet").attr("hidden", false)
        $(".form-control").click(function () {
            $("#spancitydellet").attr("hidden", true)

        })
        Select();
    })
}
let objectid;
function F2(id) {
    objectid = id;
    $.post("/Admin/BrandEdit", { id: id }, function (value) {
        $("#name").val(value.name);
        $("#date").val(value.date);
        $("#description").val(value.description);
        $("#spancityupdate").attr("hidden", true);
        $("#spancitydellet").attr("hidden", true);
        $("#spancityinsert").attr("hidden", true);

        $("#hinsert").attr("hidden", "hidden");
        $("#hupdate").attr("hidden", false);
        $("#btnUpdate").attr("hidden", false);
        $("#btnAdd").attr("hidden", "hidden");
    })
}
$("#btnUpdate").click(function () {
    $.post('/Admin/BrandUpdate', { id: objectid, name: $("#name").val(), date: $("#date").val(), description: $("#description").val() }, function (value) {
        $("#spancityupdate").attr("hidden", false);
        $("#spancitydellet").attr("hidden", true);
        $("#spancityinsert").attr("hidden", true);
        $("#hinsert").attr("hidden", false);
        $("#hupdate").attr("hidden", true);
        $("#name").val(null);
        $("#date").val(null);
        $("#description").val(null);
        $("#btnUpdate").attr("hidden", true);
        $("#btnAdd").attr("hidden", false);
        $(".form-control").click(function () {
            $("#spancityupdate").attr("hidden", true)
        })
        Select();
    })
})
