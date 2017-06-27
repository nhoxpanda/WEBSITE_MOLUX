var list_user = [];
var list_msg = [];

$(function () {
    // Declare a proxy to reference the hub. 
    var chatHub = $.connection.chatHub;
    registerClientMethods(chatHub);
    // Start Hub
    $.connection.hub.start().done(function () {
        registerEvents(chatHub)
    });
});

function registerEvents(chatHub) {
    var name = $("#hdUserName").val();
    if (name.length > 0) {
        chatHub.server.connect(name, userId);
    }

    $('#btnSendMsg').click(function () {
        var msg = $("#txtMessage").val();
        if (msg.length > 0) {
            var userName = $('#hdUserName').val();
            chatHub.server.sendMessageToAll(userName, msg, userId);
            $("#txtMessage").val('');
        }
    });

}

function registerClientMethods(chatHub) {
    // Calls when user successfully logged in
    chatHub.client.onConnected = function (ConnectionId, userName, allUsers, messages) {
        $('#hdId').val(ConnectionId);
        //$('#hdUserName').val(userName);
        //$('#spanUser').html(userName);

        // Add All Users
        for (i = 0; i < allUsers.length; i++) {

            AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].UserName, allUsers[i].id, allUsers[i].FullName, allUsers[i].Position);

            var user_ob = {
                user_name: allUsers[i].UserName,
                user_connection_id: allUsers[i].ConnectionId,
                user_id: allUsers[i].id,
                user_fullname: allUsers[i].FullName,
                user_position: allUsers[i].Position
            }
            list_user.push(user_ob);
        }

        // Add Existing Messages
        for (i = 0; i < messages.length; i++) {
            AddMessage(messages[i].UserName, messages[i].Message);
        }
    }

    // On New User Connected
    chatHub.client.onNewUserConnected = function (ConnectionId, name, id) {
        //debugger;
        //AddUser(chatHub, ConnectionId, name);
        UpdateStatusUserLogin(id);
    }

    // On User Disconnected
    chatHub.client.onUserDisconnected = function (ConnectionId, userName, id) {
        $('#divusers').find('#' + id).removeClass('login');

        //$('#' + ConnectionId).remove();

        var ctrId = 'private_' + ConnectionId;
        $('#' + ctrId).remove();

        var disc = $('<div class="disconnect">"' + userName + '" logged off.</div>');

        $(disc).hide();
        $('#divusers').prepend(disc);
        $(disc).fadeIn(200).delay(2000).fadeOut(200);
    }

    chatHub.client.createGroup = function (userId, userName, userIdTo, groupId) {
        var ctrId = 'private_' + groupId;
        if ($('#' + ctrId).length == 0) {
            OpenPrivateChatWindow(chatHub, groupId, userId, userName, userIdTo);
        }
    }

    chatHub.client.messageReceived = function (userName, message) {
        AddMessage(userName, message);
    }

    chatHub.client.sendPrivateMessage = function (windowId, userId, fromUserName, message, userIdTo) {
        var ctrId = 'private_' + windowId;
        if ($('#' + ctrId).length == 0) {
            createPrivateChatWindow(chatHub, windowId, userId, fromUserName, userIdTo);
        }
        $('#' + ctrId).find('#divMessage').append('<div class="post out">'
            + '<img class="avatar" alt="" src="/Content/assets/layouts/layout/img/avatar2.jpg" />'
            + '<div class="message">'
            + '<span class="arrow"></span>'
            + '<a href="javascript:;" class="name">' + fromUserName + '</a>'
            + '<span class="datetime"></span>'
            + '<span class="body">' + message + '</span>'
            + '</div>'
            + '</div>');
        // set scrollbar
        //var height = $('#' + ctrId).find('#divMessage')[0].scrollHeight;
        //$('#' + ctrId).find('#divMessage').scrollTop(height);
    }

    chatHub.client.getMessage = function (messages) {
        var code = "";
        for (var i = 0; i < messages.length; i++) {
            code = $('<div class="post ' + (messages[i].UserId === $("#userId").val() ? 'out' : 'in') + '">'
                   + '<img class="avatar" alt="" src="/Content/assets/layouts/layout/img/' + (messages[i].UserId === $("#userId").val() ? 'avatar3.jpg' : 'avatar2.jpg') + '" />'
                   + '<div class="message">'
                   + '<span class="arrow"></span>'
                   + '<a href="javascript:;" class="name">' + messages[i].UserName + '</a>'
                   + '<span class="datetime"> - ' + messages[i].Time + '</span>'
                   + '<span class="body">' + messages[i].Message + '</span>'
                   + '</div>'
                   + '</div>');
            $("#divMessage").append(code);
        }
    }
}

