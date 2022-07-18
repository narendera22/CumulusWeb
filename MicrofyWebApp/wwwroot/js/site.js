// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var className = ".dynamic-field";
var count = 0;
var field = "";
var maxFields = 10;

var prjclassName = ".dynamic-field";
var prjcount = 0;
var prjfield = "";
var prjmaxFields = 5;

$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });


    //$(".anchor-link").on("click", function () {
    //    $('#search').val("");
    //    $(".anchor-link").removeClass("active");
    //    $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
    //    $(this).addClass("active");
    //    // $(this).toggleClass("active");

    //    var htmltemplate;
    //    if ($(this).siblings().length === 0) {
    //        //$(".anchor-link").siblings().removeClass('show');
    //        $(this).addClass('active');
    //        var thismenu = $(this).find('a').text();
    //        // var subtabmenu=$('#documentpnl').find('nav').find('#nav-tab').find('a.nav-item.nav-link.active').text();
    //        $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    //        htmltemplate = $('<li class="active"> <span>' + thismenu + '</span></li>');
    //        $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    //        var Phase = thismenu;
    //        Getdocuments(Phase);

    //    }
    //    else {
    //        $(this).siblings().addClass('show');
    //        $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    //        htmltemplate = $('<li class="active"> <span>Dashboard </span></li></ol>');
    //        $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    //        $(this).siblings().find('.subanchor-link').find('a').first().trigger('click');
    //        $(this).removeClass('active');
    //    }
    //});



    //$(".subanchor-link").on("click", function () {
    //    $('#search').val("");
    //    $(".anchor-link").removeClass("active");
    //    $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
    //    $(this).addClass('active');
    //    var phase = $(this).parent().parent().find('a.dropdown-toggle').text();
    //    var subphase = $(this).find('a').text();
    //    $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    //    var htmltemplate = $('<li><a href="#" onclick="subanchorlink()">' + phase + '</a></li><li class="active"> <span>' + subphase + '</span></li>');
    //    $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    //    Getdocuments(phase, subphase);


    //});
    //$(".homelink").on("click", function () {
    //    $("#newdocdiv").hide();
    //    $("#dashboard").show();
    //    $("#uploadDocument").hide();
    //    $("#documentpnl").hide();
    //    $(".anchor-link").removeClass("active");
    //    $(".anchor-link").siblings().removeClass('show');
    //    $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    //    var htmltemplate = $('<li class="active"> <span>Dashboard </span></li></ol>');
    //    $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);

    //});

});
function subanchorlink() {
    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;
    ViewDocuments(phase, subphase);
}
function GetActivePhase() {
    var phase = $(".anchor-link.active").find("a").text();
    var subphase = $(".subanchor-link.active").find("a").text();
    if (subphase != "") {
        phase = $(".subanchor-link.active").parent().parent().find('a.dropdown-toggle').text();
    }
    var Active = {
        phase: phase,
        subphase: subphase
    }
    return Active;
}
function Getdocuments(phase, subphase) {
    $.ajax({
        type: "GET",
        url: "/Repository/GetDocumentRepository",
        data: { "Phase": phase, "SubPhase": subphase },
        contentType: "application/json; charset=utf-8",
        //dataType: 'json',
        success: function (data) {
            //window.location.replace(data.newUrl);
            $("#documentpnl").html(data);
            //$("#newdocdiv").show();
            $("#dashboard").hide();
            $("#uploadDocument").hide();
            $("#SerachDocument").hide();
            $("#documentpnl").show();

        },
        error: function (data) {
            console.log(JSON.stringify(data));
        }
    });
}

function fn_cancel() {
    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;
    Getdocuments(phase, subphase);
    return false;
}

