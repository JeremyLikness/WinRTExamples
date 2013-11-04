/*
Copyright (c) Microsoft Corporation. All rights reserved.
    This code is licensed as sample-code under the Visual Studio 2013 license terms.
    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
 */

apns = {
    send: function (deviceToken, payload, options) {
        ///<summary>Sends a notification to a specific device.</summary>
        ///<param name="deviceToken" type="String">Required token value that indicates which device receives the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information.</param>
        ///<param name="options" type="Object">Optional object that specifies the error handler function.</param>
    },

    getFeedback: function (completion) {
        ///<summary>Used to request information from the APNS about device-specific push notification features.</summary>
        ///<param name="completion" type="Object">A completion object that has success and error handlers.</param>
    }
}

gcm = {
    send: function (registrationId, payload, options) {
        ///<summary>Sends a notification to a specific device.</summary>
        ///<param name="registrationId" type="String">Specifies the registration ID of a device or an array of registration IDs for devices that receive the message.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information.</param>
        ///<param name="options" type="Object">Optional object that specifies the success handler and error handler functions.</param>
    },
    sendAdvanced: function (content, retryCount, options) {
        ///<summary>Used to request information from GCM using features beyond what is available in the simple send method.</summary>
        ///<param name="content" type="Object">Required object that contains the request object to send to GCM.</param>
        ///<param name="retryCount" type="Number">A non-negative integer indicating how many times the request should be re-tried if the GCM servers respond with a 5xx status code.</param>
        ///<param name="options" type="Object">Optional object that specifies the success handler and error handler functions.</param>
    }
}

console = {
    log: function (formatString, obj1) {
        ///<summary>Writes logs at the info level.</summary>
        ///<param name="formatString" type="String">The format of the entry.</param>
        ///<param name="obj1" type="Object" parameterArray="true">An object to be formatted.</param>
    },

    warn: function (formatString, obj1) {
        ///<summary>Writes logs at the warning level.</summary>
        ///<param name="formatString" type="String">The format of the entry.</param>
        ///<param name="obj1" type="Object" parameterArray="true">An object to be formatted.</param>
    },

    error: function (formatString, obj1) {
        ///<summary>Writes logs at the error level.</summary>
        ///<param name="formatString" type="String">The format of the entry.</param>
        ///<param name="obj1" type="Object" parameterArray="true">An object to be formatted.</param>
    }
}

mpns = {
    sendFlipTile: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a flip tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a flip tile notification to an array of channels.</summary>
        ///<param name="channels" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTile: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to an array of channels.</summary>
        ///<param name="channels" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToast: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to an array of channels.</summary>
        ///<param name="channels" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required JSON object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendRaw: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a raw push notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a raw push notification to an array of channels.</summary>
        ///<param name="channels" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },
}

mssql = {
    open: function (options) {
        ///<summary>Opens a SQL connection. The connection is passed as an argument to the success handler.</summary>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
    },

    query: function (sql, params, options) {
        ///<signature>
        ///<summary>Executes the specified SQL. Results are passed to the success callback on the options object.</summary>
        ///<param name="sql" type="String">The SQL to execute.</param>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
        ///</signature>
        ///<signature>
        ///<summary>Executes the specified SQL, with question mark (?) placeholders replace with values from the params array. Results are passed to the success callback on the options object.</summary>
        ///<param name="sql" type="String">The SQL to execute.</param>
        ///<param name="params" type="Array" elementType="Object">An array of values used to replace the placeholders in the SQL.</param>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
        ///</signature>
    },

    queryRaw: function (sql, params, options) {
        ///<signature>
        ///<summary>Executes the specified SQL. Results are passed to the success callback on the options object in a raw format.</summary>
        ///<param name="sql" type="String">The SQL to execute.</param>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
        ///</signature>
        ///<signature>
        ///<summary>Executes the specified SQL, with question mark (?) placeholders replace with values from the params array. Results are passed to the success callback on the options object in a raw format.</summary>
        ///<param name="sql" type="String">The SQL to execute.</param>
        ///<param name="params" type="Array" elementType="Object">An array of values used to replace the placeholders in the SQL.</param>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
        ///</signature>
    }
}

Query = function Query() {
}

