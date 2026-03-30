import api from './Api';

export default {
  post() {
    return api().post('/Queue');
  },
  get() {
    return api().get('/Queue');
  },
  delete() {
    return api().delete('/Queue');
  }
};