function DocumentSave(url, description) {
    debugger;
    var tag = [];
    var azuretag = [];
    var processtag = [];
    var technologytag = [];
    var tags = {};

    var name = $('#txtname').val();
    //var tags = $('#txtTags').val().split(",").map(function (value) {
    //    return value.trim();
    //});
    //$.each(txttags, function (i) {
    //    alert(array[i]);
    //});
    //tags.push(txttags);

    var $tags = $(className);

    $tags.each(function (key, value) {
        var tagstype = $("#tagsddl-" + (key + 1)).val()
        //var tagkey = $("#txtkey-" + (key + 1)).val();
        var tagval = $("#txtvalue-" + (key + 1)).val();
        if (tagval != "") {
            var AzureServices = {};
            var ProcessRelated = {};
            var TechnologyRelated = {};
            if (tagstype === "AzureService") {
                AzureServices[tagstype + (key + 1)] = tagval;
                azuretag.push(AzureServices);
            }
            else if (tagstype === "ProcessRelated") {
                ProcessRelated[tagstype + (key + 1)] = tagval;
                processtag.push(ProcessRelated);
            }
            else if (tagstype === "TechnologyRelated") {
                TechnologyRelated[tagstype + (key + 1)] = tagval;
                technologytag.push(TechnologyRelated);
            }
        }
    });
    tags = {
        "AzureService": azuretag,
        "ProcessRelated": processtag,
        "TechnologyRelated": technologytag
    };
    tag.push(tags);
    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;

    var documentDet = {
        "Phase": phase,
        "SubPhase": subphase,
        "DocumentName": name,
        "Description": description,
        "URL": url,
        "Tags": tag
    }
    var redirecturl = "/Repository/CreateDocument";
    $.post(redirecturl, documentDet, function (data) {
        //AlertMessage("success", "Document uploaded successfully");
        $.alert({
            title: 'Success!!',
            content: "Document uploaded successfully",
            type: 'green',
            onAction: function () {
                $("#documentpnl").html(data);
                //$("#newdocdiv").show();
                $("#dashboard").hide();
                $("#uploadDocument").hide();
                $("#SerachDocument").hide();
                $("#documentpnl").show();
            }
        });
    });
    return false;
}
function htmlEncode(s) {
    var ntable = {
        "&": "amp",
        "<": "lt",
        ">": "gt",
        "\"": "quot"
    };
    s = s.replace(/[&<>"]/g, function (ch) {
        return "&" + ntable[ch] + ";";
    })
    s = s.replace(/[^ -\x7e]/g, function (ch) {
        return "&#" + ch.charCodeAt(0).toString() + ";";
    });
    return s;
}




function UploadDocument(Documentname) {
    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;

    $.ajax({
        type: "GET",
        url: "/Repository/GetUploadPartial",
        data: { "phase": phase, "subphase": subphase, "documentname": Documentname },
        contentType: "application/json; charset=utf-8",
        //dataType: 'json',
        success: function (data) {
            //window.location.replace(data.newUrl);
            $("#uploadDocument").html(data);
            //$("#newdocdiv").hide();
            //$("#SerachDocument").hide();
            //$("#dashboard").hide();
            $("#uploadDocument").show();
            $("#documentpnl").hide();

        },
        error: function (data) {
            console.log(JSON.stringify(data));
        }
    });
}

function UploadNewDocument() {

    $.ajax({
        type: "GET",
        url: "/Repository/GetNewUploadPartial",
        data: {},
        contentType: "application/json; charset=utf-8",
        //dataType: 'json',
        success: function (data) {
            //window.location.replace(data.newUrl);
            $("#uploadDocument").html(data);
            //$("#newdocdiv").hide();
            //$("#SerachDocument").hide();
            //$("#dashboard").hide();
            $("#uploadDocument").show();
            $("#documentpnl").hide();

        },
        error: function (data) {
            console.log(JSON.stringify(data));
        }
    });
}

