var setLoginEnable = function (enabled) {
    if (enabled) {
        $("#login").removeAttr("disabled");
    } else {
        $("#login").attr("disabled", "disabled");
    }
}

var switchPanel = function (isLoginShown) {
    $("#loginPanel").toggle(isLoginShown);
    $("#chatPanel").toggle(!isLoginShown);
}

var buildChannels = function (channels) {
    $("#channels").html(""); //refresh channel list

    $.each(channels,
        function (index, channel) {
            $("#channels").append(" <li><a href='#' class='channel_item' data-channel=" + channel + ">" + channel + "</a></li>");
        });

    if (channels.length === 0)
        $("#channels").append(" <li><a href='#'>No channels are created!</a></li>");

    $("#channels").append("<li role='separator' class='divider'></li>");
    $("#channels").append(" <li><a href='#' id='newChannel'>Create new channel</a></li>");
}

var refreshUsers = function (users, name) {
    $("#userList").html("");
    $.each(users, function (index, user) {
        $("#userList").append("<div class='user' id='" + user + "' data-user='" + user + "'>" + user + "</div>");
    });

    $("#" + name).addClass("me");
}

// adding information to messagebox
var addChatInformation = function (message) {
    $("#messages").append("<div class='chat_information'>* " + message + "</div>");
}

var addChatItem = function (conversation) {
    $("#messages").append("<div class='chat_item'>" + conversation + "</div>");
}

var showAlert = function (title, message) {
    $.toast({
        heading: title,
        text: message,
        icon: "info",
        loader: false,
        showHideTransition: "fade",
        hideAfter: 6000,
        position: "top-right"
    });
}

var changeState = function (connected) {
    if (connected) {
        showAlert("Information", "You are connected to SignalR");
        $("#send").removeAttr("disabled");
        $("#login").removeAttr("disabled");
    } else {
        showAlert("Warning", "SignalR connection is lost");
        $("#send").attr("disabled", "disabled");
        $("#login").attr("disabled", "disabled");
    }
}