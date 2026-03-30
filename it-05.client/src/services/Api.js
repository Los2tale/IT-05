import axios from 'axios';

export default () => {
  axios.defaults.headers.common['Access-Control-Allow-Origin'] = '*';

  return axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7035',
  })
};
