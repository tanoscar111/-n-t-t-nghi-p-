$(document).ready(function () {
    var task;
    $("#myInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#myTable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $('.btn-active').off('click').on('click', function (e) {
        e.preventDefault();
        var btn = $(this)
        var id = btn.data('id');
        $.ajax({
            url: '/QLCompany/ChangeStatus',
            data: { id: id },
            dataType: "json",
            type: "POST",
            success: function (response) {
                if (response.status == true) {
                    btn.text('Kích hoạt');
                } else {
                    btn.text('Khóa');
                }
            }
        });
    });

    $('.idSort').mouseover(function () {
        task = $(this).data('ids');
    });

    $('.gender').change(function () {
        var valu = $(this).val();
        $.ajax({
            type: 'GET',
            data: {
                id: task,
                val: valu,
            },
            url: '/QLCompany/extension',
            success: function (result) {
                if (result == 'True') {
                    window.location.href = '/QLCompany/Index/';
                } else {
                    alert("Gia hạn thất bại");
                }
            }
        });
        //alert(sort + "id" + id);
    });
});