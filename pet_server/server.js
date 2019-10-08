const express = require('express');
const app = express();
const mongoose = require('mongoose');
const routes = require('./routes');
const config = require('./config');


mongoose.connect(config.DB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true
});

mongoose.connection.once('open', () => {

    console.log('Connected to database');
    
    app.use('/api', routes);
    app.listen('5000', () => console.log('Server is running'));
})
