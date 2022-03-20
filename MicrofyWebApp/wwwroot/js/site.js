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
        url: "/Home/GetDocumentRepository",
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
    var redirecturl = "/Home/CreateDocument";
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
        url: "/Home/GetUploadPartial",
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
        url: "/Home/GetNewUploadPartial",
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
    $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
    htmltemplate = "";
    $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    var search = $('#search').val();
    $.ajax({
        type: "GET",
        url: "/Home/SearchDocumentPartial",
        data: { "Search": search },
        contentType: "application/json; charset=utf-8",
        //dataType: 'json',
        success: function (data) {
            //window.location.replace(data.newUrl);
            $("#SerachDocument").html(data);
            //$("#newdocdiv").hide();
            $("#SerachDocument").show();
            $("#dashboard").hide();
            $("#uploadDocument").hide();
            $("#documentpnl").hide();

        },
        error: function (data) {
            console.log(JSON.stringify(data));
        }
    });
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
        "subphase": subphase
    };

    var redirecturl = "/Home/UpdateMetadata";
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
                    "subphase": subphase
                };

                var redirecturl = "/Home/DeleteDocument";
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
        var json = $.parseJSON(proj_cus_name)

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
            "projects": Projects
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
                    window.location.href = 'ListUser';
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
                        window.location.href = 'ListUser';
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
        window.location.href = "Dashboard";
    }
}

function GetAllValues() {
    var projectname = $("#projectname").val();
    var dataele = {
        "BestPractices": buildJsonInputData($('#bestPracDiv')),
        "ProjectName": projectname
    };

    var redirecturl = "/Checklist/InsertBestPractices";
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
                    window.location.href = '/Checklist/ChecklistBestPractices?projectname=' + projectname;
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
            "ListOfValues": $(select[i]).data('listOfValues')
        };
        InputData.push(ddlvalue);
    }


    return InputData;
}

function redirect() {
    console.log($('#projectname').val());
    var projectname = $('#projectname').val();
    window.location.href = "/Checklist/ChecklistBestPractices?projectname=" + projectname;

}