const User = require('./Models/User');

module.exports.getUser = (req,res) => {
    // res.send({
    //     username: req.params.username,
    //     collection: [
    //         {nickname: 'petnick'},
    //         {nickname: 'petnick2'},
    //     ]
    // });

    User.findOne({username: req.params.username}, (err, result) => {
        if(err) {
            res.sendStatus(500);
        } else {
            if(result)
                res.send(result);
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