// const passport = require('passport')
// const LocalPassport = require('passport-local')
// let sqlite3 = require('sqlite3')
// let db = new sqlite3.Database('../../database/EHCDB.db')

// module.exports = () => {
//   passport.use(new LocalPassport(function (username, password, done) {
//     db.get('SELECT Name, id FROM PersonalData WHERE Name = ? AND password = ?', username, passport, (err, row) => {
//       if (err) {
//         console.log('some errror in passport config')
//       }
//       if (!row) return done(null, false)
//       return done(null, row)
//     })
//   }))

//   passport.serializeUser((user, done) => {
//     return done(null, user.id)
//   })

//   passport.deserializeUser((id, done) => {
//     db.get('SELECT id, Name FROM users WHERE id = ?', id, (err, row) => {
//       if (!row) return done(null, false)
//       return done(null, row)
//     })
//   })
// }
