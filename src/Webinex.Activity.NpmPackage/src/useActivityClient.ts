import { useMemo } from 'react';
import { ActivityClient } from './activityClient';
import { ActivityClientSettings } from './activityClientSettings';

export function useActivityClient(settings?: ActivityClientSettings) {
  return useMemo(() => new ActivityClient(settings), [settings]);
}
