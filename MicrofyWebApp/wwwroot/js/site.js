// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
    $(".anchor-link").on("click", function () {
        $(".anchor-link").removeClass("active");
        $(this).addClass("active");
        // $(this).toggleClass("active");
        $(".anchor-link").siblings().removeClass('show');
        var htmltemplate;
        if ($(this).siblings().length === 0) {
            $("#newdocdiv").show();
            $("#dashboard").hide();
            $("#uploadDocument").hide();
            $("#documentpnl").show();
            $(this).addClass('active');
            var thismenu = $(this).find('a').text();
            // var subtabmenu=$('#documentpnl').find('nav').find('#nav-tab').find('a.nav-item.nav-link.active').text();
            $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
            htmltemplate = $('<li class="active"> <span>' + thismenu + '</span></li>');
            $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
            var Phase = thismenu;
            $.ajax({
                type: "GET",
                url: "/Home/GetResultByAjax",
                data: { "Phase": thismenu },
                contentType: "application/json; charset=utf-8",
                //dataType: 'json',
                success: function (data) {
                    //window.location.replace(data.newUrl);
                    $("#documentpnl").html(data);

                },
                error: function (data) {
                    console.log(JSON.stringify(data));
                }
            });
        }
        else {
            $("#newdocdiv").hide();
            $("#dashboard").show();
            $("#uploadDocument").hide();
            $("#documentpnl").hide();
            $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
            htmltemplate = $('<li class="active"> <span>Dashboard </span></li></ol>');
            $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
            $(this).siblings().find('.subanchor-link').find('a').first().trigger('click');
            $(this).removeClass('active');
        }
    });

    $(".subanchor-link").on("click", function () {
        $("#newdocdiv").show();
        $("#dashboard").hide();
        $("#uploadDocument").hide();
        $("#documentpnl").show();
        $('#sidebar').find('.list-unstyled.collapse.show').find('.subanchor-link.active').removeClass("active");
        $(this).addClass('active');
        var mainmenu = $(this).parent().parent().find('a.dropdown-toggle').text();
        var thismenu = $(this).find('a').text();
        $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
        var htmltemplate = $('<li><a href="#">' + mainmenu + '</a></li><li class="active"> <span>' + thismenu + '</span></li>');
        $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);

    });
    $(".homelink").on("click", function () {
        $("#newdocdiv").hide();
        $("#dashboard").show();
        $("#uploadDocument").hide();
        $("#documentpnl").hide();
        $(".anchor-link").removeClass("active");
        $(".anchor-link").siblings().removeClass('show');
        $('#divbreadcrumb').find('ol.breadcrumb').find('li.homelink').siblings().remove();
        var htmltemplate = $('<li class="active"> <span>Dashboard </span></li></ol>');
        $('#divbreadcrumb').find('ol.breadcrumb').append(htmltemplate);
    });

    $('#selectAll').click(function () {
        if (this.checked) {
            $(':checkbox').each(function () {
                this.checked = true;
            });
        } else {
            $(':checkbox').each(function () {
                this.checked = false;
            });
        }
    });

    $('#uploads').submit(function (e) {
        e.preventDefault(); // stop the standard form submission
        console.log(new FormData(this));
        var documentDet = {};
        documentDet.name = $('#txtname').val();
        documentDet.description = $('#txtdescp').val();
        documentDet.tags = $('#txtTags').val();
        $.ajax({
            url: "/Home/Upload",
            type: this.method,
            data: new FormData(this),
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) {
                documentDet.url = data;
                DocumentSave(documentDet);
                alert(data.UploadedFileCount + ' file(s) uploaded successfully');
            },
            error: function (xhr, error, status) {
                console.log(error, status);
            }
        });
    });

});

function SaveDocument() {
    var form = $("#uploads");
    var formData = new FormData(form[0]);

    var documentDet = {};
    documentDet.name = $('#txtname').val();
    documentDet.description = $('#txtdescp').val();
    documentDet.tags = $('#txtTags').val();
    $.ajax({
        url: "/Home/Upload",
        type: "Post",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            documentDet.url = data;
            DocumentSave(documentDet);
        },
        error: function (xhr, error, status) {
            console.log(error, status);
        }
    });
    return false;
}

function DocumentSave(url) {
    var tags = [];
    var name = $('#txtname').val();
    var description = $('#txtdescp').val();
    var txttags = $('#txtTags').val();
    tags.push(txttags);
    var phase = $(".anchor-link.active").find("a").text();

    var documentDet = {
        "phase": phase,
        "name": name,
        "description": description,
        "tags": tags,
        "url":url
    }
    var redirecturl = "/Home/CreateDocument";
    $.post(redirecturl, documentDet, function (data) {
        $("#documentpnl").html(data);
        $("#newdocdiv").hide();
        $("#dashboard").hide();
        $("#uploadDocument").hide();
        $("#documentpnl").show();
    });
    return false;
}

function UploadDocument(e) {
    $("#newdocdiv").hide();
    $("#dashboard").hide();
    $("#uploadDocument").show();
    $("#documentpnl").hide();
    $.ajax({
        type: "GET",
        url: "/Home/GetUploadPartial",
        data: {},
        contentType: "application/json; charset=utf-8",
        //dataType: 'json',
        success: function (data) {
            //window.location.replace(data.newUrl);
            $("#uploadDocument").html(data);

        },
        error: function (data) {
            console.log(JSON.stringify(data));
        }
    });
}