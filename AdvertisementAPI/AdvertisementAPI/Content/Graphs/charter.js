var IDs = [];
var VIEWS = [];
var CLICKS = [];
var adQuantity = 0;

function InitializeChart()
{
    var options = {
        series: [{
            name: 'Views',
            data: VIEWS
        }, {
            name: 'Clicks',
            data: CLICKS
        }],
        chart: {
            type: 'bar',
            height: 350
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '75%',
                endingShape: 'rounded'
        },
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            width: 2,
            colors: ['transparent']
        },
        xaxis: {
            categories: IDs//['Samsung', 'Aston Martin', 'Uzum', 'Payme'],
        },
        yaxis: {
            title: {
                text: 'Activity'
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val;
                }
            }
        }
    };
    
    var chart = new ApexCharts(document.querySelector("#chart"), options);
    chart.render();
}

function GetName(adID)
{
    return new Promise((resolve) => {
        fetch("http://localhost:5000/api/AD/getAD?adID=" + adID).then(response =>{
        return response.json();
    }).then(data =>{
        resolve(data.client);
    }).catch(error => {
        console.log("ErrorLoadingAdInfo;");
    })
    });
    /*return Promise.resolve(fetch("http://localhost:5000/api/AD/getAD?adID=" + adID).then(response =>{
        return response.json();
    }).then(data =>{
        return data.client;
    }).catch(error => {
        console.log("ErrorLoadingAdInfo;");
    }));*/
}

async function Fetcher()
{
    fetch("http://localhost:5000/api/AD/getStatistic").then(response => {
    return response.json();
}).then(async data => {
    adQuantity = data.length;

    for(var i = 0; i < data.length; i++)
    {
        var clientName = await GetName(data[i].statid.toString());
        IDs.push(clientName);
        //IDs.push(data[i].statid.toString());
        VIEWS.push(data[i].views);
        CLICKS.push(data[i].clicks);
    }

    InitializeChart();
}).catch(error => {
    console.log("ErrorLoadingStatistics" + error + ";");
});

}

Fetcher();
/*
fetch("http://localhost:5000/api/AD/getStatistic").then(response => {
    return response.json();
}).then(async data => {
    adQuantity = data.length;

    for(var i = 0; i < data.length; i++)
    {
        await IDs.push(GetName(data[i].statid.toString()));
        //IDs.push(data[i].statid.toString());
        VIEWS.push(data[i].views);
        CLICKS.push(data[i].clicks);
    }

    InitializeChart();
}).catch(error => {
    console.log("ErrorLoadingStatistics" + error + ";");
});
*/