function UpdateStatusUserLogin(id) {
    $('#divusers').find('#' + id).addClass('login');
}

function AddUser(chatHub, ConnectionId, name, id, fullname, position) {

    var userConnectionId = $('#hdId').val();
    var code = "";

    if (userConnectionId == ConnectionId) {
        code = $('<div class="loginUser hide">' + name + "</div>");
    }
    else {
        code = $("<li class='media'><div class='media-status'><span class='badge badge-success'>8</span></div><img class='media-object' src='/Content/assets/layouts/layout/img/avatar3.jpg' alt='...'><div class='media-body'><h4 class='media-heading'>" +
             "<a id='" + id + "' class='user " + (ConnectionId != null ? "login" : "") + "'>" + fullname + "</a></h4><div class='media-heading-sub'>" + position + "</div></div></li>");
        $(code).click(function () {
            if (userConnectionId != ConnectionId) {
                //OpenPrivateChatWindow(chatHub, id_connection, name, id);
                CreateGroupChat(chatHub, id, userId);
            }
        });
    }

    $("#divusers").append(code);
}

function AddMessage(userName, message) {
    $('#divMessage').append('<div class="post in">'
    + '<img class="avatar" alt="" src="/Content/assets/layouts/layout/img/avatar2.jpg" />'
    + '<div class="message">'
    + '<span class="arrow"></span>'
    + '<a href="javascript:;" class="name">' + userName + '</a>'
    + '<span class="datetime"></span>'
    + '<span class="body">' + message + '</span>'
    + '</div>'
    + '</div>');
    //var height = $('#divMessage')[0].scrollHeight;
    //$('#divMessage').scrollTop(height);
}

function CreateGroupChat(chatHub, userIdTo, userId) {
    $("ul.media-list").css("display", "none");
    chatHub.server.checkGroup(userIdTo, userId);
    $("#userId").val(userId);
}

function OpenPrivateChatWindow(chatHub, groupId, userId, userName, userIdTo) {
    var ctrId = 'private_' + groupId;
    if ($('#' + ctrId).length > 0) return;
    createPrivateChatWindow(chatHub, groupId, userId, userName, userIdTo);
}

function createPrivateChatWindow(chatHub, groupId, userId, userName, userIdTo) {
    var ctrId = 'private_' + groupId;
    var div = '<div class="page-quick-sidebar-item" id="' + ctrId + '">'
    + '<div class="page-quick-sidebar-chat-user">'
    + '<div class="page-quick-sidebar-nav">'
    + '<a onclick="closeChat()" style="cursor: pointer" class="page-quick-sidebar-back-to-list">'
    + '<i class="icon-arrow-left"></i>Back'
    + '</a>'
    + '</div>'
    + '<div class="page-quick-sidebar-chat-user-messages" id="divMessage">'
    + '</div>'
    + '<div class="page-quick-sidebar-chat-user-form">'
    + '<div class="input-group">'
    + '<input type="text" class="form-control" id="txtPrivateMessage_' + groupId + '" placeholder="Type a message here...">'
    + '<input class="txtGroup" type="hidden" value="' + groupId + '"  />'
    + '<div class="input-group-btn">'
    + '<button type="button" class="btn green" id="btnSendMessage_' + groupId + '">'
    + '<i class="icon-paper-clip"></i>'
    + '</button>' + '</div>' + '</div>' + '</div>' + '</div>' + '</div>';

    //$('#message').empty();
    //$('#message').append(div);
    //$(".page-quick-sidebar-item").css("margin-left", "0");

    var $div = $(div);

    // DELETE BUTTON IMAGE
    $div.find('#imgDelete').click(function () {
        $('#' + ctrId).remove();
    });

    // Send Button event
    $div.find("#btnSendMessage_" + groupId).click(function () {
        $textBox = $div.find("#txtPrivateMessage_" + groupId);
        var msg = $textBox.val();
        if (msg.length > 0) {
            chatHub.server.sendPrivateMessage(groupId, userId, msg, userIdTo);
            $textBox.val('');
        }
    });

    // Text Box event
    $div.find("#txtPrivateMessage_" + groupId).keypress(function (e) {
        if (e.which == 13) {
            $div.find("#btnSendMessage_" + groupId).click();
        }
    });

    $('#message').empty();
    $('#message').append($div);
    $(".page-quick-sidebar-item").css("margin-left", "0");
}