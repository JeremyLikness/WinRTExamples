/// <reference path="../shared/mobileservices.intellisense.js"/>

function insert(item, user, request) {
    /// <param name="item" type="Object"></param>
    /// <param name="user" type="User"></param>
    /// <param name="request" type="Request"></param>

    // Validate the input
    if (!item.LastName || !item.FirstName) {
        request.respond(statusCodes.BAD_REQUEST, "First and Last Names are required");
    } else {
        // set the id of the currently logged in user as the owner for the new item
        item.ownerId = user.userId;
        request.execute();
    }
}