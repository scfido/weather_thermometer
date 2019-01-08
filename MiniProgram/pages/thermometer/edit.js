// pages/thermometer/edit.js
var app = getApp();

Page({

  /**
   * 页面的初始数据
   */
  data: {
    id: 0,
    sn: "disp_",
    name: "我的温度计",
    method: "POST"
  },

  delete() {
    let url = `${app.globalData.host}/api/thermometer/${getApp().globalData.session}/${this.data.sn}`;
    wx.showLoading({
      title: '删除中',
    })

    wx.request({
      url,
      method: "DELETE",
      success: res => {},

      fail: () => {
        wx.showToast({
          title: '删除失败',
        })

        setTimeout(() => {
          wx.hideToast()
        }, 2000)
      },
      complete: () => {
        wx.hideLoading();
        wx.navigateBack();
      }
    })
  },

  formSubmit(e) {
    let url;
    let method = this.data.method;

    if (method === "PUT") {
      url = `${app.globalData.host}/api/thermometer/${getApp().globalData.session}/${this.data.id}`;
    } else { //POST
      url = `${app.globalData.host}/api/thermometer/${getApp().globalData.session}/${e.detail.value.sn}`;
    }

    wx.showLoading({
      title: '保存中',
    })

    wx.request({
      url,
      method,
      data: e.detail.value,
      success: res => {
        console.log(res.statusCode);
      },

      fail: () => {
        wx.showToast({
          title: '保存失败',
        })

        setTimeout(() => {
          wx.hideToast()
        }, 2000)
      },
      complete: () => {
        wx.hideLoading();
        wx.navigateBack();
      }

    })
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    let method = options.title === "编辑" ? "PUT" : "POST";
    if (method === "PUT") {
      this.setData({
        id: options.id,
        sn: options.sn,
        name: options.name,
        method
      });
    } else {
      this.setData({
        method
      });
    }


    wx.setNavigationBarTitle({
      title: options.title
    })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function() {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function() {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function() {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function() {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function() {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function() {

  }
})