Query.prototype = {
    orderBy: function (arg1, arg2) {
        ///<signature>
        ///<summary>Returns a Query object where the query is ordered by the supplied column name arguments, in ascending order.</summary>
        ///<param name="arg1" type="String">The first column name.</param>
        ///<param name="arg2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">An ordered Query object.</returns>
        ///</signature>
    },

    orderByDescending: function (arg1, arg2) {
        ///<signature>
        ///<summary>Returns a Query object where the query is ordered by the supplied column name arguments, in descending order.</summary>
        ///<param name="arg1" type="String">The first column name.</param>
        ///<param name="arg2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">An ordered Query object.</returns>
        ///</signature>
    },

    read: function (options) {
        ///<summary>Reads all data from the table and invokes the success handler specified on the options parameter passing in an array of results.</summary>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
    },

    select: function (col1, col2) {
        ///<signature>
        ///<summary>Returns a Query object with the requested column projection applied.</summary>
        ///<param name="col1" type="String">The first column name.</param>
        ///<param name="col2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">A Query object with the applied projection.</returns>
        ///</signature>
        ///<signature>
        ///<summary>Returns a Query object with the requested function projection applied.</summary>
        ///<param name="projection" type="function">A function.</param>
        ///<returns type="Query">A Query object with the applied projection.</returns>
        ///</signature>
    },

    skip: function (recordCount) {
        ///<summary>Returns a Query object that skips the first recordCount number of records.</summary>
        ///<param name="recordCount" type="Number">The number of records to skip.</param>
    },

    take: function (recordCount) {
        ///<summary>Returns a Query object that returns recordCount number of records.</summary>
        ///<param name="recordCount" type="Number">The number of records to skip.</param>
    },

    where: function (obj) {
        ///<signature>
        ///<summary>Returns a Query object instance that is filtered based on the property values of the supplied JSON object.</summary>
        ///<param name="obj" type="Object">The object used to filter results.</param>
        ///<returns type="Query">A filtered Query object.</returns>
        ///</signature>
        ///<signature>
        ///<summary>Returns a Query object instance that is filtered based on the supplied function.</summary>
        ///<param name="predicate" type="function">The object used to filter results.</param>
        ///<returns type="Query">A filtered Query object.</returns>
        ///</signature>
    }
}

Request = function Request() {
}

Request.prototype = {
    execute: function () {
        ///<signature>
        ///<summary>Executes the default behavior for the operation.</summary>
        ///</signature>
        ///<signature>
        ///<summary>Executes the operation with success or error handlers supplied as options.</summary>
        ///<param name="options" type="Object">Success and error handlers executed during the operation.</param>
        ///</signature>
    },

    respond: function () {
        ///<signature>
        ///<summary>Writes the default response.</summary>
        ///</signature>
        ///<signature>
        ///<summary>Writes a response with the specified error object.</summary>
        ///<param name="err" type="Object">The error object.</param>
        ///</signature>
        ///<signature>
        ///<summary>Writes a custom response with the provided HTTP status code and service body.</summary>
        ///<param name="statusCode" type="Number">The status code to be returned.</param>
        ///<param name="body" type="String">The message body to be returned.</param>
        ///</signature>
    }
}

statusCodes = {
    ///<field name="OK" type="Number">Request succeeded.</field>
    OK: 200,

    ///<field name="OK" type="Number">Creation succeeded.</field>
    CREATED: 201,

    ///<field name="OK" type="Number">Request accepted.</field>
    ACCEPTED: 202,

    ///<field name="OK" type="Number">The request resulted in no content.</field>
    NO_CONTENT: 204,

    ///<field name="OK" type="Number">The request was malformed.</field>
    BAD_REQUEST: 400,

    ///<field name="OK" type="Number">The user in not authorized.</field>
    UNAUTHORIZED: 401,

    ///<field name="OK" type="Number">The resource cannot be accessed.</field>
    FORBIDDEN: 403,

    ///<field name="OK" type="Number">The resource was not found.</field>
    NOT_FOUND: 404,

    ///<field name="OK" type="Number">The request conflicts with another request.</field>
    CONFLICT: 409,

    ///<field name="OK" type="Number">An internal server error occurred.</field>
    INTERNAL_SERVER_ERROR: 500
}

Table = function Table() {
}

