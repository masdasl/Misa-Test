export interface Customer {
  customer_id?: string;
  customer_code: string;
  full_name: string;
  phone: string;
  email: string;
  address: string;
  tax_code: string;
  zalo?: string;
  customer_type?: string;
  shipping_address?: string;
  billing_address?: string;
  last_purchase_date?: string;
  purchased_items?: string;
  last_purchased_item?: string;
  avatar?: string;
  avatarPath?: string;
}
