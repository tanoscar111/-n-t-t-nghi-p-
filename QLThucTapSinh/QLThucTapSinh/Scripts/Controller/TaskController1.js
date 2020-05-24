
$(document).ready(function () {
    var video = $('.btn-active').data('video');
    var taskID;
    $('.video').attr('src', video);
    $('.baitest').hide();

    $('.btn-active').click(function (e) {
        e.preventDefault();
        $('.baitest').hide();
        video = $(this).data('video');
        $('.video').attr('src', video);
        var note = $(this).data('note');
        $('.noidunghoc').text(note);
        taskID = $(this).data('id');
        var bai = $(this).data('bai');
        var dem = $(this).data('dem');
        if (bai == dem) {
            $('.baitest').show();
        }
    });

    $('.baitest').click(function (e) {
        e.preventDefault();
        $.ajax({
            type: 'GET',
            url: '/QLTest/Question',
            data: { taskid: taskID},
            success: function (result) {
                $('.from1').html(result);
            }
        });
    });

   

});
