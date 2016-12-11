const express = require('express')

const bodyParser = require('body-parser')


module.exports = (config, app) => {
  app.set('view engine', 'pug')
  app.set('views', config.rootPath + 'server/views')

  app.use(bodyParser.urlencoded({ extended: true }))

  app.use((req, res, next) => {
    if (req.user) {
      res.locals.currentUser = req.user
    }
    next()
  })
  app.use(express.static(config.rootPath + 'public'))
}
