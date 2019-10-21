const router = require('express').Router();
const controller = require('./controller');

router.get('/:username', controller.getUser);

router.post('/register', controller.register);

router.post('/login', controller.login);

router.put('/:username/pet', controller.createPet);

router.put('/:username/active', controller.updateActive);

router.post('/pet', controller.updatePet);

module.exports = router;