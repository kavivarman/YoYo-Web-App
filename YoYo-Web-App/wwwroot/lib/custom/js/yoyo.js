
//Variable declared globally.
var cowndownTimer;
var currentShuttleEndTime;
var cycle = 1;
var totalTime = '0:00';
var currentShuttleTotalSeconds;
var interval;
var currentShuttle;
var previousShuttle = 0;
var timersArray = null;

$(document).ready(function () {
    GetBeepTestData();
});
//Below method to initialize countdown timer when each level/shuttle starts.
function InitializecountdownTimer(timer) {
    var commulativeTime = timer.commulativeTime.split(':');
    var startTime = timer.startTime.split(':');
    var commulativeTimeSeconds = (parseInt(commulativeTime[0], 10) * 60) + parseInt(commulativeTime[1], 10);
    var startTimeSeconds = (parseInt(startTime[0], 10) * 60) + parseInt(startTime[1], 10);
    currentShuttleTotalSeconds = commulativeTimeSeconds - startTimeSeconds;

    if (currentShuttleTotalSeconds > 59) {
        var cowndownTimerSeconds = currentShuttleTotalSeconds % 60;
        var cowndownTimerMinutes = (currentShuttleTotalSeconds - cowndownTimerSeconds) / 60;
        cowndownTimerSeconds = (cowndownTimerSeconds < 10) ? '0' + cowndownTimerSeconds : cowndownTimerSeconds;
        return cowndownTimerMinutes + ':' + cowndownTimerSeconds;
    }
    else {
        currentShuttleTotalSeconds = (cycle == 1) ? currentShuttleTotalSeconds : ++currentShuttleTotalSeconds;
        currentShuttleTotalSeconds = (currentShuttleTotalSeconds < 10) ? '0' + currentShuttleTotalSeconds : currentShuttleTotalSeconds;
        return '0:' + currentShuttleTotalSeconds;
    }
}
//Below method initialize the progress bar when each shuttle/level starts.
function InitializeProgressBar(timer) {
    setTimeout(function () { RotateProgressBar(0); }, 200);
    timer = (timer == null) ? timersArray[0] : timer
    currentShuttle = timer;
    cowndownTimer = InitializecountdownTimer(timer);
    currentShuttleEndTime = timer.commulativeTime.split(':');
    $("#spnTotalDistance").html(timer.accumulatedShuttleDistance);
    $("#divLevel").html('Level ' + timer.speedLevel);
    $("#divShuttle").html('Shuttle ' + timer.shuttleNo);
    $("#divSpeed").html(timer.speed + ' km/h');

}
//Below method is a main method to invoke next shuttles/levels, total time, time left of current shuttle/level
function OnStartTest(timer) {
    InitializeProgressBar(timer);
    if (timer == null)
        $('.test-start').fadeOut(700, function () { $('.test-inprogress, .badge-warn, .badge-stop').fadeIn(1000); });

    interval = setInterval(function () {
        //Below logic to display time left for next shuttle
        var timer = cowndownTimer.split(':');
        var minutes = parseInt(timer[0], 10);
        var seconds = parseInt(timer[1], 10);
        --seconds;
        minutes = (seconds < 0) ? --minutes : minutes;
        seconds = (seconds < 0) ? 59 : seconds;
        seconds = (seconds < 10) ? '0' + seconds : seconds;
        $("#spnLeftTime").html(minutes + ':' + seconds);
        cowndownTimer = minutes + ':' + seconds;

        //Below logic to display total time of the test
        var totaltimer = totalTime.split(':');
        var total_minutes = parseInt(totaltimer[0], 10);
        var total_seconds = parseInt(totaltimer[1], 10);
        total_seconds++;
        total_minutes = (total_seconds > 59) ? ++total_minutes : total_minutes;
        total_seconds = (total_seconds > 59) ? 0 : total_seconds;
        total_seconds = (total_seconds < 10) ? '0' + total_seconds : total_seconds;
        $("#spnTotalTime").html(total_minutes + ':' + total_seconds);
        totalTime = total_minutes + ':' + total_seconds;

        //Below logic to calculate the percentage of test has completed for current shuttle
        var percentage = (currentShuttleTotalSeconds - ((minutes * 60) + parseInt(seconds, 10))) * 100 / currentShuttleTotalSeconds;
        //console.info('Percentage.toFixed(2) is ' + percentage.toFixed(2));
        RotateProgressBar(percentage.toFixed(2));

        //Below logic to invoke the next shuttle based on the timer
        if (total_minutes == parseInt(currentShuttleEndTime[0], 10) && total_seconds >= parseInt(currentShuttleEndTime[1], 10)) {
            if (cycle < timersArray.length) {
                clearInterval(interval);
                previousShuttle = timersArray[cycle - 1];
                OnStartTest(timersArray[cycle++]);
            } else {
                clearInterval(interval);
                OnTestCompleted();
                $('.test-inprogress, .badge-warn, .badge-stop, .div-test-progress').fadeOut(200);
                $('.div-athlete-dropdown, .badge-viewresult, .test-completed').fadeIn(500);
            }
        }
    }, 1000);
}
//Below method to show status in progress bar by passing percentage. Percentage could be 0 to 100 and it converts percentage into degree (0 to 360).
function RotateProgressBar(percent) {
    let degree1 = 0;
    let dergee2 = 0;
    if (percent > 50) {
        degree1 = 180;
        dergee2 = 2 * (percent - 50) * 180 / 100
        document.documentElement.style.setProperty("--degRight", degree1 + "deg");
        document.documentElement.style.setProperty("--degLeft", dergee2 + "deg");
    }
    else {
        degree1 = 2 * (percent) * 180 / 100;
        document.documentElement.style.setProperty("--degRight", degree1 + "deg");
        document.documentElement.style.setProperty("--degLeft", dergee2 + "deg");
    }
}
//API Calls
//Below method fetch json template after page load.
function GetBeepTestData() {
    var doAjax_params = {
        'url': '/Yoyo/GetBeepTestData',
        'requestType': "GET",
        'contentType': 'application/json; charset=UTF-8',
        'dataType': 'json',
        'data': {},
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': SuccessGetBeepTestData,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
    return;
}
//Callback function
function OnErrorAjaxCall(url, error) {
    console.error("error on ajax call url " + url + " error: " + error)
}
//Callback function
function SuccessGetBeepTestData(data) {
    timersArray = data;
}
//Update the warned athlete details by coach
function OnAthleteWarned(event) {
    $(event).attr('disabled', 'disabled').removeAttr('onclick').text('Warned');
    var doAjax_params = {
        'url': $(event).attr('formaction'),
        'requestType': "PUT",
        'contentType': 'application/json',
        'dataType': 'json',
        'data': JSON.stringify({ Level: currentShuttle.speedLevel, Shuttle: currentShuttle.shuttleNo }),
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': null,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
    return;
}
//Update the stopped athlete details by coach
function OnAthleteStopped(event) {
    var LevelShuttle = (previousShuttle == 0) ? '' : previousShuttle.speedLevel + '-' + previousShuttle.shuttleNo;
    var data = { Level: (previousShuttle == 0) ? "0" : previousShuttle.speedLevel, Shuttle: (previousShuttle == 0) ? "0" : previousShuttle.shuttleNo };
    $(event).parents('tr').find('td.tbl-warn button').css("display", "none");
    $(event).css("display", "none");
    $(event).parents('td.tbl-stop').find('div.div-athlete-dropdown select').val(LevelShuttle);
    $(event).parents('td.tbl-stop').find('div.div-athlete-dropdown').css("display", "block");
    $(event).parents('tr').attr("athlete", "stopped");

    if ($("#tblAthletesInfo tbody tr").length == $("#tblAthletesInfo tbody tr[athlete='stopped']").length)
        AllUserStopped();

    var doAjax_params = {
        'url': $(event).attr('formaction'),
        'requestType': "PUT",
        'contentType': 'application/json',
        'dataType': 'json',
        'data': JSON.stringify(data),
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': null,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
    return;
}
//When test has completed and few of the athletes were running.
function OnTestCompleted() {
    var athletesLevelShuttleList = [];
    $("#tblAthletesInfo tbody tr").each(function () {
        let levelShuttle = {};
        if (this.hasAttribute('athlete')) {
            var stoppedLevelShuttle = $(this).find('td.tbl-stop div.div-athlete-dropdown select').val();
            stoppedLevelShuttle = (stoppedLevelShuttle == null) ? "0-0" : stoppedLevelShuttle;
            levelShuttle.LevelShuttleDetail = { Level: stoppedLevelShuttle.split('-')[0], Shuttle: stoppedLevelShuttle.split('-')[1] };
        }
        else {
            levelShuttle.LevelShuttleDetail = { Level: currentShuttle.speedLevel, Shuttle: currentShuttle.shuttleNo };
            $(this).find('td.tbl-stop div.div-athlete-dropdown select').val(currentShuttle.speedLevel + '-' + currentShuttle.shuttleNo);
        }
        levelShuttle.Id = parseInt($(this).find('td.tbl-stop').attr('user-id'));
        athletesLevelShuttleList.push(levelShuttle);

    });
    // console.log('athletesLevelShuttleList - ' + JSON.stringify(athletesLevelShuttleList));
    var doAjax_params = {
        'url': 'Yoyo/OnTestCompleted',
        'requestType': "PUT",
        'contentType': 'application/json',
        'dataType': 'json',
        'data': JSON.stringify(athletesLevelShuttleList),
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': null,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
    return;
}
//Below method updates the level and Shuttle of athletes, When coach changes it in dropdown.
function OnChangelevelShuttle(event) {
    var selectedValue = $(event).val().split('-');
    var data = { Level: selectedValue[0], Shuttle: selectedValue[1] };
    var doAjax_params = {
        'url': $(event).parents('td.tbl-stop').find('button').attr('formaction'),
        'requestType': "PUT",
        'contentType': 'application/json',
        'dataType': 'json',
        'data': JSON.stringify(data),
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': null,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
}
//Below method to show the test completed status, when last athlete stopped by coach.
function AllUserStopped() {
    clearInterval(interval);
    RotateProgressBar(100);
    $('.test-inprogress, .badge-warn, .badge-stop, .div-test-progress').fadeOut(100);
    $('.div-athlete-dropdown, .badge-viewresult, .test-completed').fadeIn(100);
}
//Below method to fetch all athletes completed level and shuttle details
function GetAthletesResult() {
    var doAjax_params = {
        'url': '/Yoyo/ViewResult',
        'requestType': "GET",
        'contentType': 'application/json; charset=UTF-8',
        'dataType': 'json',
        'data': {},
        'beforeSendCallbackFunction': null,
        'successCallbackFunction': SuccessGetAthletesResult,
        'completeCallbackFunction': null,
        'errorCallBackFunction': OnErrorAjaxCall,
    };
    AjaxCall(doAjax_params);
    return;
}
//CallBack function
function SuccessGetAthletesResult(data) {
    console.log("Test Result - " + JSON.stringify(data));
}
//Common API call method
function AjaxCall(doAjax_params) {
    var url = doAjax_params['url'];
    var requestType = doAjax_params['requestType'];
    var contentType = doAjax_params['contentType'];
    var dataType = doAjax_params['dataType'];
    var data = doAjax_params['data'];
    var beforeSendCallbackFunction = doAjax_params['beforeSendCallbackFunction'];
    var successCallbackFunction = doAjax_params['successCallbackFunction'];
    var completeCallbackFunction = doAjax_params['completeCallbackFunction'];
    var errorCallBackFunction = doAjax_params['errorCallBackFunction'];

    $.ajax({
        url: url,
        crossDomain: true,
        type: requestType,
        contentType: contentType,
        dataType: dataType,
        data: data,
        tryCount: 0,
        timeout: 5000,
        retryLimit: 3,
        beforeSend: function (jqXHR, settings) {
            if (typeof beforeSendCallbackFunction === "function") {
                beforeSendCallbackFunction();
            }
        },
        success: function (data, textStatus, jqXHR) {
            if (typeof successCallbackFunction === "function") {
                successCallbackFunction(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus == 'timeout') {
                this.tryCount++;
                if (this.tryCount <= this.retryLimit) {
                    $.ajax(this);
                    return;
                }
            }
            else {
                if (typeof errorCallBackFunction === "function") {
                    errorCallBackFunction(this.url, errorThrown);
                }
            }
        },
        complete: function (jqXHR, textStatus) {
            if (typeof completeCallbackFunction === "function") {
                completeCallbackFunction();
            }
        }
    });
}