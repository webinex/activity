export function trackReducer<TState>(
  kind: string,
  reducer: (state: TState, action: any) => TState,
  log: ((...args: any[]) => any) | null | undefined
): (state: TState, action: any) => TState {
  if (!log) {
    return reducer;
  }

  return (state: TState, action: any): TState => {
    log(`[Activity (${kind})]: ${action.type}`, action);
    const result = reducer(state, action);

    if (state !== result) {
      log(`[Activity (${kind})]: state changed`, result);
    }

    return result;
  };
}
