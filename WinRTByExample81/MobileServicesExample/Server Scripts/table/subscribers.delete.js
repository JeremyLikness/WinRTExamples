/// <reference path="../shared/mobileservices.intellisense.js"/>

function del(id, user, request) {
    /// <param name="id" type="Number"></param>
    /// <param name="user" type="User"></param>
    /// <param name="request" type="Request"></param>

    var subscriberTable = tables.current;
    subscriberTable.where({
        id: id
    }).read({
        success: function (items) {
            if (items.length === 0) {
                request.respond(
                    statusCodes.NOT_FOUND,
                    { error: 'No matching item found to delete.' });
            } else {
                if (items[0].ownerId != user.userId) {
                    request.respond(
                        statusCodes.BAD_REQUEST,
                        { error: "Cannot delete someone else's item." });
                } else {
                    request.execute();
                }
            }
        }
    });
}