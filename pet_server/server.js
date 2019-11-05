const express = require('express');
const app = express();
const mongoose = require('mongoose');
const routes = require('./routes');
const config = require('./config');


mongoose.connect(config.DB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true
}).catch((err) => console.log(err));

mongoose.connection.once('open', () => {

    console.log('Connected to database');
    
    app.use(express.json());
    app.use('/api', routes);
    app.listen(config.PORT, () => console.log(`Server is running on ${config.PORT}`));
})
