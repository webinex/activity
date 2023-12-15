import { FilterRule, PagingRule, SortRule } from '@webinex/asky';

export type ActivityField =
  | 'id'
  | 'operationId'
  | 'kind'
  | 'parentId'
  | 'tenantId'
  | 'userId'
  | 'success'
  | 'performedAt'
  | string;

export interface ActivityListRequest {
  includeTotal?: boolean;
  paging?: PagingRule;
  sort?: SortRule<ActivityField>;
  filter?: FilterRule<ActivityField>;
}
