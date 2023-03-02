mergeInto(LibraryManager.library, {

  _GameStart: function () {
    window.GameStart();
  },

  _GameEnd: function (score) {
    window.GameEnd(score);
  },

  _GameSave: function (data) {
    window.GameSave(UTF8ToString(data));
  },

  _GameGetData: function () {
    window.GameGetData();
  },

  _ShowRewardAd: function () {
    window.ShowRewardAd();
  },

  _DebugOut: function (str) {
    window.alert(UTF8ToString(str));
  },



});
