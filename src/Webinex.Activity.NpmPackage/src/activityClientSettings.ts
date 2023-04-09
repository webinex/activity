import axios, { AxiosInstance } from 'axios';

export const DEFAULT_ACTIVITY_SETTINGS: ActivityClientSettings = {
  axios: axios.create({ baseURL: '/api/activity' }),
};

export interface ActivityClientSettings {
  axios?: AxiosInstance;
}