function SearchDocument() {
    $(".anchor-link").removeClass("active");
    $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
    //$('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    htmltemplate = "";
    //$('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    var search = $('#search').val();
    window.location.href = "/Repository/DocumentSearch?Search=" + search;

    //$.ajax({
    //    type: "GET",
    //    url: "/Repository/SearchDocumentPartial",
    //    data: { "Search": search },
    //    contentType: "application/json; charset=utf-8",
    //    //dataType: 'json',
    //    success: function (data) {
    //        //window.location.replace(data.newUrl);
    //        $("#SerachDocument").html(data);
    //        //$("#newdocdiv").hide();
    //        $("#SerachDocument").show();
    //        $("#dashboard").hide();
    //        $("#uploadDocument").hide();
    //        $("#documentpnl").hide();

    //    },
    //    error: function (data) {
    //        console.log(JSON.stringify(data));
    //    }
    //});

}

function ViewDocuments(phase, subphase) {
    if (subphase === null) {
        var activate = $(".anchor-link");
        $(activate).each(function () {
            var value = $(this).find("a").text();
            if (value === phase) {
                $(this).click();
            }
        });
    }
    else {
        var subactivate = $(".subanchor-link");
        $(subactivate).each(function () {
            var value = $(this).find("a").text();
            if (value === subphase) {
                $(this).parent().addClass('show');
                $(this).click();
            }
        });
    }
}

function addtags(e) {
    if (totalFields() != maxFields) {
        if (e.parent().data('tags') === 'hold') { return false; }
        else { e.parent().data('tags', 'hold'); }
        count = totalFields() + 1;
        field = $("#dynamic-field-1").clone();
        field.attr("id", "dynamic-field-" + count);
        //field.find("#txtkey-1").attr("id", "txtkey-" + count);
        field.find("#txtvalue-1").attr("id", "txtvalue-" + count);
        field.find("#tagsddl-1").val("AzureService");
        field.find("#tagsddl-1").attr("id", "tagsddl-" + count);
        field.attr('data-tags', 'add');
        field.find("input").val("");
        field.find("a.delete").show();
        $(className + ":last").after($(field));

    }
}

function totalFields() {
    return $(className).length;
}

function deletetags(e) {
    e.parent().remove();
    count = totalFields();
    if (count <= 1) { $(".dynamic-field").data('tags', 'add'); }
    else {
        $(className + ":last").data('tags', 'add')
    }
    return false;
}

function ViewTag() {
    if (totalFields() != maxFields) {
        count = totalFields() + 1;
        field = $("#dynamic-field-1").clone();
        field.attr("id", "dynamic-field-" + count);
        //field.find("#txtkey-1").attr("id", "txtkey-" + count);
        field.find("#txtvalue-1").attr("id", "txtvalue-" + count);
        field.find("#tagsddl-1").val("AzureService");
        field.find("#tagsddl-1").attr("id", "tagsddl-" + count);
        field.attr('data-tags', 'add');
        field.find("input").val("");
        field.find("a.delete").show();
        $(className + ":last").after($(field));

    }
}

function highlight(text) {
    var highlightRe = /<span class="highlight">(.*?)<\/span>/g,
        highlightHtml = '<span class="highlight">$1</span>';
    var inputText = document.getElementsByClassName("doctitle");
    $(inputText).each(function (key, value) {
        var txt = value.innerHTML.replace(highlightRe, '$1');
        if (text !== '') {
            txt = txt.replace(new RegExp('(' + text + ')', 'gi'), highlightHtml);
        }
        $(this).html(txt);
    });
    var docdescrip = document.getElementsByClassName("docdescrip");
    $(docdescrip).each(function (key, value) {
        var txt = value.innerHTML.replace(highlightRe, '$1');
        if (text !== '') {
            txt = txt.replace(new RegExp('(' + text + ')', 'gi'), highlightHtml);
        }
        var res = wordWrap(txt.trim(), 100);
        $(this).html(res);
    });
}

function wordWrap(str, maxWidth) {
    var newLineStr = "\n"; done = false; res = '';
    while (str.length > maxWidth) {
        found = false;
        // Inserts new line at first whitespace of the line
        for (i = maxWidth - 1; i >= 0; i--) {
            if (testWhite(str.charAt(i))) {
                res = res + [str.slice(0, i), newLineStr].join('');
                str = str.slice(i + 1);
                found = true;
                break;
            }
        }
        // Inserts new line at maxWidth position, the word is too long to wrap
        if (!found) {
            res += [str.slice(0, maxWidth), newLineStr].join('');
            str = str.slice(maxWidth);
        }

    }

    return res + str;
}

