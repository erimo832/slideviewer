/*******************************************
* functions
*******************************************/
//Gets value from a key in the querystring
function getQueryParam(paramName, defaultValue) {
    //Remove first ? character
    var query = location.search.substring(1);

    if (query.length > 0) {
        //Display date time
        query = query.split("%20").join(" ");
        query = query.split("%23").join("#");
        query = query.split("%27").join("'");
        query = query.split("%C3%B6").join("ö");
        query = query.split("%C3%A5").join("å");
        query = query.split("%C3%A4").join("ä");


        var params = query.split("&");

        //style, format, offset
        for (i = 0; i < params.length; i++) {

            var keyVal = params[i].split("=");
            if (keyVal[0] == paramName) {
                return keyVal[1];
            }
        }
    }

    if (typeof defaultValue != 'undefined')
        return defaultValue;

    return null;
}
