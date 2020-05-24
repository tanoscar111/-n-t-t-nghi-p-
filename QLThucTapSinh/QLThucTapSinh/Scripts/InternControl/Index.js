var gr = '@Request.RequestContext.HttpContext.Session["Role"]';
if (gr == '2') {
    $(document).ready(function () {
        $(".notIntership").hide();
    })
}
$(function () {
    $('#checkAll').change(function () {
        if ($(this).prop('checked') == true) {
            $('input[name=dschon]').prop('checked', true);
        } else {
            $('input[name=dschon]').prop('checked', false);
        }
    });
});
//document.getElementById('btn').onclick = function () {
//    // Khai báo tham số
//    var checkbox = document.getElementsByName('dschon');
//    var result = "";
//    // Lặp qua từng checkbox để lấy giá trị
//    for (var i = 0; i < checkbox.length; i++) {
//        if (checkbox[i].checked === true) {
//            result += ' [' + checkbox[i].value + ']';
//        }
//    }

//    // In ra kết quả
//    alert("Sở thích là: " + result);
//};
$(document).ready(function () {
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
            url: '/QLIntern/ChangeStatus',
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

    $('.btn-task').click(function (e) {
        // Khai báo tham số
        e.preventDefault();
        var checkbox = document.getElementsByName('dschon');
        var listIntern = [];
        var count = 0;
        var id = $(this).data('id');
        // Lặp qua từng checkbox để lấy giá trị
        for (var i = 0; i < checkbox.length; i++) {
            if (checkbox[i].checked === true) {

                listIntern[count] = checkbox[i].value;
                count++;
            }
        }
        $.ajax({
            url: '/QLIntern/AddIntern',
            traditional: true,
            data: {
                listIntern: listIntern,
                id: id,
            },
            type: "GET",
            success: function (response) {
                if (response == 'False') {
                    alert("Thêm Bài học thất bại");
                } else {
                    window.location.href = '/QLIntern/Index1';    
                    alert("Thêm Bài học thành công");
                }
            }
        });
    });
});