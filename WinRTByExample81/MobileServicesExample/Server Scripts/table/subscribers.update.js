/// <reference path="../shared/mobileservices.intellisense.js"/>

function update(item, user, request) {
    /// <param name="id" type="Number"></param>
    /// <param name="user" type="User"></param>
    /// <param name="request" type="Request"></param>

    // Validate the input
    if (!item.LastName || !item.FirstName) {
        request.respond(statusCodes.BAD_REQUEST, "First and Last Names are required");
    } else {
        var subscriberTable = tables.current;
        subscriberTable.where({
            id: item.id
        }).read({
            success: function(items) {
                if (items.length === 0) {
                    request.respond(statusCodes.NOT_FOUND, { error: 'No matching item found to update.' });
                } else {
                    request.execute({
                        success: function() {
                            if (items[0].ownerId != user.userId) {
                                notifyOwnerOfChanges(items[0]);
                                //console.log("Sending a push for item id " + items[0].id + ' and owner ' + items[0].ownerId);
                            }
                            request.respond();
                        }
                    });
                }
            }
        });
    }
}

function notifyOwnerOfChanges(changedItem) {
    var channelTable = tables.getTable("channels");
    channelTable.where({ userId: changedItem.ownerId }).read({
        success: function (results) {
            if (results.length > 0) {
                for (var i = 0; i < results.length; i++) {
                    var matchingNotificationChannel = results[i];
                    sendNotifications(matchingNotificationChannel.channelUri, changedItem);
                }
            }
        }
    });
}

function sendNotifications(uri, changedItem) {
    console.log("Uri: ", uri);
    push.wns.sendToastText01(uri, {
        text1: 'One of your records - ' + changedItem.FirstName + ' ' + changedItem.LastName + ' - has been edited by another user.'
    }, {
        success: function (pushResponse) {
            console.log("Sent push:", pushResponse);
        }
    });
}