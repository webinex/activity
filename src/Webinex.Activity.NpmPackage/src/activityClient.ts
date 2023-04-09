import { AxiosInstance } from 'axios';
import { Activity } from './activity';
import {
  ActivityClientSettings,
  DEFAULT_ACTIVITY_SETTINGS,
} from './activityClientSettings';
import { ActivityListRequest } from './activityListRequest';
import { ActivityList } from './activityList';

export class ActivityClient {
  private _axios: AxiosInstance;

  constructor(settings: ActivityClientSettings = {}) {
    settings = { ...DEFAULT_ACTIVITY_SETTINGS, ...settings };
    this._axios = settings.axios!;
  }

  public getAll = async (
    request: ActivityListRequest
  ): Promise<ActivityList> => {
    if (request == null) throw new Error('`request` might not be null');

    const queryString = this.queryString(request);
    const response = await this._axios.get<ActivityList>(`?${queryString}`);
    return response.data;
  };

  private queryString = (request: ActivityListRequest) => {
    const result = [];

    function push(key: string, value: any) {
      result.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
    }

    if (request.filter) {
      push('filter', JSON.stringify(request.filter));
    }

    if (request.includeTotal != null) {
      push('includeTotal', request.includeTotal);
    }

    if (request.paging) {
      push('paging', JSON.stringify(request.paging));
    }

    if (request.sort) {
      push('sort', JSON.stringify(request.sort));
    }

    return result.join('&');
  };

  public get = async (id: string): Promise<Activity> => {
    if (id == null) throw new Error('`id` might not be null');

    const response = await this._axios.get<Activity>(id);
    return response.data;
  };

  public getKinds = async (): Promise<string> => {
    const response = await this._axios.get<string>('kinds');
    return response.data;
  };
}
