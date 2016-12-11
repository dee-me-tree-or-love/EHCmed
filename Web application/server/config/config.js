const path = require('path')

let rootPath = path.normalize(path.join(__dirname, '/../../'))

module.exports = {
  development: {
    rootPath: rootPath,
    // db: 'mongodb://localhost:27017/express-db',
    port: 1337
  },
  production: {
    rootPath: rootPath,
    // db: process.env.MONGO_DB_CONN_STR,
    port: process.env.port
  }
}
