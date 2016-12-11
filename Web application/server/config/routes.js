const controllers = require('../controllers')
// const auth = require('../config/auth')
module.exports = (app) => {
  app.get('/', controllers.home.index)

  // app.get('/about', controllers.home.about)

  // app.get('/users/register', controllers.users.register)

  // app.get('/users/login', controllers.users.login)

  // app.post('/users/login', controllers.users.authenticate)

  // app.post('/users/create', controllers.users.create)
  // app.post('/users/logout', controllers.users.logout)

  // app.get('/articles/create', auth.isAuthenticated, controllers.articles.create)
  app.all('*', (req, res) => {
    res.status(404)
    res.send('not found')
    res.end()
  })
}