function testWhite(x) {
    var white = new RegExp(/^\s$/);
    return white.test(x.charAt(0));
};

function TextWrap() {
    var docdescrip = document.getElementsByClassName("docdescrip");
    $(docdescrip).each(function (key, value) {
        var txt = value.innerHTML;
        var res = wordWrap(txt.trim(), 100);
        $(this).html(res);
    });
}

function SearchTag(tag) {
    $('#search').val(tag);
    SearchDocument();
}

function UpdateMetadata(displaytags, usertags, filename) {
    debugger;
    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;
    var metadata = {
        "filename": filename,
        "displaytags": displaytags,
        "usertags": usertags,
        "phase": phase,
        "subphase": subphase,
        "flag":"Document"
    };

    var redirecturl = "/Repository/UpdateMetadata";
    $.post(redirecturl, metadata, function (data) {

    });
    return false;

}

function DeleteDocument(filename, documentname) {
    debugger;
    $.confirm({
        type: 'orange',
        title: 'Confirm!',
        content: 'Do you want to delete document ?',
        buttons: {
            confirm: function () {
                var active = GetActivePhase();
                var phase = active.phase;
                var subphase = active.subphase;
                var deletedoc = {
                    "filename": filename,
                    "documentname": documentname,
                    "phase": phase,
                    "subphase": subphase,
                    "flag":"Document"
                };

                var redirecturl = "/Repository/DeleteDocument";
                $.post(redirecturl, deletedoc, function (data) {
                    $.alert({
                        title: 'Success!!',
                        content: "Document deleted successfully",
                        type: 'green',
                        onAction: function () {
                            $("#documentpnl").html(data);
                            //$("#newdocdiv").show();
                            $("#dashboard").hide();
                            $("#uploadDocument").hide();
                            $("#SerachDocument").hide();
                            $("#documentpnl").show();
                        }
                    });
                });
            },
            cancel: function () {

            }
        }
    });

    return false;

}

