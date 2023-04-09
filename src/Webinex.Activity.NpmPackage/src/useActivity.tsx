import React, { useCallback, useMemo, useReducer } from 'react';
import { Activity } from './activity';
import { ActivityClient } from './activityClient';
import { ActivityClientSettings } from './activityClientSettings';
import { ActivityOptions } from './activityOptions';
import { useActivityClient } from './useActivityClient';
import { trackReducer } from './trackReducer';

export interface ActivityState {
  value?: Activity;
  error?: string;
  pending: boolean;
}

const INITIAL_STATE: ActivityState = {
  value: undefined,
  error: undefined,
  pending: true,
};

export interface UseActivityResult {
  state: ActivityState;
  fetch: () => Promise<any>;
  setState: (state: ActivityState) => any;
}

function reducer(state: ActivityState, action: any): ActivityState {
  switch (action.type) {
    case 'fetch/pending': {
      return { ...state, pending: true };
    }

    case 'fetch/fulfilled': {
      const payload: Activity = action.payload;

      return {
        ...state,
        pending: false,
        error: undefined,
        value: payload,
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

function useFetch(
  dispatch: React.Dispatch<any>,
  id: string,
  client: ActivityClient
) {
  return useCallback(() => {
    dispatch({ type: 'fetch/pending' });
    return client
      .get(id)
      .then(result => dispatch({ type: 'fetch/fulfilled', payload: result }))
      .catch(error => dispatch({ type: 'fetch/rejected', payload: error }));
  }, [dispatch, id, client]);
}

function useSetState(dispatch: React.Dispatch<any>) {
  return useCallback(
    (state: ActivityState) => dispatch({ type: 'set-state', payload: state }),
    [dispatch]
  );
}

export const useActivity = (
  id: string,
  settings?: ActivityClientSettings,
  options?: ActivityOptions
): UseActivityResult => {
  if (!id) throw new Error('`id` might not be null');

  const client = useActivityClient(settings);
  const trackedReducer = trackReducer('one', reducer, options?.log);
  const [state, dispatch] = useReducer(trackedReducer, INITIAL_STATE);
  const fetch = useFetch(dispatch, id, client);
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
