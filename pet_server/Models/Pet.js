const mongoose = require('mongoose');

const PetSchema = new mongoose.Schema({
    id: Number,
    species: String,
    nickname: String,
    hunger: Number,
    hungerStamp: Number,
    strength: Number,
    strengthStamp: Number,
    attention: Number,
    attentionStamp: Number,
});

const Pet = mongoose.model('Pet', PetSchema);
module.exports = Pet;