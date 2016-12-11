const controllers = require('../controllers')
// const auth = require('../config/auth')
module.exports = (app) => {
  app.get('/', controllers.home.index)

  // app.get('/main', controllers.main)

  app.all('*', (req, res) => {
    res.status(404)
    res.send('not found')
    res.end()
  })
}
