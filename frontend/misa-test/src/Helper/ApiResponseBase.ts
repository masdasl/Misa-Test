import type { AxiosError, AxiosInstance } from "axios";
import axios from "axios";
import type { ApiResponse } from "../Model/ApiResponse";

export class ApiResponseBase {
  protected instance: AxiosInstance;
  constructor(url: string) {
    this.instance = axios.create({
      baseURL: url,
      timeout: 10000,
      headers: {
        "Content-Type": "application/json",
      },
    });
    this.setupInterceptors();
  }

  // -------------------------------------------------------
  // Công dụng: Thiết lập interceptor cho axios instance để
  //            xử lý phản hồi (response) và lỗi (error) từ server.
  //            - Khi nhận response thành công: trả về nguyên AxiosResponse.
  //            - Khi gặp lỗi: reject trực tiếp lỗi gốc của Axios.
  // Đầu vào: Không có
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  private setupInterceptors() {
    this.instance.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        return Promise.reject(error);
      }
    );
  }

  // -------------------------------------------------------
  // Công dụng: Chuyển một chuỗi từ camelCase hoặc PascalCase sang snake_case
  // Đầu vào:
  //        str: string - chuỗi cần chuyển đổi
  // Trả về: string - chuỗi đã được chuyển sang snake_case
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/19/2025
  // -------------------------------------------------------
  private toSnakeCase(str: string): string {
    return str
      .replace(/([A-Z])/g, "_$1")
      .toLowerCase()
      .replace(/^_/, "");
  }

  // -------------------------------------------------------
  // Công dụng: Chuyển một chuỗi snake_case sang camelCase
  // Đầu vào:
  //        str: string - chuỗi cần chuyển đổi
  // Trả về:
  //        string - chuỗi camelCase
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/20/2025
  // -------------------------------------------------------
  private toCamelCase(str: string) {
    // Tìm các đoạn "_[chữ cái]" và thay bằng chữ cái viết hoa
    // Ví dụ: "customer_code" → "customerCode"
    return str.replace(/_([a-z])/g, (_, c) => c.toUpperCase());
  }
  // -------------------------------------------------------
  // Công dụng: Đệ quy chuyển tất cả key trong object hoặc array sang snake_case
  // Đầu vào:
  //        obj: any - object hoặc array cần chuyển đổi
  // Trả về: any - object/array đã được chuyển key sang snake_case
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/19/2025
  // -------------------------------------------------------
  private keysToSnakeCase(obj: any): any {
    if (Array.isArray(obj)) {
      return obj.map(this.keysToSnakeCase.bind(this));
    } else if (obj !== null && typeof obj === "object") {
      return Object.keys(obj).reduce((acc, key) => {
        acc[this.toSnakeCase(key)] = this.keysToSnakeCase(obj[key]);
        return acc;
      }, {} as any);
    }
    return obj;
  }

  // -------------------------------------------------------
  // Công dụng: Đệ quy convert tất cả key của object hoặc array
  //            từ snake_case → camelCase
  // Đầu vào:
  //        obj: any - object hoặc array cần chuyển key
  // Trả về:
  //        any - object/array với key đã đổi sang camelCase
  // Ai tạo: ChatGPT
  // -------------------------------------------------------
  private keysToCamelCase(obj: any): any {
    if (Array.isArray(obj)) return obj.map((o) => this.keysToCamelCase(o));
    if (obj !== null && typeof obj === "object") {
      return Object.keys(obj).reduce((acc, key) => {
        const camelKey = this.toCamelCase(key);
        acc[camelKey] = this.keysToCamelCase(obj[key]);

        return acc;
      }, {} as any);
    }
    return obj;
  }

  // -------------------------------------------------------
  // Công dụng: Xây dựng URL
  // Đầu vào:
  //          url: string - URL gốc
  //          params?: Record<string, any> - đối tượng chứa tham số query
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  private buildUrl(url: string, params?: Record<string, any>) {
    if (!params || Object.keys(params).length === 0) return url;
    const query = new URLSearchParams(params).toString();
    return `${url}?${query}`;
  }

  // -------------------------------------------------------
  // Công dụng: Gửi request GET tới API với URL và params tùy chọn.
  //            - Xây dựng URL đầy đủ với query string nếu params tồn tại.
  // Đầu vào:
  //          url: string - đường dẫn endpoint
  //          params?: any - đối tượng chứa tham số query
  // Trả về: Promise<ApiResponse<T>> - dữ liệu từ server
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  async get<T>(url: string, params?: any): Promise<ApiResponse<T>> {
    const finalUrl = this.buildUrl(url, params);
    const response = await this.instance.get<ApiResponse<T>>(finalUrl);

    return {
      data: this.keysToSnakeCase(response.data.data),
      meta: response.data.meta,
      error: response.data.error,
    };
  }

  // -------------------------------------------------------
  // Công dụng: Gửi request POST tới API với URL, body và params tùy chọn.
  //            - Xây dựng URL đầy đủ với query string nếu params tồn tại.
  // Đầu vào:
  //          url: string - đường dẫn endpoint
  //          body?: any - dữ liệu gửi kèm trong body
  //          params?: any - đối tượng chứa tham số query
  // Trả về: Promise<ApiResponse<T>> - dữ liệu từ server
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  async post<T>(
    url: string,
    body?: any,
    params?: any
  ): Promise<ApiResponse<T>> {
    const finalUrl = this.buildUrl(url, params);
    const bodyCamel = this.keysToCamelCase(body);
    const response = await this.instance.post<ApiResponse<T>>(
      finalUrl,
      bodyCamel
    );

    return {
      data: this.keysToSnakeCase(response.data.data),
      meta: response.data.meta,
      error: response.data.error,
    };
  }

  // -------------------------------------------------------
  // Công dụng: Gửi request PUT tới API với URL, body và params tùy chọn.
  //            - Xây dựng URL đầy đủ với query string nếu params tồn tại.
  // Đầu vào:
  //          url: string - đường dẫn endpoint
  //          body?: any - dữ liệu gửi kèm trong body
  //          params?: any - đối tượng chứa tham số query
  // Trả về: Promise<ApiResponse<T>> - dữ liệu từ server
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  async put<T>(url: string, body?: any, params?: any): Promise<ApiResponse<T>> {
    const finalUrl = this.buildUrl(url, params);
    const bodyCamel = this.keysToCamelCase(body);
    const response = await this.instance.put<ApiResponse<T>>(
      finalUrl,
      bodyCamel
    );

    return {
      data: this.keysToSnakeCase(response.data.data),
      meta: response.data.meta,
      error: response.data.error,
    };
  }

  // -------------------------------------------------------
  // Công dụng: Gửi request DELETE tới API với URL và params tùy chọn.
  //            - Xây dựng URL đầy đủ với query string nếu params tồn tại.
  // Đầu vào:
  //          url: string - đường dẫn endpoint
  //          params?: any - đối tượng chứa tham số query
  // Trả về: Promise<ApiResponse<T>> - dữ liệu từ server
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  async delete<T>(url: string, params?: any): Promise<ApiResponse<T>> {
    const finalUrl = this.buildUrl(url, params);
    const response = await this.instance.delete<ApiResponse<T>>(finalUrl);

    return {
      data: this.keysToSnakeCase(response.data.data),
      meta: response.data.meta,
      error: response.data.error,
    };
  }

  // -------------------------------------------------------
  // Công dụng: Gửi request POST dạng multipart/form-data
  //            để upload file lên BE. Phương thức này không
  //            chuyển đổi camelCase/snakeCase và không bị
  //            ảnh hưởng bởi Content-Type mặc định của axios.
  //
  // Đầu vào:
  //      url: string       - đường dẫn endpoint của API
  //      formData: FormData - dữ liệu file cần upload
  //
  // Trả về:
  //      Promise<ApiResponse<T>> - phản hồi từ server gồm:
  //          data: dữ liệu dạng T
  //          meta: thông tin phân trang (nếu có)
  //          error: lỗi từ server (nếu có)
  //
  // Ai tạo: Phan Duy Anh
  // Tạo ngày: 11/18/2025
  // -------------------------------------------------------
  async postFormData<T>(
    url: string,
    formData: FormData
  ): Promise<ApiResponse<T>> {
    const response = await this.instance.post<ApiResponse<T>>(url, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });

    return {
      data: this.keysToSnakeCase(response.data.data),
      meta: response.data.meta,
      error: response.data.error,
    };
  }
}