Table.prototype = {
    del: function (itemOrId, options) {
        ///<summary>Deletes the specified itemID, where itemID can be either the item or a row with the specified ID.</summary>
        ///<param name="itemOrId" type="Object">The item or ID of the row to be deleted.</param>
        ///<param name="options" type="Object">An optional object that contains success or error handlers.</param>
    },

    insert: function (item, options) {
        ///<summary>Inserts the specified item into the specified tableName.</summary>
        ///<param name="item" type="Object">The item to be inserted.</param>
        ///<param name="options" type="Object">An optional object that contains success or error handlers.</param>
    },

    orderBy: function (arg1, arg2) {
        ///<signature>
        ///<summary>Returns a Query object where the query is ordered by the supplied column name arguments, in ascending order.</summary>
        ///<param name="arg1" type="String">The first column name.</param>
        ///<param name="arg2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">An ordered Query object.</returns>
        ///</signature>
    },

    orderByDescending: function (arg1, arg2) {
        ///<signature>
        ///<summary>Returns a Query object where the query is ordered by the supplied column name arguments, in descending order.</summary>
        ///<param name="arg1" type="String">The first column name.</param>
        ///<param name="arg2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">An ordered Query object.</returns>
        ///</signature>
    },

    read: function (options) {
        ///<summary>Reads all data from the table and invokes the success handler specified on the options parameter passing in an array of results.</summary>
        ///<param name="options" type="Object">A options object that has success and failure handlers.</param>
    },

    select: function (col1, col2) {
        ///<signature>
        ///<summary>Returns a Query object with the requested column projection applied.</summary>
        ///<param name="col1" type="String">The first column name.</param>
        ///<param name="col2" type="String">The second column name.</param>
        ///<param name="..." type="String">The remaining column names.</param>
        ///<returns type="Query">A Query object with the applied projection.</returns>
        ///</signature>
        ///<signature>
        ///<summary>Returns a Query object with the requested function projection applied.</summary>
        ///<param name="projection" type="function">A function.</param>
        ///<returns type="Query">A Query object with the applied projection.</returns>
        ///</signature>
    },

    skip: function (recordCount) {
        ///<summary>Returns a Query object that skips the first recordCount number of records.</summary>
        ///<param name="recordCount" type="Number">The number of records to skip.</param>
    },

    take: function (recordCount) {
        ///<summary>Returns a Query object that returns recordCount number of records.</summary>
        ///<param name="recordCount" type="Number">The number of records to skip.</param>
    },

    where: function (obj) {
        ///<signature>
        ///<summary>Returns a Query object instance that is filtered based on the property values of the supplied JSON object.</summary>
        ///<param name="obj" type="Object">The object used to filter results.</param>
        ///<returns type="Query">A filtered Query object.</returns>
        ///</signature>
        ///<signature>
        ///<summary>Returns a Query object instance that is filtered based on the supplied function.</summary>
        ///<param name="predicate" type="function">The object used to filter results.</param>
        ///<returns type="Query">A filtered Query object.</returns>
        ///</signature>
    },

    update: function (item, options) {
        ///<summary>Updates the specified item into the specified tableName.</summary>
        ///<param name="item" type="Object">The item to be updated.</param>
        ///<param name="options" type="Object">An optional object that contains success or error handlers.</param>
    },
}

tables = {
    getTable: function (tableName) {
        ///<summary>Provides functionality for working with specific tables as a Table object instance.</summary>
        ///<param name="tableName" type="String">The name of the table to return.</param>
        ///<returns type="Table"></returns>
    }
}

User = function User() {
}

User.prototype = {
    ///<field name="accessTokens" type="Object">A JSON object that contains the access token.</field>
    accessTokens: new Object(),

    ///<field name="level" type="String">
    ///The level of authentication, which can be one of the following:
    ///<para>admin: The master key was included in the request.</para>
    ///<para>anonymous: A valid authentication token was not provided in the request.</para>
    ///<para>authenticated: A valid authentication token was provided in the request.</para>
    ///</field>
    level: "",

    ///<field name="userId" type="String">The user ID of an authenticated user. When a user is not authenticated, this property returns undefined.</field>
    userId: "",

    getIdentities: function () {
        ///<summary>Returns a JSON object that contains identity information.</summary>
        ///<returns type="Object"></returns>
    }
}

