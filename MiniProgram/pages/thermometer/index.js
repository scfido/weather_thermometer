//index.js
//获取应用实例
import {
  format
} from 'timeago.js';

var app = getApp();
var lineChart = null;

Page({
  data: {
    thermometers: [],

    background: ['demo-text-1', 'demo-text-2', 'demo-text-3'],
    indicatorDots: false,
    vertical: false,
    autoplay: false,
    circular: false,
    interval: 2000,
    duration: 500,
    previousMargin: 0,
    nextMargin: 0,
  },

  //charts

  touchHandler: function(e) {
    console.log(lineChart.getCurrentDataIndex(e));
    lineChart.showToolTip(e, {
      format: function(item, category) {
        return category + ' ' + item.name + ':' + item.data
      }
    });
  },

  createSimulationData: function() {
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

  updateData: function() {
    var simulationData = this.createSimulationData();
    var series = [{
      name: '成交量1',
      data: simulationData.data,
      format: function(val, name) {
        return val.toFixed(2);
      }
    }];
    lineChart.updateData({
      categories: simulationData.categories,
      series: series
    });
  },

  onLoad: function(e) {
    // 由于 login 是网络请求，可能会在 Page.onLoad 之后才返回
    // 所以此处加入 callback 以防止这种情况
    app.loginCallback = res => {
      if (res.statusCode === 200) {
        wx.request({
          url: `${app.globalData.host}/api/thermometer/${getApp().globalData.session}`,
          success: res => {
            if (res.statusCode !== 200)
              return;

            let data = res.data;
            data.forEach(d => {
              d.lastUpdate = format(d.lastUpdate, "zh_CN")
              d.batteryPercent = d.battery - 3.60 / (4.20 - 3.60);
            })
            this.setData({
              thermometers: res.data,
              indicatorDots: res.data.length > 1
            })
          }
        })
      }
    }
  }
})