$(function () {
    $("a[class='edit']").click(function () {
        debugger;
        var userid = $(this).closest("tr").find('td:eq(0)').text();
        var first_name = $(this).closest("tr").find('td:eq(1)').text();
        var role = $(this).closest("tr").find('td:eq(2)').text();
        //var cust_name = $(this).closest("tr").find('td:eq(3)').text();
        var proj_cus_name = $(this).closest("tr").find('td:eq(4)').text();
        var Status = $(this).closest("tr").find('td:eq(3)').find(".switch .togglecheckbox").is(":checked");
        var pwd = $(this).closest("tr").find('td:eq(5)').find(".pwd").val();
        var moduleaccess = $(this).closest("tr").find('td:eq(6)').text();


        var json = $.parseJSON(proj_cus_name);
        var module = $.parseJSON(moduleaccess);
        $('#userid').val(userid);
        $('#first_name').val(first_name);
        $('#role').val(role);
        //$('#cust_name').val(cust_name);
        //$('#project_name').val(project_name);
        $('#userid').val(userid);
        //json.each(function (key, value) {
        //    if (key == 0) {
        //        $('#project_name-1').val(value.ProjectName);
        //        $('#cust_name-1').val(value.CustomerName);
        //    }
        //    else {
        //        Viewprj();
        //        $('#project_name-' + (key + 1)).val(value.ProjectName);
        //        $('#cust_name-' + (key + 1)).val(value.CustomerName);
        //    }
        //});
        $('.dynamic-field').slice(1).remove();
        for (var key = 0; key < json.length; key++) {
            if (key == 0) {
                $('#project_name-1').val(json[key].projectName);
                $('#cust_name-1').val(json[key].customerName);
            }
            else {
                Viewprj();
                $('#project_name-' + (key + 1)).val(json[key].projectName);
                $('#cust_name-' + (key + 1)).val(json[key].customerName);
            }
        }
        $('.form-check-input').prop("checked", false);

        for (var key = 0; key < module.length; key++) {
            var $moduleaccess = $('.moduleAccess');
            $moduleaccess.each(function (i, value) {
                var modname = $("#chkmodule_" + (i + 1) + ".form-check-input").attr("name");
                if (modname == module[key].name) {
                    var visible = false;
                    if (module[key].visible == "true") {
                        visible = true;
                    }
                    $('#chkmodule_' + (i + 1)).prop("checked", visible);
                }
            });
            
        }
        if (Status) {
            $(".switch").find("#statusCheck").prop("checked", true);
        }
        else { $(".switch").find("#statusCheck").prop("checked", false); }
        $('#password').val(pwd);
        $("#UserEditPopup").modal("show");
        return false;
    });
    $("a[class='resetpwd']").click(function () {
        debugger;
        var userid = $(this).closest("tr").find('td:eq(0)').text();
        var first_name = $(this).closest("tr").find('td:eq(1)').text();
        var role = $(this).closest("tr").find('td:eq(2)').text();
        //var cust_name = $(this).closest("tr").find('td:eq(3)').text();
        var proj_cus_name = $(this).closest("tr").find('td:eq(4)').text();
        var Status = $(this).closest("tr").find('td:eq(3)').find(".switch .togglecheckbox").is(":checked");
        var pwd = $(this).closest("tr").find('td:eq(5)').find(".pwd").val();
        var Projects = $.parseJSON(proj_cus_name)
        var moduleaccess = $(this).closest("tr").find('td:eq(6)').text();
        var module = $.parseJSON(moduleaccess);

        //var Projects = [{
        //    "ProjectName": project_name,
        //    "CustomerName": cust_name
        //}];

        var UserDetails = {
            "username": userid,
            "password": "",
            "fullName": first_name,
            "userRole": role,
            "isActive": Status,
            "isDeleted": false,
            "projects": Projects,
            "moduleAccess": module
        }
        var ActivityTracker = {
            "UserName": userid,
            "ActivityType": "ResetPassword",
            "ActivityDetails": " has successfully reset password for " + userid
        }

        var redirecturl = "/Login/UpdateUser";
        $.post(redirecturl, { "users": UserDetails, "activityTracker": ActivityTracker }, function (data) {
            var title = "";
            var type = "";
            var content = "";
            if (data.statusCode == true) {
                title = "Success!!";
                type = "green";
                content = "Password Reset Successfully : " + data.defaultPassword;
            }
            else {
                title = "Error!!";
                type = "red";
                content = data.responseMessage;

            }
            $.alert({
                title: title,
                content: content,
                type: type,
                onAction: function () {
                    window.location.href = '/Settings/ListUsers';
                }
            });
        })
            .fail(function (response) {
                $.alert({
                    title: 'Error!!',
                    content: response,
                    alignMiddle: true,
                    type: 'red'
                });
            });
        return false;
    });

    $("a[class='deleteusr']").click(function () {
        var username = $(this).closest("tr").find('td:eq(0)').text();
        $.ajax({
            type: "GET",
            url: "/Login/DeleteUser",
            data: { "username": username },
            contentType: "application/json; charset=utf-8",
            //dataType: 'json',
            success: function (data) {
                var title = "";
                var type = "";
                var content = "";
                if (data == true) {
                    title = "Success!!";
                    type = "green";
                    content = "User Deleted Successfully";
                }
                else {
                    title = "Error!!";
                    type = "red";
                    content = "User deletion failed";

                }
                $.alert({
                    title: title,
                    content: content,
                    type: type,
                    onAction: function () {
                        window.location.href = '/Settings/ListUsers';
                    }
                });
            },
            error: function (data) {
                console.log(JSON.stringify(data));
            }
        });
        return false;
    });
});



