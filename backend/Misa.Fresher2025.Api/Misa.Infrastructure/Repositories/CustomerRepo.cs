using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Interfaces.Repository;
using Misa.Infrastructure.DatabaseConnectionProvider;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Infrastructure.Repositories
{
    public class CustomerRepo : BaseRepo<Customer, string, string[], CustomerDto>, ICustomerRepo
    {
        public CustomerRepo(MySqlConnectionProvider connectionProvider)
           : base(connectionProvider)
        {
        }

        // -------------------------------------------------------
        // Công dụng: Lấy số thứ tự lớn nhất trong mã khách hàng
        //            dựa theo tiền tố prefix được truyền vào.
        //            Sử dụng truy vấn có FOR UPDATE để khóa bản ghi,
        //            đảm bảo an toàn trong môi trường nhiều luồng.
        // Đầu vào: prefix - Tiền tố mã khách hàng (vd: "KH202511")
        // Đầu ra: Số nguyên là số thứ tự lớn nhất (0 nếu chưa có bản ghi nào)
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public int GetMaxNumberByPrefix(string prefix)
        {
            using var conn = GetConnection();
            conn.Open();
            string sql = @"SELECT IFNULL(MAX(CAST(SUBSTRING(customer_code, 9, 6) AS UNSIGNED)),0)
               FROM customer
               WHERE customer_code LIKE @prefix AND is_deleted = 0 FOR UPDATE";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@prefix", prefix + "%");
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // -------------------------------------------------------
        // Công dụng: Lấy số thứ tự kế tiếp theo prefix truyền vào.
        //            Hàm sẽ:
        //              - Tìm mã khách hàng có tiền tố giống prefix
        //              - Lấy phần số lớn nhất (6 chữ số cuối)
        //              - Tăng lên 1 để tạo số thứ tự mới
        // Đầu vào: prefix - Tiền tố mã khách hàng (vd: "KH202511")
        // Đầu ra: Chuỗi số thứ tự mới (6 chữ số, ví dụ: "000123")
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public string createNumber(string prefix)
        {
            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                string sql = @"SELECT IFNULL(MAX(CAST(SUBSTRING(customer_code, 9, 6) AS UNSIGNED)), 0) 
                   FROM customer 
                   WHERE customer_code LIKE @prefix AND is_deleted = 0";
                using var cmd = new MySqlCommand(sql, conn, transaction);
                cmd.Parameters.AddWithValue("@prefix", prefix + "%");
                var result = cmd.ExecuteScalar();

                int nextNumber = (result != DBNull.Value && result != null)
                    ? Convert.ToInt32(result) + 1
                    : 1;

                transaction.Commit();

                return nextNumber.ToString("D6");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
