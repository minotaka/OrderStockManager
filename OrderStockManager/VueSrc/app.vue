﻿<template lang="pug">
div
  el-menu(mode="horizontal" :router="true")
    el-menu-item(:index="homePath" v-text="title")
    el-menu-item(index="signin" v-show="!isAuthenticated") サインイン
    el-submenu(index="acount" v-show="isAuthenticated")
      template(slot="title") {{fullName}}
      el-menu-item(index="/" @click="signout") サインアウト
  #content(v-loading="loading" element-loading-text="サインアウト中...")
    el-row(style="padding-bottom:8px;")
      el-col.bread(:span="24")
        el-breadcrumb(separator="/")
          el-breadcrumb-item(v-for="(item,index) in breadlist" :to="{path: item.path}")
            i.material-icons(v-show="index===0" style="font-size:13px;") &#xE88A;
            |{{item.name}}
    router-view
  footer#footer
    |Copyright&copy; 2016,2017 -
    a(href="https://github.com/minotaka" target="new") Minoru Takayama
    |All Rights Reserved.
</template>

<script>
import { generateUUID } from './libraries/generateUUID'
export default {
  metaInfo: {
    title: '',
    titleTemplate: '%s - 発注在庫管理 for arcane'
  },
  data() {
    return {
      title: "発注在庫管理 for arcane",
      loading: false
    }
  },
  computed: {
    isAuthenticated: function () {
      return this.$store.getters.isAuthenticated
    },
    fullName: function () {
      return this.$store.getters['fullName']
    },
    homePath: function () {
      if (this.$store.getters.isAuthenticated) {
        return "/menu"
      } else {
        return "/"
      }
    },
    breadlist: function () {
      return this.$store.getters.getBreadlist
    }
  },
  methods: {
    goHome() {
      if (!this.$store.getters.isAuthenticated) {
        this.$router.push('/')
      } else {
        this.$router.push('/menu')
      }
    },
    signout() {
      this.loading = true
      this.$store.dispatch('signOut').then(() => {
        this.loading = false
        this.$router.push('/')
      })
    }
  },
  created: function () {
    if (!this.$store.getters.isAuthenticated) {
      this.$ls.set("bearer", null)
      this.$ls.set("refresh", null)
    }
    if (this.$ls.get("clientId", null) == null) {
      this.$ls.set("clientId", generateUUID())
    }
  }
}
</script>

<style lang="scss" scoped>
#content {
  margin: 8px 8px 25px+8px 8px;
}

#footer {
  position: fixed;
  bottom: 0;
  width: 100%;
  height: 25px;
  background-color: lightgray;
  text-align: center;
  vertical-align: central;
}

.bread {
  background-color: #EFF2F7;
  border-radius: 4px 4px 0 0;
  padding: 8px 0 8px 4px;
}
</style>
