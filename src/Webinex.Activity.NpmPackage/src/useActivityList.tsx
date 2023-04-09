import React, { useCallback, useMemo, useReducer } from 'react';
import { Activity } from './activity';
import { ActivityClient } from './activityClient';
import { ActivityClientSettings } from './activityClientSettings';
import { ActivityListRequest } from './activityListRequest';
import { ActivityList } from './activityList';
import { useActivityClient } from './useActivityClient';
import { trackReducer } from './trackReducer';
import { ActivityOptions } from './activityOptions';

export interface ActivityListState {
  pending: boolean;
  error?: string;

  total: number;
  items: string[];
  itemById: {
    [key: string]: Activity;
  };
}

const INITIAL_STATE: ActivityListState = {
  pending: true,
  total: -1,
  items: [],
  itemById: {},
};

function reducer(state: ActivityListState, action: any): ActivityListState {
  switch (action.type) {
    case 'fetch/pending': {
      return { ...state, pending: true };
    }

    case 'fetch/fulfilled': {
      const payload: ActivityList = action.payload;
      const args: ActivityListRequest = action.meta.args;

      const itemById = payload.items.reduce((prev, item) => {
        prev[item.id] = item;
        return prev;
      }, {} as ActivityListState['itemById']);

      return {
        ...state,
        pending: false,
        error: undefined,
        itemById,
        items: Object.keys(itemById),
        total: args.includeTotal ? payload.total : state.total,
      };
    }

    case 'fetch/rejected': {
      return {
        ...state,
        pending: false,
        error: action.payload.toString(),
      };
    }

    case 'set-state': {
      return action.payload;
    }

    default:
      return state;
  }
}

function useFetch(dispatch: React.Dispatch<any>, client: ActivityClient) {
  function fetch(args: ActivityListRequest) {
    const meta = { args };

    dispatch({ type: 'fetch/pending', meta });
    return client
      .getAll(args)
      .then(payload => dispatch({ type: 'fetch/fulfilled', payload, meta }))
      .catch(error =>
        dispatch({ type: 'fetch/rejected', payload: error, meta })
      );
  }

  return useCallback(fetch, [dispatch, client]);
}

function useSetState(dispatch: React.Dispatch<any>) {
  return useCallback(
    (state: ActivityListState) =>
      dispatch({ type: 'set-state', payload: state }),
    [dispatch]
  );
}

export interface UseActivityListResult {
  state: ActivityListState;
  fetch: ReturnType<typeof useFetch>;
  setState: ReturnType<typeof useSetState>;
}

export const useActivityList = (
  settings?: ActivityClientSettings,
  options?: ActivityOptions
): UseActivityListResult => {
  const client = useActivityClient(settings);
  const trackedReducer = trackReducer('list', reducer, options?.log);
  const [state, dispatch] = useReducer(trackedReducer, INITIAL_STATE);
  const fetch = useFetch(dispatch, client);
  const setState = useSetState(dispatch);

  return useMemo(
    () => ({
      state,
      fetch,
      setState,
    }),
    [state, fetch, setState]
  );
};
