$(document).ready(function () {
    $('#style-switcher').toggle("style-switcher-panel");    
    $('.back-to-top').click(function () {   
        $('#style-switcher').toggle("style-switcher-panel"); 
        $('.back-to-top').addClass('matdi');
       
    });
});
$(document).ready(function () {
    $('.closebtn').click(function () { 
        $('#style-switcher').toggle("style-switcher-panel"); 
        $('.back-to-top').removeClass('matdi'); 
    });
});