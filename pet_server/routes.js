const router = require('express').Router();
const controller = require('./controller');

router.get('/:username', controller.getUser);

router.post('/register', controller.register);

module.exports = router;