// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
var ctx = document.getElementById("myPieChart");
$(document).ready(function () {
    var list = [4];
    $.ajax({
        url: '/Statistic/Jsonstatis',
        dataType: "json",
        type: "POST",
        success: function (response) {
            $(response).each(function (i, e) {
                var so = e;
                list[i] = so;
            });
            var myPieChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ["Công Ty", "Nhà Trường", "Intern", "Khóa học"],
                    datasets: [{
                        data: [list[0], list[1], list[2],list[3]],
                        backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#99FFFF'],
                        hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf',],
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: false
                    },
                    cutoutPercentage: 80,
                },
            });
        }
    });
});