function addprj(e) {
    if (prjtotalFields() != prjmaxFields) {
        if (e.parent().data('tags') === 'hold') { return false; }
        else { e.parent().data('tags', 'hold'); }
        prjcount = prjtotalFields() + 1;
        prjfield = $("#dynamic-PrjCus-field-1").clone();
        prjfield.attr("id", "dynamic-PrjCus-field-" + prjcount);
        prjfield.find("#cust_name-1").attr("id", "cust_name-" + prjcount);
        prjfield.find("#project_name-1").attr("id", "project_name-" + prjcount);
        prjfield.find('#cust_name-' + prjcount).removeAttr('required')
        prjfield.find('#project_name-' + prjcount).removeAttr('required')
        prjfield.attr('data-tags', 'add');
        prjfield.find("input").val("");
        prjfield.find("a.delete").show();
        $(prjclassName + ":last").after($(prjfield));

    }
}

function prjtotalFields() {
    return $(prjclassName).length;
}

function deleteprj(e) {
    e.parent().remove();
    prjcount = prjtotalFields();
    if (prjcount <= 1) { $(".dynamic-field").data('tags', 'add'); }
    else {
        $(prjclassName + ":last").data('tags', 'add')
    }
    return false;
}

function Viewprj() {
    if (prjtotalFields() != prjmaxFields) {
        prjcount = prjtotalFields() + 1;
        prjfield = $("#dynamic-PrjCus-field-1").clone();
        prjfield.attr("id", "dynamic-PrjCus-field-" + prjcount);
        //field.find("#txtkey-1").attr("id", "txtkey-" + count);
        prjfield.find("#cust_name-1").attr("id", "cust_name-" + prjcount);
        prjfield.find("#project_name-1").attr("id", "project_name-" + prjcount);
        prjfield.attr('data-tags', 'add');
        prjfield.find("input").val("");
        prjfield.find("a.delete").show();
        $(prjclassName + ":last").after($(prjfield));

    }
}
function funcRedirect() {
    var search = $('#search').val();
    if (search == "") {
        window.location.href = "/Repository/Dashboard";
    }
}

function GetAllValues() {
    var projectname = $("#projectname").val();
    var dataele = {
        "BestPractices": buildJsonInputData($('#bestPracDiv')),
        "ProjectName": projectname
    };

    var redirecturl = "/Application/InsertBestPractices";
    $.post(redirecturl, { "data": JSON.stringify(dataele) }, function (data) {
        var title, Message, type;
        if (data.isSuccess) {
            title = 'Success!!';
            Message = data.data;
            type = "green";
            $.alert({
                title: title,
                content: Message,
                type: type,
                onAction: function () {
                    window.location.href = '/Application/ChecklistBestPractices?projectname=' + projectname;
                }
            });
        }
        else {
            title = 'Error!!';
            Message = data.data;
            type = "red";
            $.alert({
                title: title,
                content: Message,
                type: type
            });
        }

    });
}