wns = {
    sendTileSquareBlock: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquareText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquareText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquareText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquareText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText05: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText06: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText07: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText08: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText09: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText10: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideText11: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquareImage: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquarePeekImageAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquarePeekImageAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquarePeekImageAndText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileSquarePeekImageAndText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideImage: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideImageCollection: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideImageAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideImageAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideBlockAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideBlockAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideSmallImageAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideSmallImageAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideSmallImageAndText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideSmallImageAndText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWideSmallImageAndText05: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection05: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageCollection06: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImageAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage05: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendTileWidePeekImage06: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a tile notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastImageAndText01: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastImageAndText02: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastImageAndText03: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendToastImageAndText04: function (channel, payload, options) {
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a toast notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">Required object that contains the notification information for the template.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    ///<param name="value" type="Number">
    ///Either a numeric value or a string value that specifies a predefined badge glyph. Numerically, this value can accept any valid integer. A value of 0 clears the badge, values from 1-99 display as given, and any value greater than 99 displays as 99+. Supported string values include the following:
    ///<para></para>
    ///</param>

    sendBadge: function (channel, value, options) {
        ///<signature>
        ///<summary>Sends a badge notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="value" type="Number">
        ///This value can accept any valid integer. A value of 0 clears the badge, values from 1-99 display as given, and any value greater than 99 displays as 99+.
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a badge notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="value" type="Number">
        ///This value can accept any valid integer. A value of 0 clears the badge, values from 1-99 display as given, and any value greater than 99 displays as 99+.
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a badge notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="value" type="String">
        ///A string value that specifies a predefined badge glyph. Supported string values include the following:
        ///<para>none</para>
        ///<para>activity</para>
        ///<para>alert</para>
        ///<para>available</para>
        ///<para>away</para>
        ///<para>busy</para>
        ///<para>newMessage</para>
        ///<para>paused</para>
        ///<para>playing</para>
        ///<para>unavailable</para>
        ///<para>error</para>
        ///<para>attention</para>
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a badge notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="value" type="String">
        ///A string value that specifies a predefined badge glyph. Supported string values include the following:
        ///<para>none</para>
        ///<para>activity</para>
        ///<para>alert</para>
        ///<para>available</para>
        ///<para>away</para>
        ///<para>busy</para>
        ///<para>newMessage</para>
        ///<para>paused</para>
        ///<para>playing</para>
        ///<para>unavailable</para>
        ///<para>error</para>
        ///<para>attention</para>
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    sendRaw: function (channel, value, options) {
        ///<signature>
        ///<summary>Sends a raw notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="value" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a raw notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="value" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    },

    send: function (channel, payload, type, options) {
        ///<signature>
        ///<summary>Sends a raw notification to a specific channel.</summary>
        ///<param name="channel" type="String">Required channel URI value that indicates the channel to receive the notification.</param>
        ///<param name="payload" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="type" type="String">
        ///Specifies the type of the notification, as one of the following values:
        ///<para>wns/badge: A notification to create a badge overlay on the tile. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/tile: A notification to update the tile content. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/toast: A notification to raise a toast on the client. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/raw: A notification which can contain a custom payload and is delivered directly to the app. The Content-Type header included in the notification request must be set to "application/octet-stream".</para>
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
        ///<signature>
        ///<summary>Sends a raw notification to a specific channel.</summary>
        ///<param name="channel" type="Array">Required array of URI values that indicates the channels to receive the notification.</param>
        ///<param name="payload" type="Object">An application-specific string that is delivered to the client without modification.</param>
        ///<param name="type" type="String">
        ///Specifies the type of the notification, as one of the following values:
        ///<para>wns/badge: A notification to create a badge overlay on the tile. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/tile: A notification to update the tile content. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/toast: A notification to raise a toast on the client. The Content-Type header included in the notification request must be set to "text/xml".</para>
        ///<para>wns/raw: A notification which can contain a custom payload and is delivered directly to the app. The Content-Type header included in the notification request must be set to "application/octet-stream".</para>
        ///</param>
        ///<param name="options" type="Object">Object that is used to supply the success and error callbacks and other optional behaviors of the notification.</param>
        ///</signature>
    }
}

push = {
    ///<field name="apns" type="apns">An apns object that is used to send push notifications to an iOS app using the Apple Push Notification Service (APNS).</field>
    apns: apns,

    ///<field name="mpns" type="mpns">An mpns object that is used to send push notifications to a Windows Phone 8 app using the Microsoft Push Notification Service (MPNS).</field>
    mpns: mpns,

    ///<field name="wns" type="wns">A wns object that is used to send push notifications to a Windows Store app using Windows Notification Services (WNS).</field>
    wns: wns
}
