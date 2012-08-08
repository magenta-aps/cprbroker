using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CprBroker.Providers.CPRDirect
{
    public static class Extensions
    {
        public static void BulkInsertAll<T>(this SqlConnection conn, IEnumerable<T> entities, SqlTransaction trans)
        {
            entities = entities.ToArray();

            Type t = typeof(T);

            var tableAttribute = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false).Single();
            var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans)
            {
                DestinationTableName = tableAttribute.Name,
                BatchSize = 100
            };

            var properties = t.GetProperties().Where(EventTypeFilter).ToArray();
            var table = new DataTable();

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }
                table.Columns.Add(new DataColumn(property.Name, propertyType));
            }

            foreach (var entity in entities)
            {
                table.Rows.Add(properties.Select(
                  property => GetPropertyValue(
                  property.GetValue(entity, null))).ToArray());
            }

            bulkCopy.WriteToServer(table);
        }

        private static bool EventTypeFilter(System.Reflection.PropertyInfo p)
        {
            var attribute = Attribute.GetCustomAttribute(p,
                typeof(ColumnAttribute)) as ColumnAttribute;

            return attribute != null;
        }

        private static object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }

        public static void DeleteAll<T>(this SqlConnection conn, SqlTransaction transaction)
        {
            Type t = typeof(T);
            var tableAttribute = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false).Single();
            var command = new SqlCommand(string.Format("DELETE [{0}]", tableAttribute.Name.Replace(".", "].[")), conn, transaction);

            int ret = command.ExecuteNonQuery();
        }
    }
}
