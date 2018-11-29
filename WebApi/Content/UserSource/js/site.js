$(document).ready(function () {
    //ajax模拟表单
    var getToken = $("#explore");
    getToken.on("click", function () {
        var token = GetQueryString("token");
        if (token == null && token == "" || token == "null") {
            if (confirm('非法操作，请登录授权！?')) {
                window.location.href = "/Home/Index";
            } else {
                window.location.href = "/Home/Index";
            }
        } else {
            $("#input_apiKey").val(token);
            alert("启用token成功！")
        }
    })
});

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}