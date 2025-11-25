using Misa.Core.CoreAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.DTo
{
    public class CustomerDto
    {
        public Guid? CustomerId { get; set; }
        [ColumnName("customer_code")]
        public string CustomerCode { get; set; }
        [ColumnName("full_name")]
        public string FullName { get; set; }
        [ColumnName("phone")]
        public string Phone { get; set; }
        [ColumnName("email")]
        public string Email { get; set; }
        [ColumnName("Zalo")]
        public string? Zalo { get; set; }
        [ColumnName("address")]
        public string? Address { get; set; }
        [ColumnName("tax_code")]
        public string TaxCode { get; set; }
        [ColumnName("customer_type")]
        public string? CustomerType { get; set; }
        [ColumnName("shipping_address")]
        public string? ShippingAddress { get; set; }
        [ColumnName("billing_address")]
        public string? BillingAddress { get; set; }
        [ColumnName("last_purchase_date")]
        public DateTime? LastPurchaseDate { get; set; }
        [ColumnName("purchased_items")]
        public string? PurchasedItems { get; set; }
        [ColumnName("last_purchased_item")]
        public string? LastPurchasedItem { get; set; }
        [ColumnName("avatar")]
        public string? Avatar { get; set; }
        [ColumnName("avatar_path")]
        public string? AvatarPath { get; set; }
    }
}
