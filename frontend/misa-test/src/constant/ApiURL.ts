const apiUrl = `${import.meta.env.VITE_API_PROTOCOL}://${
  import.meta.env.VITE_API_HOST
}:${import.meta.env.VITE_API_PORT}/api`;

export const CUSTOMER_API = () => `${apiUrl}/Customer`;
export const CUSTOMER_API_FIND_CUSTOMER = (customerId: string) =>
  `${CUSTOMER_API()}/${customerId}`;
export const CUSTOMER_EDIT_API = (customer_id: string) =>
  `${CUSTOMER_API()}/${customer_id}`;
export const CUSTOMER_DELETE_API = () => `${CUSTOMER_API()}/delete`;
export const CUSTOMER_CREATE_CODE = () => `${CUSTOMER_API()}/create-code`;
export const CUSTOMER_CHECK_EMAIL = () => `${CUSTOMER_API()}/check-email`;
export const CUSTOMER_CHECK_PHONE = () => `${CUSTOMER_API()}/check-phone`;
export const CUSTOMER_LOAD_DATA_TABLE = () => `${CUSTOMER_API()}/data-table`;
export const CUSTOMER_IMPORT_FILE_EXCEL = () =>
  `${CUSTOMER_API()}/import-excel`;
export const CUSTOMER_UPLOAD_IMG = () => `${CUSTOMER_API()}/update-image`;
