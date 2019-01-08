//app.js
// 7天过期
const SessionExpiredTime = 7 * 24 * 60 * 60 * 1000

App({

  // 登录
  login: function() {

    let now = +new Date()
    let session = wx.getStorageSync("SESSION");

    if (session && now - session.expiredTime <= SessionExpiredTime) {
      this.globalData.session = session.value;
      return;
    }

    wx.login({
      success: res => {
        if (res.code) {
          // 发起网络请求
          wx.request({
            url: this.globalData.host + '/api/account/login/' + res.code,
            success: res => {
              if (res.statusCode === 200) {
                this.globalData.session = res.data.session;

                // 假设登录态保持7天
                var expiredTime = now + SessionExpiredTime;
                wx.setStorageSync('SESSION', {
                  value: res.data.session,
                  expiredTime
                })
              }

              // 由于 login 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.loginCallback) {
                this.loginCallback(res)
              }

            }
          })
        } else {
          console.log('登录失败！' + res.errMsg)
        }
      }
    })
  },

  onLaunch: function() {
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)

    this.login();

    // 获取用户信息
    wx.getSetting({
      success: res => {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
          wx.getUserInfo({
            success: res => {
              // 可以将 res 发送给后台解码出 unionId
              this.globalData.userInfo = res.userInfo

              // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.userInfoReadyCallback) {
                this.userInfoReadyCallback(res)
              }
            }
          })
        }
      }
    })
  },

  globalData: {
    userInfo: null,
    session: null,
    host: "https://ws.fcca.top"
  }
})