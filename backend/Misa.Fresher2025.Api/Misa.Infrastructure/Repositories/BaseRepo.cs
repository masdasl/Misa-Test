using Misa.Core.CoreAttribute;
using Misa.Core.DTo;
using Misa.Core.Interfaces.Repository;
using Misa.Infrastructure.DatabaseConnectionProvider;
using Misa.Infrastructure.Mappers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Misa.Infrastructure.Repositories
{
    public class BaseRepo<TEntity, TKey, TKeys, TDto> : IBaseRepo<TEntity, TKey, TKeys, TDto>
        where TEntity : class, new()
        where TDto : class, new()
    {
        protected readonly MySqlConnectionProvider _connectionProvider;
        protected readonly string _tableName;
        protected string _privateKey;

        protected MySqlConnection GetConnection() => _connectionProvider.GetConnection();

        public BaseRepo(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _tableName = typeof(TEntity).Name.ToLower();
            _privateKey = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                ?.GetCustomAttribute<PrimaryKeyAttribute>()?.ColumnName;
        }

        // -------------------------------------------------------
        // Công dụng: Lấy thông tin thực thể (entity) theo khóa chính
        //            từ bảng tương ứng. Hàm thực hiện truy vấn SELECT
        //            theo khóa chính và chỉ lấy bản ghi chưa bị xóa
        //            (is_deleted = 0).
        // Đầu vào:
        //      - id: Giá trị khóa chính của bản ghi cần lấy
        // Đầu ra:
        //      - IEnumerable<TEntity>: Danh sách chứa 1 entity nếu tìm thấy
        //      - null nếu không có bản ghi phù hợp
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual IEnumerable<TEntity>? GetById(TKey id)
        {
            using var conn = GetConnection();
            conn.Open();

            var pk = _privateKey;
            var cmd = new MySqlCommand($"SELECT * FROM {_tableName} WHERE {pk} = @id AND is_deleted = 0", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return new List<TEntity> { EntityMapper.Map<TEntity>(reader) };

            return null;
        }

        // -------------------------------------------------------
        // Công dụng: Thực hiện thêm mới nhiều bản ghi vào bảng tương ứng.
        //            Hàm sẽ:
        //              - Sinh khóa chính (GUID) cho từng bản ghi
        //              - Ánh xạ các thuộc tính từ DTO sang cột trong DB
        //              - Thực hiện INSERT theo transaction để đảm bảo
        //                toàn vẹn dữ liệu (nếu một bản ghi lỗi → rollback)
        // Đầu vào:
        //      - dtos: Mảng DTO chứa dữ liệu cần thêm vào database
        // Đầu ra:
        //      - "OK" nếu thêm thành công tất cả bản ghi
        //      - "Failed: <message>" nếu có lỗi xảy ra
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual string Insert(TDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0)
                return "Failed: No data to insert";

            using var conn = GetConnection();
            conn.Open();

            var mappedProps = EntityMapper.GetMappedProperties<TDto>().ToList();

            var columnNames = new List<string> { _privateKey };
            columnNames.AddRange(mappedProps.Select(p => p.column));

            var paramNames = new List<string> { "@" + _privateKey };
            paramNames.AddRange(mappedProps.Select(p => "@" + p.prop.Name));

            var sql = $"INSERT INTO {_tableName} ({string.Join(", ", columnNames)}) " +
                      $"VALUES ({string.Join(", ", paramNames)})";

            using var cmd = new MySqlCommand(sql, conn);
            using var transaction = conn.BeginTransaction();
            cmd.Transaction = transaction;

            try
            {
                foreach (var dto in dtos)
                {
                    cmd.Parameters.Clear();
                    var idValue = Guid.NewGuid().ToString();
                    cmd.Parameters.AddWithValue("@" + _privateKey, idValue);

                    foreach (var (prop, column) in mappedProps)
                    {
                        var value = prop.GetValue(dto);
                        if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                            value = DBNull.Value;

                        cmd.Parameters.AddWithValue("@" + prop.Name, value);
                    }

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows == 0)
                        throw new Exception("Insert failed for one row");
                }

                transaction.Commit();
                return "OK";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return "Failed: " + ex.Message;
            }
        }

        // -------------------------------------------------------
        // Công dụng: Cập nhật dữ liệu cho một bản ghi dựa trên khóa chính.
        //            Hàm sẽ ánh xạ thuộc tính từ DTO sang cột DB và thực hiện
        //            UPDATE trong transaction để đảm bảo an toàn dữ liệu.
        // Đầu vào:
        //      - id: Khóa chính của bản ghi cần cập nhật
        //      - dto: Đối tượng chứa dữ liệu mới
        // Đầu ra:
        //      - "OK" nếu cập nhật thành công
        //      - "Failed" hoặc "Failed: <message>" nếu lỗi hoặc không có bản ghi nào bị ảnh hưởng
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual string Update(TKey id, TDto dto)
        {
            if (dto == null)
                return "Failed";

            using var conn = GetConnection();
            conn.Open();

            var mappedProps = EntityMapper.GetMappedProperties<TDto>();
            if (mappedProps.Count == 0)
                return "Failed";
            var setClauses = mappedProps.Select(p => $"{p.column} = @{p.prop.Name}");
            var sql = $"UPDATE {_tableName} SET {string.Join(", ", setClauses)} WHERE {_privateKey} = @id";

            using var cmd = new MySqlCommand(sql, conn);
            using var transaction = conn.BeginTransaction();
            cmd.Transaction = transaction;

            try
            {
                foreach (var propTuple in mappedProps)
                {
                    var value = propTuple.prop.GetValue(dto) ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@" + propTuple.prop.Name, value);
                }

                cmd.Parameters.AddWithValue("@id", id);

                var affectedRows = cmd.ExecuteNonQuery();
                transaction.Commit();

                return affectedRows > 0 ? "OK" : "Failed";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return "Failed: " + ex.Message;
            }
        }

        // -------------------------------------------------------
        // Công dụng: Xóa mềm (soft delete) nhiều bản ghi bằng cách
        //            cập nhật trường is_deleted = 1 cho danh sách id truyền vào.
        // Đầu vào:
        //      - ids: Danh sách khóa chính cần xóa (mảng hoặc collection)
        // Đầu ra:
        //      - "OK" nếu xóa thành công ít nhất 1 bản ghi
        //      - "Failed" nếu lỗi hoặc không bản ghi nào được cập nhật
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual string Delete(TKeys ids)
        {
            using var conn = GetConnection();
            conn.Open();
            if (ids == null)
                return "Failed";
            var idArray = ids as Array;
            if (idArray == null || idArray.Length == 0)
                return "Failed";
            var paramNames = new string[idArray.Length];
            for (int i = 0; i < idArray.Length; i++)
            {
                paramNames[i] = $"@id{i}";
            }
            var sql = $"UPDATE {_tableName} SET is_deleted = 1 WHERE {_privateKey} IN ({string.Join(",", paramNames)})";

            using var cmd = new MySqlCommand(sql, conn);
            for (int i = 0; i < idArray.Length; i++)
            {
                cmd.Parameters.AddWithValue(paramNames[i], idArray.GetValue(i));
            }
            var affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0 ? "OK" : "Failed";
        }

        // -------------------------------------------------------
        // Công dụng: Kiểm tra xem giá trị truyền vào có tồn tại trong
        //            cột tương ứng của bảng hay không (dùng để validate).
        // Đầu vào:
        //      - columnName: Tên cột cần kiểm tra
        //      - value: Giá trị cần xác thực
        // Đầu ra:
        //      - Giá trị tìm thấy trong DB (nếu tồn tại)
        //      - default(TKey) nếu không tồn tại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/22/2025
        // -------------------------------------------------------
        public virtual TKey CheckValid(string columnName, TKey value)
        {
            using var conn = GetConnection();
            conn.Open();

            string sql = $"SELECT {columnName} FROM {_tableName} WHERE {columnName} = @value AND is_deleted = 0 LIMIT 1";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@value", value);

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return default;
            return (TKey)Convert.ChangeType(result, typeof(TKey));
        }

        // -------------------------------------------------------
        // Công dụng: Tải dữ liệu dạng bảng có phân trang, tìm kiếm,
        //            lọc và sắp xếp cho entity. Hàm tự động ánh xạ
        //            các thuộc tính sang cột DB để build điều kiện.
        // Các chức năng hỗ trợ:
        //      - Lọc theo nhiều trường
        //      - Tìm kiếm theo nhiều trường (LIKE %value%)
        //      - Sắp xếp theo trường tuỳ chọn
        //      - Phân trang (PageSize, PageNo)
        // Đầu vào:
        //      - dataTableRequest: Thông tin filter, search, sort, paging
        // Đầu ra:
        //      - Tuple gồm:
        //            + Data: danh sách entity sau khi truy vấn
        //            + Total: tổng số bản ghi thỏa điều kiện (phục vụ phân trang)
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public (IEnumerable<TEntity> Data, int Total) LoadCustomersTable(DataTableRequest dataTableRequest)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            string isVisibleData = " WHERE is_deleted = 0";
            var mappedProps = EntityMapper.GetMappedProperties<TEntity>();

            if (dataTableRequest.Filters != null)
            {
                foreach (var filter in dataTableRequest.Filters)
                {
                    if (string.IsNullOrEmpty(filter.Value)) continue;
                    var mapping = mappedProps.FirstOrDefault(p =>
                        string.Equals(p.prop.Name, filter.Key, StringComparison.OrdinalIgnoreCase));
                    if (mapping.prop != null)
                    {
                        string columnName = mapping.column;
                        isVisibleData += $" AND {columnName} = @{filter.Key}";
                        cmd.Parameters.AddWithValue("@" + filter.Key, filter.Value);
                    }
                }
            }
            if (dataTableRequest.SearchValue != null)
            {
                var orClauses = new List<string>();
                foreach (var search in dataTableRequest.SearchValue)
                {
                    if (string.IsNullOrEmpty(search.Value)) continue;
                    var mapping = mappedProps.FirstOrDefault(p =>
                        string.Equals(p.prop.Name, search.Key, StringComparison.OrdinalIgnoreCase));
                    if (mapping.prop != null)
                    {
                        string paramName = $"@search_{search.Key}";
                        orClauses.Add($"{mapping.column} LIKE {paramName}");
                        cmd.Parameters.AddWithValue(paramName, $"%{search.Value}%");
                    }
                }
                if (orClauses.Any())
                {
                    isVisibleData += " AND (" + string.Join(" OR ", orClauses) + ")";
                }
            }
            string countSql = $"SELECT COUNT(*) FROM {_tableName} {isVisibleData}";
            cmd.CommandText = countSql;

            int total = Convert.ToInt32(cmd.ExecuteScalar());

            string sql = $"SELECT * FROM {_tableName} {isVisibleData}";
            if (!string.IsNullOrEmpty(dataTableRequest.SortKey))
            {
                sql += $" ORDER BY {dataTableRequest.SortKey} {dataTableRequest.SortType}";
            }
            else
            {
                sql += $" ORDER BY created_at {dataTableRequest.SortType}";
            }
            int offset = (dataTableRequest.PageNo - 1) * dataTableRequest.PageSize;
            sql += " LIMIT @PageSize OFFSET @Offset";

            cmd.Parameters.AddWithValue("@PageSize", dataTableRequest.PageSize);
            cmd.Parameters.AddWithValue("@Offset", offset);

            cmd.CommandText = sql;

            var result = new List<TEntity>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var dto = EntityMapper.Map<TEntity>(reader);
                result.Add(dto);
            }

            return (result, total);
        }
    }
}
