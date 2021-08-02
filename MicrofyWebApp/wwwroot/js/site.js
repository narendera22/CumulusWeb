// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    $(".anchor-link").on("click", function () {
        $(".anchor-link").removeClass("active");
        $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
        $(this).addClass("active");
        // $(this).toggleClass("active");

        var htmltemplate;
        if ($(this).siblings().length === 0) {
            $(".anchor-link").siblings().removeClass('show');
            $(this).addClass('active');
            var thismenu = $(this).find('a').text();
            // var subtabmenu=$('#documentpnl').find('nav').find('#nav-tab').find('a.nav-item.nav-link.active').text();
            $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
            htmltemplate = $('<li class="active"> <span>' + thismenu + '</span></li>');
            $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
            var Phase = thismenu;
            Getdocuments(Phase);

        }
        else {
            $(this).siblings().addClass('show');
            $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
            htmltemplate = $('<li class="active"> <span>Dashboard </span></li></ol>');
            $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
            $(this).siblings().find('.subanchor-link').find('a').first().trigger('click');
            $(this).removeClass('active');
        }
    });



    $(".subanchor-link").on("click", function () {

        $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
        $(this).addClass('active');
        var phase = $(this).parent().parent().find('a.dropdown-toggle').text();
        var subphase = $(this).find('a').text();
        $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
        var htmltemplate = $('<li><a href="#" onclick="subanchorlink()">' + phase + '</a></li><li class="active"> <span>' + subphase + '</span></li>');
        $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
        Getdocuments(phase, subphase);


    });
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

function DocumentSave(url) {
    //var tags = [];
    var name = $('#txtname').val();
    var description = $('#txtdescp').val();
    var tags = $('#txtTags').val().split(",").map(function (value) {
        return value.trim();
    });
    //$.each(txttags, function (i) {
    //    alert(array[i]);
    //});
    //tags.push(txttags);

    var active = GetActivePhase();
    var phase = active.phase;
    var subphase = active.subphase;


    var documentDet = {
        "Phase": phase,
        "SubPhase": subphase,
        "DocumentName": name,
        "Description": description,
        "URL": url,
        "Tags": tags
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
                $("#documentpnl").show();
            }
        });
    });
    return false;
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
            $("#dashboard").hide();
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
            $("#dashboard").hide();
            $("#uploadDocument").show();
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