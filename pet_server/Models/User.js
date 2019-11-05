const mongoose = require('mongoose');
const Pet = require('./Pet');

const UserSchema = new mongoose.Schema({
    username: String,
    password: String,
    email: String,
    active: [mongoose.Types.ObjectId],
    graveyard: [mongoose.Types.ObjectId],
    pets: [Pet.schema]
});

const User = mongoose.model('User', UserSchema);
module.exports = User;