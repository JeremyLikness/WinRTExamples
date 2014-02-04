// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    WinJS.Namespace.define("WordSplitter", {
        splitter: new MyLibrary.WordSplitter()
    });

    var list = {};

    function addItem(word) {
        var item = document.createElement("li");
        item.innerText = word;
        list.appendChild(item);
    }
    
    function clicked() {
        var text = document.getElementById("textToSplit").value;
        list = document.getElementById("splitText");
        list.innerHTML = "";
        var words = WordSplitter.splitter.split(text);
        if (words) {
            var wordList = new WinJS.Binding.List(words);
            wordList.forEach(addItem);
        }
    }

    app.onactivated = function (args) {
        var button = document.getElementById("btnSplit");
        button.onclick = clicked;
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();
