export interface Activity {
  [key: string]: any;

  id: string;
  kind: string;
  operationId: string;
  tenantId?: string;
  userId?: string;
  userName?: string;
  success: boolean;
  performedAt: string;
  parentId?: string;
  values: { [key: string]: any };
}
