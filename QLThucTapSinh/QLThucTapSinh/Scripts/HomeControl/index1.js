var modal = document.getElementById('id01');
var modal1 = document.getElementById('id02');
const password1 = document.getElementById('Password');
const password = document.getElementById('Newpassword');
const toggle = document.getElementById('toggle');
const toggle2 = document.getElementById('toggle2');
const passwordAgain = document.getElementById('passwordAgain');
var pass1 = document.getElementById('Newpassword');
var pass2 = document.getElementById('passwordAgain');

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
var modal = document.getElementById('id02');
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

$(function () {
    $('.datepicker').datepicker({
        dateFormat: "dd-mm-yy",
        changeMonth: true,
        changeYear: true,
        yearRange: '1970:2030',
        showOn: "both",
        buttonText: "<i class='fa fa-calendar'></i>"
    });
});

function showHide() {
    if (password.type === 'password') {
        password.setAttribute('type', 'text');
    } else {
        password.setAttribute('type', 'password');
    }
}
function showHide2() {
    if (passwordAgain.type === 'password') {
        passwordAgain.setAttribute('type', 'text');
    } else {
        passwordAgain.setAttribute('type', 'password');
    }
}
function kiểm
function kiemtra() {
    if (pass2.value !== pass1.value) {
        alert('Mật Khẩu Không Trùng Khớp');
    }
}

$(document).ready(function () {
    $("#passwordAgain").blur(function () {
        if (pass2.value !== pass1.value) {
            alert('Mật Khẩu Không Trùng Khớp');
        }
    });
});