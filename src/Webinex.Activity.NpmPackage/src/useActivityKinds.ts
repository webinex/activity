import React, { useCallback, useMemo, useReducer } from 'react';
import { ActivityClient } from './activityClient';
import { ActivityClientSettings } from './activityClientSettings';
import { ActivityOptions } from './activityOptions';
import { useActivityClient } from './useActivityClient';
import { trackReducer } from './trackReducer';

export interface ActivityKindsState {
  values: string[];
  pending: boolean;
  error?: string;
}

const INITIAL_STATE: ActivityKindsState = {
  values: [],
  pending: true,
  error: undefined,
};

function reducer(state: ActivityKindsState, action: any): ActivityKindsState {
  switch (action.type) {
    case 'fetch/pending': {
      return { ...state, pending: true };
    }

    case 'fetch/fulfilled': {
      const payload: string[] = action.payload;

      return {
        ...state,
        pending: false,
        error: undefined,
        values: payload,
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

export function useFetch(
  dispatch: React.Dispatch<any>,
  client: ActivityClient
) {
  return useCallback(() => {
    dispatch({ type: 'fetch/pending' });
    return client
      .getKinds()
      .then(result => dispatch({ type: 'fetch/fulfilled', payload: result }))
      .catch(error => dispatch({ type: 'fetch/rejecte', payload: error }));
  }, [client, dispatch]);
}

function useSetState(dispatch: React.Dispatch<any>) {
  return useCallback(
    (state: ActivityKindsState) =>
      dispatch({ type: 'set-state', payload: state }),
    [dispatch]
  );
}

export interface UseActivityKindsResult {
  state: ActivityKindsState;
  fetch: ReturnType<typeof useFetch>;
  setState: ReturnType<typeof useSetState>;
}

export function useActivityKinds(
  settings?: ActivityClientSettings,
  options?: ActivityOptions
): UseActivityKindsResult {
  const client = useActivityClient(settings);
  const trackedReducer = trackReducer('kinds', reducer, options?.log);
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
}
