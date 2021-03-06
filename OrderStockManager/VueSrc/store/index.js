﻿// Vue関連
import Vue from 'vue'
import Vuex from 'vuex'
// ライブラリ
import createPersistedState from 'vuex-persistedstate'

// アプリケーション
import { actions, mutations } from './mutations'
import { getters } from './getters'
import maintenance from './maintenance'
import auth from './auth'

// 設定
Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    maintenance: maintenance,
    authentication: auth
  },
  actions: actions,
  mutations,
  getters: getters,
  state: {
    breadlist: []
  },
  plugins: [
    createPersistedState({ storage: window.sessionStorage })
  ]
})