function buildJsonInputData(form) {
    debugger;
    var select = form.find('select'),
        input = form.find('input'),
        textarea = form.find('textarea');

    var displayattr = {};
    var InputData = [];
    for (var i = 0; i < select.length; i++) {
        var selectval;
        selectval = $(select[i]).val();
        displayattr = {
            "Mandatory": $(select[i]).data('mandatory'),
            "Field": $(select[i]).data('field'),
            "ListOfValues": $(select[i]).data('listOfValues')
        };
        var ddlvalue = {
            "Service": $(select[i]).data('service'),
            "ImpactArea": $(select[i]).data('impactarea'),
            "Title": $(select[i]).data('title'),
            "Description": $(select[i]).data('description'),
            "Value": selectval,
            "Mandatory": $(select[i]).data('mandatory'),
            "Field": $(select[i]).data('field'),
            "ListOfValues": $(select[i]).data('listOfValues'),
            "Remarks": ""
        };
        InputData.push(ddlvalue);
    }
    for (var i = 0; i < input.length; i++) {
        if ($(input[i]).attr('type') !== 'file' && $(input[i]).attr('type') !== 'checkbox' && $(input[i]).attr('type') != "button" && $(input[i]).attr('type') != "submit") {
            var inputval;
            inputval = $(input[i]).val();
            //displayattr = {
            //    "Mandatory": $(input[i]).data('mandatory'),
            //    "Field": $(input[i]).data('field'),
            //    "ListOfValues": $(input[i]).data('listOfValues')
            //};
            var inputValue = {
                "Service": $(input[i]).data('service'),
                "ImpactArea": $(input[i]).data('impactarea'),
                "Title": $(input[i]).data('title'),
                "Description": $(input[i]).data('description'),
                "Value": inputval,
                "Mandatory": $(input[i]).data('mandatory'),
                "Field": $(input[i]).data('field'),
                "ListOfValues": $(input[i]).data('listOfValues')
            };
            $(InputData).each(function (key, value) {
                if (value.Title == inputValue.Title && value.ImpactArea == inputValue.ImpactArea) {
                    if (value.values != "Required – Implemented") {
                        value.Remarks = inputval;
                    }
                }
            });
            //InputData.push(inputValue);
        }
    }
    for (var i = 0; i < textarea.length; i++) {
        var inputval;
        inputval = $(textarea[i]).val();
        //displayattr = {
        //    "Mandatory": $(input[i]).data('mandatory'),
        //    "Field": $(input[i]).data('field'),
        //    "ListOfValues": $(input[i]).data('listOfValues')
        //};
        var inputValue = {
            "Service": $(textarea[i]).data('service'),
            "ImpactArea": $(textarea[i]).data('impactarea'),
            "Title": $(textarea[i]).data('title'),
            "Description": $(textarea[i]).data('description'),
            "Value": inputval,
            "Mandatory": $(textarea[i]).data('mandatory'),
            "Field": $(textarea[i]).data('field'),
            "ListOfValues": $(textarea[i]).data('listOfValues')
        };
        $(InputData).each(function (key, value) {
            if (value.Title == inputValue.Title && value.ImpactArea == inputValue.ImpactArea) {
                if (value.values != "Required – Implemented") {
                    value.Remarks = inputval;
                }
            }
        });
        //InputData.push(inputValue);
    }


    return InputData;
}

function buildProjectjson(form) {
    debugger;
    var select = form.find('select.ProjectDet'),
        input = form.find('input.form-control.ProjectDet'),
        textarea = form.find('textarea.ProjectDet');

    var InputData = {};
    for (var i = 0; i < select.length; i++) {
        var selectval;
        if ($(select[i]).data('field') == "multiselect") {
            if ($(select[i]).val() != null) {
                selectval = ($(select[i]).val()).join();
            }
        }
        else {
            selectval = $(select[i]).val();
        }
        var selectname = $(select[i]).attr('name');

        InputData[selectname] = selectval;
    }

    for (var i = 0; i < input.length; i++) {
        if ($(input[i]).attr('type') !== 'file' && $(input[i]).attr('type') !== 'checkbox' && $(input[i]).attr('type') != "button" && $(input[i]).attr('type') != "submit") {
            var inputval;
            inputval = $(input[i]).val();
            var inputname = $(input[i]).attr('name');

            InputData[inputname] = inputval;
        }
        else if ($(input[i]).attr('type') == 'file') {
            var file = jQuery($(input[i])).get(0).files[0];
            var fileurl;
            if (file != null) {
                fileurl = buildFileInput(file, $(input[i]).data('customername'), $(input[i]).data('projectname'));
                var fname = file.name;
                $(input[i]).attr('data-link', fileurl);
                $(input[i]).parent().find('span').find('span').text(fname);
                $(input[i]).parent().find('span').show();
                $(input[i]).val('');
            }
            else {
                fileurl = $(input[i]).data('link');
            }

            var inputname = $(input[i]).attr('name');

            InputData[inputname] = fileurl;
        }
    }

    for (var i = 0; i < textarea.length; i++) {
        var textareaval;
        textareaval = $(textarea[i]).val();
        var textareaname = $(textarea[i]).attr('name');

        InputData[textareaname] = textareaval;
    }
    //table values
    var table = form.find('table');
    for (var i = 0; i < table.length; i++) {
        var tabledata = [];
        var tableref = $(table[i]);
        var rowctr = $(tableref).rowCount();
        var colctr = $(tableref).columnCount();
        if (rowctr > 0) {
            var tbInputData = [];
            for (var j = 1; j <= rowctr; j++) {
                var colprop = {};
                for (var k = 0; k < (colctr - 1); k++) {
                    var columnval = $(tableref).find('tr:eq(' + j + ')').find('td:eq(' + k + ')').text();
                    var columnname = $(tableref).find('tr:eq(' + j + ')').find('td:eq(' + k + ')').attr("name");
                    colprop[columnname] = columnval;
                }
                tbInputData.push(colprop);
            }
            //tabledata.push(tbInputData);
        }
        var tabname = $(tableref).attr("name");
        InputData[tabname] = tbInputData;
    }


    return InputData;
}

