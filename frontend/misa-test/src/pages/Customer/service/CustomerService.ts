import {
  CUSTOMER_API,
  CUSTOMER_API_FIND_CUSTOMER,
  CUSTOMER_CHECK_EMAIL,
  CUSTOMER_CHECK_PHONE,
  CUSTOMER_CREATE_CODE,
  CUSTOMER_DELETE_API,
  CUSTOMER_EDIT_API,
  CUSTOMER_IMPORT_FILE_EXCEL,
  CUSTOMER_LOAD_DATA_TABLE,
  CUSTOMER_UPLOAD_IMG,
} from "../../../constant/ApiURL";
import { ApiResponseBase } from "../../../Helper/ApiResponseBase";
import type { PaginationData, Row } from "../../../Model/misaTableModel";
import type { Customer } from "../Model/customer";

export class CustomerService {
  private customerService: ApiResponseBase;

  constructor() {
    this.customerService = new ApiResponseBase(CUSTOMER_API());
  }
  public findCustomerId(id: string) {
    return this.customerService.get<Customer>(CUSTOMER_API_FIND_CUSTOMER(id));
  }
  public createaCustomerCode() {
    return this.customerService.get<string>(CUSTOMER_CREATE_CODE());
  }
  public createCustomer(customer: Customer[]) {
    return this.customerService.post<string>(CUSTOMER_API(), customer);
  }
  public editCustomer(customer_id: string, customer: Customer) {
    return this.customerService.put<string>(
      CUSTOMER_EDIT_API(customer_id),
      customer
    );
  }
  public deleteCustomer(customerId: string[]) {
    return this.customerService.put<string>(CUSTOMER_DELETE_API(), customerId);
  }
  public checkEmail(email: string) {
    return this.customerService.post<string>(CUSTOMER_CHECK_EMAIL(), email);
  }
  public checkPhone(phone: string) {
    return this.customerService.post<string>(CUSTOMER_CHECK_PHONE(), phone);
  }
  public loadDataCustomerTable(paginationData: PaginationData) {
    return this.customerService.post<Row[]>(
      CUSTOMER_LOAD_DATA_TABLE(),
      paginationData
    );
  }
  public importCustomers(excelFile: FormData) {
    return this.customerService.postFormData<Array<Record<string, string>>>(
      CUSTOMER_IMPORT_FILE_EXCEL(),
      excelFile
    );
  }
  public uploadImage(image: FormData) {
    return this.customerService.postFormData<string>(
      CUSTOMER_UPLOAD_IMG(),
      image
    );
  }
}
