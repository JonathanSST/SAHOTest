$(document).ready(function () {
    $("#TableCode").find("tr").each(function (index) {
        var code = $(this).find("td").last().find('input[name*="OpStatus"]').val();
        if (code == "1") {
            $(this).find("td").css("background-color", "Red").css("color", "#fff");
        } else if (code == "2") {
            $(this).find("td").css("background-color", "#ffffff");        
        } else if (code == "3") {
            $(this).find("td").css("background-color", "NAVY").css("color", "#fff");        
        } else if (code == "4") {
            $(this).find("td").css("background-color", "Yellow").css("color", "#000");
        }
    });
});