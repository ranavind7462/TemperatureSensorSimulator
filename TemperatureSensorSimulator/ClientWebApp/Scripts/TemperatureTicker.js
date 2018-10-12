
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r+"F" : a;
            }
        );
    };
}
$(function () {

    $.connection.hub.url = "http://localhost:8077/signalr";
       var ticker = $.connection.temperatureHub,
        $tempTable = $('#tempTable'),
        $temptableBody = $tempTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{Date}"><td>{Date}</td><td>{TemperatureValue}</td></tr>';

    function formatTemperature(temp) {
        return $.extend(temp, {
            Date: temp.Date,
            Temperature: temp.TemperatureValue      
        });
    }
    function init() {
        ticker.server.getTemperature().done(function (temps) {
            $temptableBody.empty();
            $.each(temps, function () {
                var temp = formatTemperature(this);
                $temptableBody.append(rowTemplate.supplant(temp));
            });
        });
    }
    ticker.client.loadTemperature = function (temp) {

        var displayTemp = formatTemperature(temp),
            $row = $(rowTemplate.supplant(displayTemp));
        $temptableBody.append(rowTemplate.supplant(displayTemp));
    };

    
    $.connection.hub.start().done(init);

    //Enabling the client logging. it will log the information in the browser console.
    $.connection.hub.logging = true;
    $.connection.hub.connectionSlow(function () {
        console.log('we are currently experiencing difficulties with the connection');
    });
    var tryingToReconnect = false;
    $.connection.hub.reconnecting(function () {
        tryingToReconnect = true;
    });
    $.connection.hub.disconnected(function () {
        if (tryingToReconnect) {
            $("#lbl1").html("Server Down");
            setTimeout(function () {
                $.connection.hub.start();
            }, 5000);
        }
        else {
            $("#lbl1").html("");
        }
    });
    $.connection.hub.reconnected(function () {
        tryingToReconnect = false;
    });
});