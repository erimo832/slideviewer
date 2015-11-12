var images = [
                "/images/gothouses/stark.jpg",
                "/images/gothouses/targaryen.jpg",
                "/images/gothouses/tully.jpg",
                "/images/gothouses/tyrell.jpg",
                "/images/gothouses/martell.jpg",
                "/images/gothouses/lannister.jpg",
                "/images/gothouses/greyjoy.jpg",
                "/images/gothouses/baratheon.jpg",
                "/images/gothouses/arryn.jpg"
];
var intervalId;

//init
function initGotHouses() {
    intervalId = setInterval("updatebackground()", 5000);    
    updatebackground();
}

//clear interval
function clearGotHouses() {
    if (id != null) {
        window.clearInterval(id);

        id = null;

        $("#body").css("background-color", "");
    }
}


//
function updatebackground() {
    var num = Math.round((Math.random() * (images.length - 1)));
    var value = 'url("' + images[num] + '")';
    $("#body").css("background-image", value);
}