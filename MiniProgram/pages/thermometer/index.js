//index.js
//获取应用实例
import {
  format
} from 'timeago.js';

var app = getApp();
var lineChart = null;
let currentIndex = 0;

Page({
  data: {
    name: "",
    thermometers: [],
    indicatorDots: false,
    interval: 2000,
  },

  loadThermometers: function(done) {
    wx.request({
      url: `${app.globalData.host}/api/thermometer/${getApp().globalData.session}`,
      success: res => {
        if (res.statusCode !== 200)
          return;

        let data = res.data;
        data.forEach(d => {
          d.lastUpdate = format(d.lastUpdate, "zh_CN")
          d.batteryPercent = (d.battery - 3.60) / (4.15 - 3.60) * 100;
          if (d.batteryPercent > 100)
            d.batteryPercent = 100;
        })
        this.setData({
          thermometers: res.data,
          indicatorDots: res.data.length > 1
        })

        this.updateName();
      },
      complete: () => {
        if (done)
          done();
      }
    })
  },

  updateName: function() {
    let name = "";
    if (currentIndex < this.data.thermometers.length)
      name = this.data.thermometers[currentIndex].name;

    this.setData({
      name
    });

  },

  swiperChange: function(e) {
    currentIndex = e.detail.current;
    this.updateName();
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
        this.loadThermometers();
      }
    }
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {
    this.loadThermometers();
  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function() {
    this.loadThermometers(() => {
      wx.stopPullDownRefresh();
    });
  },

})