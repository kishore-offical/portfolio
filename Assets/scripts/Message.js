var messages = null;
$.Messages = function (options) {
    messages = $.extend({
        receiver: '',
        currentUserId: '',
        existingGroupId: 0
    }, options);
};

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var currentUserName = "";

// Handle connection state transitions
connection.onclose(function (e) {
    console.log("Connection closed:", e);
});

connection.onreconnecting(function (reconnectionId) {
    console.log("Reconnecting. ID:", reconnectionId);
});

connection.onreconnected(function (connectionId) {
    console.log("Reconnected. ID:", connectionId);
});

connection.on("ReceiveMessage", function (user, message) {
    console.log("ReceiveMessage invoked")
    var messageContainer = $("#message-container");
    var messageElement = $("<div>").addClass("message");

    if (user === currentUserName) {
        messageElement.addClass("sent");
        messageElement.append("<strong>You</strong>: ");
    } else {
        messageElement.addClass("received");
        messageElement.append("<strong>" + user + "</strong>: ");
    }

    messageElement.append(message);
    messageContainer.append(messageElement);

    scrollToBottom();
});

// Start the connection when the document is ready
$(document).ready(function () {
    connection.start().then(function () {
        console.log("Connected to SignalR Hub");

        // Get current user name and id
        connection.invoke("GetCurrentUserName").then(function (username) {
            currentUserName = username;
        }).catch(function (err) {
            console.error(err.toString());
        });
        connection.invoke("GetCurrentUserId").then(function (userId) {
            messages.currentUserId = userId;
            console.log("userId", userId);
            connection.invoke("GetGroupId", messages.currentUserId, messages.receiver).then(function (groupId) {
                messages.existingGroupId = groupId;
                if (messages.existingGroupId > 0) {
                    connection.invoke("AddToGroup", messages.existingGroupId).then(function () {
                    }).catch(function (err) {
                        console.error(err.toString());
                    });
                    LoadMessage(messages.existingGroupId);
                }
            }).catch(function (err) {
                console.error(err.toString());
            });
        }).catch(function (err) {
            console.error(err.toString());
        });
    }).catch(function (err) {
        console.error("Error starting connection:", err.toString());
    });
});
$("#sendMessageBtn").click(function () {
    var messageInput = $("#messageInput");
    var message = messageInput.val();
    if (message) {
        //connection.invoke("GetGroupId", messages.currentUserId, messages.receiver).then(function (groupId) {

        //    if (groupId > 0) {
        //        connection.invoke("SendMessage", currentUserName, message, groupId).catch(function (err) {
        //            console.error(err.toString());
        //        });
        //    }
        //}).catch(function (err) {
        //    console.error(err.toString());
        //});
    

    saveConversation(messages.currentUserId, messages.receiver, message);
    messageInput.val("");
}
    
});

$("#HomeBtn").click(function () {
    window.location.href = "/Account/Home";
});

function saveConversation(sender, receiver, message) {
    $.ajax({
        url: `/api/chat/createGroup?senderId=${sender}&receiverId=${receiver}`,
        method: "POST",
        success: function (groupId) {
            messages.existingGroupId = groupId;

            connection.invoke("SendMessage", currentUserName, message, groupId).catch(function (err) {
                console.error(err.toString());
            });
            saveMessage(sender, receiver, message, groupId);
        },
        error: function (error) {
            showErrorNotification("Error creating or retrieving group.");
        }
    });
}

function saveMessage(sender, receiver, message, groupId) {
    $.ajax({
        url: `/api/chat/create?sender=${sender}&receiver=${receiver}&message=${message}&groupId=${groupId}`,
        method: "POST",
        success: function () {
        },
        error: function (error) {
            showErrorNotification("Something went wrong!.");
        }
    });
}

function showErrorNotification(message) {
    toastr.error(message, 'Error', { toastClass: 'toastr-error' });
}
function LoadMessage(groupId) {
    $.ajax({
        url: `/api/chat/getMessages?groupId=${groupId}`,
        method: "GET",
        success: function (response) {
            if (response.length > 0) {
                response.forEach(function (message) {
                    appendMessageToChat(message);
                });
            }
        },
        error: function (error) {
            showErrorNotification("Something went wrong!.");
        }
    });
}
function appendMessageToChat(message) {
    var messageContainer = $("#message-container");
    var messageElement = $("<div>").addClass("message");

    if (message.sender === "You") {
        messageElement.addClass("sent");
        //messageElement.append("<span class='timestamp'>" + message.sendOn + "</span> ");
        messageElement.append("<span class='sender'><strong>You:</strong></span> ");
        messageElement.append("<span class='text'>" + message.message + "</span>");
    } else if (message.receiver === "You") {
        messageElement.addClass("received");
        //messageElement.append("<span class='timestamp'>" + message.sendOn + "</span> ");
        messageElement.append("<span class='sender'><strong>" + message.sender + "</strong>:</span> ");
        messageElement.append("<span class='text'>" + message.message + "</span>");
    }

    messageContainer.append(messageElement);
    scrollToBottom();
}
function scrollToBottom() {
    const element = $("#message-container");
    element.animate(
        {
            scrollTop: element.prop("scrollHeight"),
        },
        500
    );
};