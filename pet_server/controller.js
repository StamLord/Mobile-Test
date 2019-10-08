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

module.exports.register = (req,res) => {
    console.log(req.body);
    const {username, password, email} = req.body;

    User.estimatedDocumentCount({username}, (err, count) => {
        
        if(err) res.sendStatus(500);

        if(count > 0){
            // Username exists
            res.status('409').send({msg: 'Username exists'});
        } else {
            User.count({email}, (err, count) => {
                
                if(err) res.sendStatus(500);

                if(count > 0){
                    // Email exists
                    res.status('409').send({msg: 'Email exists'});
                } else {
                    User.create({
                        username,
                        password,
                        email
                    } , (err, res) => {
                        if(err) res.sendStatus(500);
                        console.log(res);
                    })
                }
            });
        }
    });

    
}