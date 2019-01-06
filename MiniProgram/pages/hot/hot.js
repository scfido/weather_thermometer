// pages/hot/hot.js

var wxCharts = require('./../../utils/wxcharts.js');
var app = getApp();
var lineChart = null;

Page({
  data: {
    motto: 'Hello World',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
  },
  //事件处理函数
  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },


  onLoad: function () {
    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }
  },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },



  //charts

  touchHandler: function (e) {
    console.log(lineChart.getCurrentDataIndex(e));
    lineChart.showToolTip(e, {
      format: function (item, category) {
        return category + ' ' + item.name + ':' + item.data
      }
    });
  },
  createSimulationData: function () {
    var categories = [];
    var data = [];
    for (var i = 0; i < 10; i++) {
      categories.push('2016-' + (i + 1));
      data.push(Math.random() * (20 - 10) + 10);
    }
    // data[4] = null;
    return {
      categories: categories,
      data: data
    }
  },
  updateData: function () {
    var simulationData = this.createSimulationData();
    var series = [{
      name: '成交量1',
      data: simulationData.data,
      format: function (val, name) {
        return val.toFixed(2);
      }
    }];
    lineChart.updateData({
      categories: simulationData.categories,
      series: series
    });
  },
  onLoad: function (e) {
    var windowWidth = windowWidth;
    try {
      var res = wx.getSystemInfoSync();
      windowWidth = res.windowWidth;
    } catch (e) {
      console.error('getSystemInfoSync failed!');
    }

    var simulationData = this.createSimulationData();
    lineChart = new wxCharts({
      canvasId: 'lineCanvas',
      type: 'area',
      animation: true, //是否开启动画
      categories: ['0', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18',
        '19', '20', '21', '22', '23',],
      series: [{
        name: '白天温度',
        data: [15, 20, 22, 21, 22, 21, 18, 20, 22, 21, 22, 21, 16, 20, 22, 21, 22, 21, 18, 20, 22, 21, 22, 21],
      },],
      yAxis: {
        min: 15,
        fontColor: 'rgba(255,255,255,0.35)',
        gridColor: 'rgba(255,255,255,0.35)',
        titleFontColor: 'rgba(255,255,255,0.35)'
      },
      xAxis: {   //是否隐藏x轴分割线
        disableGrid: true,
        fontColor: 'rgba(255,255,255,0)',
      },
      width: windowWidth,
      height: 160,
      dataPointShape: false,
      legend: false, // 是否显示图表下方各类别的标识
      dataLabel: false, // 是否在图表中显示数据内容值
      extra: {
        lineStyle: 'curve',
      },
    });
  }




})

