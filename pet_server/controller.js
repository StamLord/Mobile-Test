const ObjectId = require('mongoose').Types.ObjectId;
const User = require('./Models/User');
const Pet = require('./Models/Pet');

module.exports.getUser = (req,res) => {

    User.findOne({username: req.params.username}, (err, result) => {
        if(err) {
            res.sendStatus(500);
        } else {
            if(result){
                let {username, pets} = result;
                const user = {username, pets};

                res.send(user);
            }
            else
                res.sendStatus(404);
        }
    });
}

module.exports.register = (req, res) => {
    const {username, password, email} = req.body;

    User.countDocuments({username}, (err, count) => {
        
        if(err) {
            res.sendStatus(500);
            return;
        }

        if(count > 0){
            // Username exists
            res.status('409').send({msg: 'Username exists'});
        } else {
            User.count({email}, (err, count) => {
                
                if(err) {
                    res.sendStatus(500);
                    return;
                }

                if(count > 0){
                    // Email exists
                    res.status('409').send({msg: 'Email exists'});
                } else {
                    User.create({
                        username,
                        password,
                        email
                    } , (err, result) => {
                        if(err) {
                            res.sendStatus(500);
                            return;
                        }
                        res.sendStatus(201);
                    });
                }
            });
        }
    });
}

module.exports.login = (req, res) => {
    
    if(!req.body){ 
        res.sendStatus(404);
        return;
    }

    const {username, password} = req.body;
    console.log(req.body);
    User.findOne({username, password}, (err, result) =>{
        if(err) {
            res.sendStatus(500);
            return;
        }

        if(result) {
            let user = result;
            user.password = undefined;
            
            res.send(user);
        } else {
            res.status(404).send({msg: 'Invalid credentials'});
        }
    })
}

module.exports.createPet = (req, res) => {
    if(!req.body){ 
        res.sendStatus(404);
        return;
    }

    //#region Deconstruct

    const{species, treeName, stage, stageStamp,
        birth, nickname, longetivity,
        careMistakes, careMistakeCost,
        weight, starveAt, 
        hunger, hungerStamp, hungerRate,
        strength, strengthStamp, strengthRate,
        attention, attentionStamp, attentionRate,
        happiness, happinessRate, happinessStamp,
        discipline, disciplineRate, disciplineStamp,
        energy, energyRecoveryRate, energyStamp,
        s_atk, s_spd, s_def,
        g_atk, g_spd, g_def,
        t_atk, t_spd, t_def} = req.body;

    //#endregion

    // User.findOne({ username: req.params.username}, 
    // (err, user) => {
    //     if(err) {
    //         res.sendStatus(500);
    //         return;
    //     }

    //     if(user)
    //     {
            const _id = ObjectId();
            console.log(req.body);
            console.log("Finding user and creating Pet...");
            User.findOneAndUpdate({username: req.params.username}, {$push: {pets: new Pet({
                    _id,
                    species, treeName, stage, stageStamp,
                    birth, nickname, longetivity,
                    careMistakes, careMistakeCost,
                    weight, starveAt, 
                    hunger, hungerStamp, hungerRate,
                    strength, strengthStamp, strengthRate,
                    attention, attentionStamp, attentionRate,
                    happiness, happinessRate, happinessStamp,
                    discipline, disciplineRate, disciplineStamp,
                    energy, energyRecoveryRate, energyStamp,
                    s_atk, s_spd, s_def,
                    g_atk, g_spd, g_def,
                    t_atk, t_spd, t_def
            })}}, {upsert: true}, 
                (err, result) => {
                    if(err){
                        res.status(500).send({msg:"Error pushing to user"});
                    } else if (result){
                        res.status(201).send({_id});
                    }
            });
            
    //     } else {
    //         res.status(500).send({msg: 'Error creating resource'});
    //     }
    // });
}

module.exports.updatePet = (req, res) => {
    
    //#region Deconstruct

    const{_id, species, treeName, stage, stageStamp,
        birth, nickname, longetivity,
        careMistakes, careMistakeCost,
        weight, starveAt, 
        hunger, hungerStamp, hungerRate,
        strength, strengthStamp, strengthRate,
        attention, attentionStamp, attentionRate,
        happiness, happinessRate, happinessStamp,
        discipline, disciplineRate, disciplineStamp,
        energy, energyRecoveryRate, energyStamp,
        s_atk, s_spd, s_def,
        g_atk, g_spd, g_def,
        t_atk, t_spd, t_def} = req.body;

    //#endregion
    User.findOneAndUpdate({'pets._id' : _id}, {
        $set: {'pets.$': {
                _id,
                species, treeName, stage, stageStamp,
                birth, nickname, longetivity,
                careMistakes, careMistakeCost,
                weight, starveAt, 
                hunger, hungerStamp, hungerRate,
                strength, strengthStamp, strengthRate,
                attention, attentionStamp, attentionRate,
                happiness, happinessRate, happinessStamp,
                discipline, disciplineRate, disciplineStamp,
                energy, energyRecoveryRate, energyStamp,
                s_atk, s_spd, s_def,
                g_atk, g_spd, g_def,
                t_atk, t_spd, t_def
            }
    }}, (err, result) => {
        if(err){
            res.sendStatus(500);
            return;
        }
        res.status(201).send({msg: 'Success'});
    })
}