//document.getElementsByClassName('btn-task').onclick = function () {
$('.btn-task').click(function (e) {
    // Khai báo tham số
    e.preventDefault();
    var checkbox = document.getElementsByName('dschon');
    var listTask = [];
    var count = 0;
    var id = $(this).data('id');
    // Lặp qua từng checkbox để lấy giá trị
    for (var i = 0; i < checkbox.length; i++) {
        if (checkbox[i].checked === true) {
            
            listTask[count] = checkbox[i].value;
            count++;
        }
    }
    $.ajax({
        url: '/QLTask/AddTask',
        traditional: true,
        data: {
            listTask: listTask,
            id: id,
        },
        type: "GET",
        success: function (response) {
            if (response == 'false') {
                alert("Thêm Bài học thất bại");
            } else {
                alert("Thêm Bài học thành công");
            }
        }
    });
});
