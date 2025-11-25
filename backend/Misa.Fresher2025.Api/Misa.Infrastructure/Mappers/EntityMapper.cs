using Misa.Core.CoreAttribute;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Misa.Infrastructure.Mappers
{
    public static class EntityMapper
    {
        // -------------------------------------------------------
        // Công dụng: Ánh xạ (map) dữ liệu từ MySqlDataReader vào một
        //            đối tượng kiểu T. Hàm tự động đọc thuộc tính,
        //            kiểm tra attribute ColumnName hoặc PrimaryKey,
        //            và gán giá trị tương ứng từ reader.
        // Đầu vào:
        //      - reader: MySqlDataReader chứa dữ liệu truy vấn từ database
        // Đầu ra:
        //      - Đối tượng T đã được gán giá trị từ dữ liệu đọc được
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public static T Map<T>(MySqlDataReader reader) where T : class, new()
        {
            var entity = new T();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                try
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnNameAttribute>();
                    var pkAttr = prop.GetCustomAttribute<PrimaryKeyAttribute>();

                    string columnName = columnAttr?.ColumnName ?? pkAttr?.ColumnName ?? prop.Name;

                    var value = reader[columnName];

                    if (value != DBNull.Value)
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (targetType == typeof(Guid))
                            prop.SetValue(entity, Guid.Parse(value.ToString()));
                        else
                            prop.SetValue(entity, Convert.ChangeType(value, targetType));
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return entity;
        }

        // -------------------------------------------------------
        // Công dụng: Lấy danh sách các thuộc tính của kiểu T có gắn
        //            ColumnNameAttribute hoặc PrimaryKeyAttribute,
        //            đồng thời trả về tên cột tương ứng trong database.
        // Đầu vào:
        //      - Không có
        // Đầu ra:
        //      - List gồm các tuple (PropertyInfo prop, string column)
        //        đại diện cho thuộc tính và tên cột được ánh xạ
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public static List<(PropertyInfo prop, string column)> GetMappedProperties<T>()
        {
            return typeof(T)
                .GetProperties()
                .Select(p =>
                {
                    var colAttr = p.GetCustomAttribute<ColumnNameAttribute>();
                    var pkAttr = p.GetCustomAttribute<PrimaryKeyAttribute>();
                    var columnName = colAttr?.ColumnName ?? pkAttr?.ColumnName ?? p.Name.ToLower();
                    return (prop: p, column: columnName, isMapped: colAttr != null || pkAttr != null);
                })
                .Where(x => x.isMapped)
                .Select(x => (x.prop, x.column))
                .ToList();
        }
    }
}
