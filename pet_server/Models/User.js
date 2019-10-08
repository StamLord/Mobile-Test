const mongoose = require('mongoose');
const Pet = require('./Pet');

const UserSchema = new mongoose.Schema({
    username: String,
    password: String,
    email: String,
    pets: [Pet.schema]
});

const User = mongoose.model('User', UserSchema);
module.exports = User;