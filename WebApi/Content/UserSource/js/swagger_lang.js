'use strict';

/**
 * Translator for documentation pages.
 *
 * To enable translation you should include one of language-files in your index.html
 * after <script src='lang/translator.js' type='text/javascript'></script>.
 * For example - <script src='lang/ru.js' type='text/javascript'></script>
 *
 * If you wish to translate some new texsts you should do two things:
 * 1. Add a new phrase pair ("New Phrase": "New Translation") into your language file (for example lang/ru.js). It will be great if you add it in other language files too.
 * 2. Mark that text it templates this way <anyHtmlTag data-sw-translate>New Phrase</anyHtmlTag> or <anyHtmlTag data-sw-translate value='New Phrase'/>.
 * The main thing here is attribute data-sw-translate. Only inner html, title-attribute and value-attribute are going to translate.
 *
 */
window.SwaggerTranslator = {
    _words: [],

    translate: function () {
        var $this = this;
        $('[data-sw-translate]').each(function () {
            $(this).html($this._tryTranslate($(this).html()));
            $(this).val($this._tryTranslate($(this).val()));
            $(this).attr('title', $this._tryTranslate($(this).attr('title')));
        });
    },

    _tryTranslate: function (word) {
        return this._words[$.trim(word)] !== undefined ? this._words[$.trim(word)] : word;
    },

    learn: function (wordsMap) {
        this._words = wordsMap;
    },
    _setControllerSummary: function () {
        $.ajax({
            type: "get",
            async: true,
            url: $("#input_baseUrl").val(),
            dataType: "json",
            success: function (data) {
                var summaryDict = data.ControllerDesc;
                var id, controllerName, strSummary;
                $("#resources_container .resource").each(function (i, item) {
                    id = $(item).attr("id");
                    if (id) {
                        controllerName = id.substring(9);
                        strSummary = summaryDict[controllerName];
                        if (strSummary) {
                            $(item).children(".heading").children(".options").prepend('<li class="controller-summary" title="' + strSummary + '">' + strSummary + '</li>');
                        }
                    }
                });
            }
        });
    },
    _translator2Cn: function () {
        var $this = this;
        if ($("#resources_container .resource").length > 0) {
            $this.translate()
        }
        if ($("#explore").text() == "绑定" && $this.iexcute < 500) {
            this.iexcute++;
            setTimeout($this._translator2Cn, 50);
        }
    }
};


/* jshint quotmark: double */
window.SwaggerTranslator.learn({
    "Warning: Deprecated": "警告：已过时",
    "SwaggerUi": "Web端接口",
    "Implementation Notes": "实现备注",
    "Response Class": "响应类",
    "Status": "状态",
    "Parameters": "参数",
    "Parameter": "参数",
    "Value": "值",
    "Example Value":"示例值",
    "Description": "描述",
    "Parameter Type": "参数类型",
    "Data Type": "数据类型",
    "Response Messages": "响应消息",
    "HTTP Status Code": "HTTP状态码",
    "Reason": "原因",
    "Response Model": "响应模型",
    "Request URL": "请求URL",
    "Response Body": "响应体",
    "Response Code": "响应码",
    "Response Headers": "响应头",
    "Hide Response": "隐藏响应",
    "Headers": "头",
    "Try it out!": "提交",
    "Show/Hide": "显示/隐藏",
    "List Operations": "显示操作",
    "Expand Operations": "展开操作",
    "Raw": "原始",
    "can't parse JSON.  Raw result": "无法解析JSON. 原始结果",
    "Model Schema": "模型架构",
    "Model": "模型",
    "apply": "应用",
    "Username": "用户名",
    "Password": "密码",
    "Terms of service": "服务条款",
    "Created by": "创建者",
    "See more at": "查看更多：",
    "Contact the developer": "联系开发者",
    "Response Content Type": "响应类型",
    "fetching resource": "正在获取资源",
    "fetching resource list": "正在获取资源列表",
    "Explore": "启用token",
    "Show Swagger Petstore Example Apis": "显示 Swagger Petstore 示例 Apis",
    "Can't read from server.  It may not have the appropriate access-control-origin settings.": "无法从服务器读取。可能没有正确设置access-control-origin。",
    "Please specify the protocol for": "请指定协议：",
    "Can't read swagger JSON from": "无法读取swagger JSON于",
    "Finished Loading Resource Information. Rendering Swagger UI": "已加载资源信息。正在渲染Swagger UI",
    "Unable to read api": "无法读取api",
    "from path": "从路径",
    "Click to set as parameter value": "点击设置参数",
    "server returned": "服务器返回"
});


$(function () {
    document.title = "易去对外Api";
    var info_title = document.getElementsByClassName('info_title')[0];
    info_title.innerHTML = "Web端接口";
    var footer = document.getElementsByClassName('footer')[0];
    footer.innerHTML = "<p>&copy; 2018易去旅行社成都分社</p>";
    $('body').append('<style type="text/css">.controller-summary{color:#10a54a !important;word-break:keep-all;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;max-width:250px;text-align:right;cursor:default;} </style>');
    $("#logo").html("易去对外Api").attr("href", "/Home/Index");
    window.SwaggerTranslator.translate();

    window.SwaggerTranslator._setControllerSummary();
});
