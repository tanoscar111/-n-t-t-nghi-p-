
    const password = document.getElementById('password');
    const toggle = document.getElementById('toggle');
    const toggle2 = document.getElementById('toggle2');
    const passwordAgain = document.getElementById('passwordAgain');
    var pass1 = document.getElementById('password');
    var pass2 = document.getElementById('passwordAgain');
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
