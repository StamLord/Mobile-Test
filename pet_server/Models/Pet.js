const mongoose = require('mongoose');

const PetSchema = new mongoose.Schema({
    species: String,
    treeName: String,
    
    stage: Number,
    stageStamp: Number,
    
    birth: Number,
    nickname: String,

    longetivity: Number,

    careMistakes: Number,
    careMistakeCost: Number,

    weight: Number,
    minWeight: Number,
    maxWeight: Number,
    starveAt: Number,

    hunger: Number,
    hungerStamp: Number,
    hungerRate: Number,

    strength: Number,
    strengthStamp: Number,
    strengthRate: Number,

    attention: Number,
    attentionStamp: Number,
    attentionRate: Number,

    happiness: Number,
    happinessRate: Number,
    happinessStamp: Number,

    discipline: Number,
    disciplineRate: Number,
    disciplineStamp: Number,

    misbehaveStamp: Number,

    energy: Number,
    energyRecoveryRate: Number,
    energyStamp: Number,

    injury: Number,
    injuryRecoveryRate: Number,
    injuryStamp: Number,

    sleepStamp: Number,
    sleepHours: Number,

    s_atk: Number,
    s_spd: Number,
    s_def: Number,

    g_atk: Number,
    g_spd: Number,
    g_def: Number,

    t_atk: Number,
    t_spd: Number,
    t_def: Number,

});

const Pet = mongoose.model('Pet', PetSchema);
module.exports = Pet;