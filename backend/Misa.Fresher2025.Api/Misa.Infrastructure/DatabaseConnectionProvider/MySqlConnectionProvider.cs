using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Infrastructure.DatabaseConnectionProvider
{
    public class MySqlConnectionProvider
    {
        private readonly string _connectionString;

        // -------------------------------------------------------
        // Công dụng: Cung cấp phương thức khởi tạo và lấy đối tượng
        //            MySqlConnection dựa trên chuỗi kết nối truyền vào.
        //            Dùng để tạo kết nối tới database MySQL.
        // Đầu vào:
        //      - connectionString: Chuỗi kết nối đến database MySQL
        // Đầu ra:
        //      - Đối tượng MySqlConnection dùng để thực hiện truy vấn
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public MySqlConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        // -------------------------------------------------------
        // Công dụng: Tạo và trả về một đối tượng MySqlConnection
        //            sử dụng chuỗi kết nối đã được cung cấp cho class.
        // Đầu vào: Không có
        // Đầu ra: Đối tượng MySqlConnection mới, chưa mở kết nối
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
