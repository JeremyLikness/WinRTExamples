/// <reference path="../shared/mobileservices.intellisense.js"/>

function insert(item, user, request) {
    /// <param name="item" type="Object"></param>
    /// <param name="user" type="User"></param>
    /// <param name="request" type="Request"></param>

    // Validate the input
    if (!item.LastName || !item.FirstName) {
        console.error("Invalid user data was provided");
        request.respond(statusCodes.BAD_REQUEST, "First and Last Names are required");
    } else {
        // Extend the item to include the date added
        item.dateAdded = new Date();

        // set the id of the logged in user as the owner for the new item
        item.ownerId = user.userId;

        request.execute({
            success: function () {
                // Strip server-only properties off of the result
                if (item.dateAdded) delete (item.dateAdded);
                if (item.ownerId) delete (item.ownerId);
                request.respond();
            }
        });
    }
}