$.fn.rowCount = function () {
    return $('tr', $(this).find('tbody')).length;
};

$.fn.columnCount = function () {
    return $('th', $(this).find('thead')).length;
};

function buildFileInput(file, folder,subfolder) {
    var fileurl = "";
    if (file != null) {
        $.ajax({
            url: "/Repository/Upload",
            type: "POST",
            data: function () {
                var data = new FormData();
                data.append("flag", "Application");
                data.append("phase", folder);
                data.append("subphase", subfolder);
                data.append("file", jQuery("#fileToUpload").get(0).files[0]);
                data.append("tags", "");
                data.append("displaytags", "");
                return data;
            }(),
            async: false,
            contentType: false,
            processData: false,
            success: function (response) {
                fileurl = response.encodedurl;
            },
            error: function (jqXHR, textStatus, errorMessage) {
                console.log(errorMessage);
                $.alert({
                    title: 'Error!!',
                    content: errorMessage,
                });
            }
        });
    }
    return fileurl;
}
function removeFile(e) {
    $(e).parent().parent().find('span').remove();
}
function redirect() {
    console.log($('#projectname').val());
    var projectname = $('#projectname').val();
    window.location.href = "/Application/ChecklistBestPractices?projectname=" + projectname;

}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [day, month, year].join('/');
}
function YearformatDate(date) {
    var from = date.split("/")
    var d = new Date(from[2], from[1] - 1, from[0]),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}
function LoadService(NavParam,displaymode) {
    debugger;
    $("div.loading").show();
    var json = $.parseJSON(NavParam);
    var prodcat = $('.nav-link.border-0.text-uppercase.font-weight-bold.active').data('prodcat');
    var service = $('.nav-link.border-0.text-uppercase.font-weight-bold.active').data('service');
    var divid = $('.nav-link.border-0.text-uppercase.font-weight-bold.active').data('div');
    if (displaymode != "readonly") {
        var dataele = {
            "Checklist": buildJsonInputData($('#' + divid)),
            "ProjectName": json.projectName,
            "customername": json.customerName,
            "ProductCategory": prodcat,
            "Service": service,
            "Status": "In Progress"
        };
        var redirecturl = "/Application/InsertBestPractices";
        $.post(redirecturl, { "data": JSON.stringify(dataele) }, function (data) {

        });
    }
    $.ajax({
        async: false,
        type: "GET",
        url: "/Application/LoadService",
        data: { "productcat": json.currentProdCat, "service": json.currentService, "projectname": json.projectName, "customerName": json.customerName },
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#" + json.currentDivid).html(data);
        },
        error: function (data) {
            console.log(JSON.stringify(data));
        },
        complete: function (data) {
            // Hide image container
            $("div.loading").hide();

        }
    });

    return false;
}

