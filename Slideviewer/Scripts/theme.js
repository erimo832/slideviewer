/* requiers ref to jquery and query param */
$(document).ready(function () {

    var theme = getQueryParam("theme");

    if (!(theme == null || theme == "")) {
        $("head link#theme").attr("href", "/css/themes/" + theme + ".css");
    }

    var size = getQueryParam("size", 30);

    if (!(size == null || size == "")) {
        $("body").css("font-size", size + "px");
        $(".sizable").css("height", size + "px");
    }

    var altsize = getQueryParam("altsize");

    if (altsize) {
        $('.altsize').each(function () {
            $(this).css("font-size", altsize + "px");
        });
    }

    var fontFamily = getQueryParam("font-family"); //Arial Black

    if (!(fontFamily == null || fontFamily == "")) {
        $("body").css("font-family", fontFamily);
    }

});
